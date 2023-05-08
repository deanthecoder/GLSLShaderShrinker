// -----------------------------------------------------------------------
//  <copyright file="GlslOutputFormatter.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;
using Shrinker.Lexer;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser
{
    /// <summary>
    /// Takes a syntax tree and converts it back into GLSL code for output.
    /// </summary>
    public static class GlslOutputFormatter
    {
        public static string ToCode(this SyntaxNode rootNode, CustomOptions customOptions = null)
        {
            if (rootNode == null)
                throw new ArgumentNullException(nameof(rootNode));

            var sb = new StringBuilder();
            AppendCode(sb, rootNode, customOptions);
            
            if (customOptions?.TranspileToCSharp == true)
            {
                // 'const' not needed in C#.
                sb.Replace("const ", null);
                
                // 'inout' -> 'ref'
                sb.Replace("inout ", "ref ");
            }

            // Strip excessive newlines.
            var lines = sb.Replace("\r\n", "\n").ToString().Split('\n').Select(o => o.TrimEnd()).ToList();
            for (var i = 1; i < lines.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                    continue;
                if (!string.IsNullOrWhiteSpace(lines[i - 1]) && !lines[i - 1].TrimStart().StartsWith("//"))
                    continue;
                
                lines.RemoveAt(i);
                i--;
            }

            // Apply indents.
            var indentLevel = 0;
            for (var i = 0; i < lines.Count; i++)
            {
                if (lines[i].TrimStart().StartsWith("}"))
                    indentLevel = Math.Max(0, indentLevel - 1);

                if (indentLevel > 0 && !lines[i].TrimStart().StartsWith("#"))
                    lines[i] = new string('\t', indentLevel) + lines[i];
                lines[i] = lines[i].TrimEnd();

                var s = lines[i];
                var commentIndex = s.IndexOf("//", StringComparison.Ordinal);
                if (commentIndex > 0)
                    s = s.Substring(0, commentIndex - 1).TrimEnd();
                if (s.EndsWith("{"))
                    indentLevel++;
            }

            return string.Join("\n", lines).TrimEnd();
        }

        internal static void AppendCode(StringBuilder sb, SyntaxNode rootNode, CustomOptions customOptions)
        {
            switch (rootNode)
            {
                case FileSyntaxNode o:
                    var nodeBuilder = new StringBuilder();
                    foreach (var child in o.Children)
                    {
                        nodeBuilder.Clear();
                        AppendCode(nodeBuilder, child, customOptions);

                        if (child is not CommentSyntaxNodeBase)
                        {
                            // Format negative references, etc
                            nodeBuilder
                                .Replace("  - ", " -")
                                .Replace(", - ", ", -")
                                .Replace("( - ", "(-")
                                .Replace("( -", "(-")
                                .Replace("return - ", "return -")
                                .Replace("{  }", "{ }");
                            foreach (var ch in new[] { '*', '/', '+' })
                                nodeBuilder.Replace($"  {ch} ", $" {ch} ");
                        }

                        sb.Append(nodeBuilder);
                    }
                    break;

                case GenericSyntaxNode o:
                    if (o.Token != null)
                    {
                        switch (o.Token)
                        {
                            case CommaToken:
                                sb.Append(", ");
                                break;

                            case SemicolonToken:
                                if (sb.Length > 0 && sb[sb.Length - 1] == ' ')
                                    sb.Remove(sb.Length - 1, 1);

                                sb.Append(o.Parent == null || o.HasAncestor<RoundBracketSyntaxNode>() || o.HasAncestor<PragmaDefineSyntaxNode>() ? "; " : ";\n");
                                break;

                            case EqualityOperatorToken:
                            case AssignmentOperatorToken:
                                sb.Append($" {o.Token.Content} ");
                                break;

                            case SymbolOperatorToken t:
                                sb.Append(!t.IsAnyOf("++", "--", "!") ? $" {t.Content} " : t.Content);
                                break;

                            case UniformToken:
                            case PreprocessorToken:
                                if (sb.GetColumn() > 0)
                                    sb.AppendLine();
                                sb.Append($"{o.Token.Content} ");
                                break;

                            case KeywordToken t:
                                sb.Append(t.Content);

                                var isTerminator = t.IsAnyOf("continue", "break");
                                if (!isTerminator)
                                    sb.Append(' ');
                                break;

                            case TypeToken t:
                                var typeRequiresNewOperator =
                                    customOptions?.TranspileToCSharp == true &&
                                    !t.IsGlslType &&
                                    o.Parent?.Parent is not FunctionDefinitionSyntaxNode;
                                if (typeRequiresNewOperator)
                                    sb.Append("new ");

                                sb.Append(t.Content);
                                if (o.Next is not RoundBracketSyntaxNode && o.Next is not SquareBracketSyntaxNode)
                                    sb.Append(' ');
                                break;

                            case MiscCharacterToken:
                            case IntToken:
                            case DotToken:
                            case LineEndToken:
                                sb.Append(o.Token.Content);
                                break;

                            case FloatToken:
                                sb.Append(o.Token.Content);
                                if (customOptions?.TranspileToCSharp == true)
                                {
                                    if (sb[^1] == '.')
                                        sb.Append("0f");
                                    else if (sb[^1] != 'f')
                                        sb.Append('f');
                                }
                                break;

                            case AlphaNumToken:
                                // Re-insert spaces between words.
                                if (o.Previous?.Token is AlphaNumToken || o.Previous?.Token?.Content == ")")
                                    sb.Append(' ');

                                sb.Append(o.Token.Content);

                                if (o.Next is VariableAssignmentSyntaxNode or FunctionCallSyntaxNode or VariableDeclarationSyntaxNode)
                                    sb.Append(' ');
                                else if ((o.Next as GenericSyntaxNode)?.Token is INumberToken or TypeToken)
                                    sb.Append(' ');
                                break;

                            case ConstToken:
                                sb.Append("const ");
                                break;

                            default:
                                throw new SyntaxErrorException($"Unsupported token: {o.Token.GetType()}({o.Token.Content})");
                        }
                    }

                    break;

                case PragmaDefineSyntaxNode o:
                {
                    if (o.Previous != null && o.Previous is not PragmaDefineSyntaxNode)
                        sb.AppendLine();

                    sb.Append($"#define {o.Name}");

                    if (o.Params != null)
                        AppendCode(sb, o.Params, customOptions);

                    if (o.ValueNodes?.Any() == true)
                    {
                        var valueString = new StringBuilder();
                        o.ValueNodes.ToList().ForEach(child => AppendCode(valueString, child, customOptions));
                        valueString.Replace('\n', ' ').Replace('\r', ' ').Replace("  ", " ");
                        sb.Append($"\t{valueString.ToString().Trim()}");
                    }

                    sb.AppendLine();
                    if (o.Next is not PragmaDefineSyntaxNode)
                        sb.AppendLine();
                    break;
                }

                case VerbatimLineSyntaxNode o:
                    sb.Append(o.Token.Content);
                    break;

                case SquareBracketSyntaxNode o:
                    sb.Append('[');
                    o.Children.ToList().ForEach(child => AppendCode(sb, child, customOptions));
                    sb.Append(']');
                    if (o.Next is VariableAssignmentSyntaxNode)
                        sb.Append(' ');
                    break;

                case RoundBracketSyntaxNode o:
                    sb.Append('(');
                    o.Children.ToList().ForEach(child => AppendCode(sb, child, customOptions));
                    sb.Append(')');
                    if (o.Previous?.Token is KeywordToken)
                    {
                        if (o.Previous.HasNodeContent("for"))
                            sb.AppendLine();
                        else
                            sb.Append(' ');
                    }

                    break;

                case BraceSyntaxNode o:
                {
                    var subExpr = new StringBuilder();
                    o.Children.ToList().ForEach(child => AppendCode(subExpr, child, customOptions));
                    var s = subExpr.ToString().Trim();

                    var strings = s.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    var lineCount = strings.Length;

                    if (lineCount < 2 && o.Children.LastOrDefault() is not CommentSyntaxNodeBase)
                    {
                        sb.AppendLine($"{{ {s} }}");
                    }
                    else
                    {
                        sb.Append("{\n");
                        sb.AppendLine(s);
                        sb.Append("}\n");
                    }

                    break;
                }

                case ReturnSyntaxNode o:
                {
                    var subExpr = new StringBuilder();
                    o.Children.ToList().ForEach(child => AppendCode(subExpr, child, customOptions));
                    var s = subExpr.ToString().Trim();

                    sb.Append(string.IsNullOrEmpty(s) ? o.Name : $"{o.Name} {s}");
                    break;
                }

                case GroupSyntaxNode o:
                    o.Children.ToList().ForEach(child => AppendCode(sb, child, customOptions));
                    break;

                case VariableDeclarationSyntaxNode o:
                    sb.Append($"{o.VariableType.Content} ");

                    var isFirstChild = true;
                    foreach (var childDefinition in o.Children.Cast<VariableAssignmentSyntaxNode>())
                    {
                        if (!isFirstChild)
                        {
                            var lineStart = sb.Length - 1;
                            var lineTabs = 0;
                            while (lineStart > 0 && sb[lineStart - 1] != '\n')
                            {
                                if (sb[lineStart - 1] == '\t')
                                    lineTabs++;
                                lineStart--;
                            }

                            sb.Append(childDefinition.HasValue ? $",\n{new string('\t', lineTabs)}{new string(' ', o.VariableType.Content.Length + 1)}" : ", ");
                        }
                        
                        AppendCode(sb, childDefinition, customOptions);

                        isFirstChild = false;
                    }

                    if (!o.HasAncestor<RoundBracketSyntaxNode>())
                    {
                        sb.Append(';');
                        sb.AppendLine();
                    }
                    break;

                case VariableAssignmentSyntaxNode o:
                    if (o.IsArray && // An array
                        o.Parent is not VariableDeclarationSyntaxNode && // ...not defined in within a declaration.
                        o.ValueNodes.FirstOrDefault()?.Token is TypeToken &&
                        o.ValueNodes.FirstOrDefault()?.NextNonComment is SquareBracketSyntaxNode)
                        sb.Append(o.Name); // Exclude the array brackets [].
                    else
                        sb.Append(o.FullName);

                    var rhs = new StringBuilder();
                    o.ValueNodes.ToList().ForEach(child => AppendCode(rhs, child, customOptions));
                    if (rhs.Length > 0)
                        sb.Append($" = {rhs}");

                    if (!o.HasAncestor<VariableDeclarationSyntaxNode>())
                    {
                        sb.Append(';');

                        if (!o.HasAncestor<RoundBracketSyntaxNode>())
                            sb.AppendLine();
                        else
                            sb.Append(' ');
                    }

                    break;

                case StructDefinitionSyntaxNode o:
                    sb.AppendLine($"struct {o.Name} {{");
                    if (customOptions?.TranspileToCSharp == true)
                        StructDefinitionSyntaxNode.WriteConstructor(sb, o);
                    else
                        o.Braces.Children.ToList().ForEach(child => AppendCode(sb, child, customOptions));

                    sb.AppendLine("};\n");
                    break;

                case IfSyntaxNode o:
                    sb.Append("if ");
                    AppendCode(sb, o.Conditions, customOptions);
                    sb.Append(' ');

                    var braceCode = new StringBuilder();
                    AppendCode(braceCode, o.TrueBranch, customOptions);
                    var trueBranch = braceCode.ToString();
                    if (o.TrueBranch.Children.FirstOrDefault() is not IfSyntaxNode || o.FalseBranch == null)
                        trueBranch = trueBranch.AllowBraceRemoval();
                    sb.AppendLine(trueBranch);

                    if (o.FalseBranch != null)
                    {
                        sb.Append("else ");
                        braceCode.Clear();

                        if (o.FalseBranch.Children.Count == 1 && o.FalseBranch.Children.Single() is IfSyntaxNode subIfNode)
                            AppendCode(braceCode, subIfNode, customOptions);
                        else
                            AppendCode(braceCode, o.FalseBranch, customOptions);

                        sb.AppendLine(braceCode.ToString().AllowBraceRemoval());
                    }

                    if (o.Next != null && (o.FalseBranch != null || trueBranch.EndsWith("}")))
                        sb.AppendLine();

                    break;

                case SwitchSyntaxNode o:
                    sb.Append("switch ");
                    AppendCode(sb, o.Condition, customOptions);
                    sb.Append(' ');
                    var caseCode = new StringBuilder();
                    AppendCode(caseCode, o.Cases, customOptions);
                    caseCode.Replace(" :", ":");
                    sb.AppendLine(caseCode.ToString());
                    break;

                case PragmaIfSyntaxNode o:
                    if (sb.GetColumn() > 0)
                        sb.AppendLine();
                    sb.Append(o.Name);
                    break;

                case ForSyntaxNode o:
                    var indent = sb.GetColumn();
                    sb.Append("for ");
                    AppendCode(sb, o.LoopSetup, customOptions);

                    var loopCode = new StringBuilder();
                    AppendCode(loopCode, o.LoopCode, customOptions);
                    var braceSection = loopCode.ToString().AllowBraceRemoval();

                    if (braceSection.StartsWith("{"))
                    {
                        sb.Append(' ');
                    }
                    else
                    {
                        sb.AppendLine();
                        sb.Append(new string(' ', indent));
                        sb.Append('\t');
                    }

                    sb.AppendLine(braceSection);

                    if (o.Next != null)
                        sb.AppendLine();

                    break;

                case FunctionDeclarationSyntaxNode o:
                    sb.Append($"{o.ReturnType} {o.Name}");
                    var paramStr = new StringBuilder();
                    AppendCode(paramStr, o.Params, customOptions);
                    paramStr.Replace(" ,", ",");
                    paramStr.Replace(" )", ")");
                    sb.Append(paramStr);
                    sb.AppendLine(";");
                    break;

                case FunctionDefinitionSyntaxNode o:
                    sb.Append($"{o.ReturnType} {o.Name}");
                    AppendCode(sb, o.Params, customOptions);
                    sb.Append(' ');
                    
                    var braces = o.Braces;

                    if (customOptions?.TranspileToCSharp == true)
                    {
                        var typeTokens = o.Params.Children.Select(p => p.Token).OfType<TypeToken>().ToArray();
                        if (typeTokens.Any(t => t.InOut is TypeToken.InOutType.None or TypeToken.InOutType.In))
                        {
                            for (var i = 0; i < typeTokens.Length; i++)
                            {
                                // We only need to clone 'in' params (To stop the function changing the caller's value).
                                if (typeTokens[i].InOut != TypeToken.InOutType.None && typeTokens[i].InOut != TypeToken.InOutType.In)
                                    continue;

                                // Don't need to clone value types.
                                if (typeTokens[i].IsVector() || typeTokens[i].IsMatrix() || typeTokens[i].IsStruct())
                                    braces.InsertChild(0, new GenericSyntaxNode($"Clone(ref {o.ParamNames[i].Name});")); // todo - only clone if field set.
                            }
                        }
                    }
                    
                    AppendCode(sb, braces, customOptions);
                    sb.AppendLine();
                    break;
                
                case FunctionCallSyntaxNode o:
                    sb.Append(o.Name);
                    AppendCode(sb, o.Params, customOptions);
                    break;
                
                case CommentSyntaxNodeBase o:
                    if (o.IsAppendedToLine)
                    {
                        while (sb.Length > 0 && sb[sb.Length - 1].IsNewline())
                            sb.Remove(sb.Length - 1, 1);
                        sb.Append(' ');
                    }
                    else
                    {
                        if (o.Previous == null && o.Parent is BraceSyntaxNode or FileSyntaxNode ||
                            o.Previous is CommentSyntaxNodeBase)
                        {
                            // No newline necessary.
                        }
                        else
                        {
                            sb.AppendLine();
                        }
                    }

                    sb.AppendLine(o.Comment);
                    break;

                default:
                    var tail = TailString(sb);
                    if (rootNode == null)
                        throw new InvalidOperationException($"Output formatter failed - SyntaxNode is null.\nTail:\n{tail}");
                    throw new InvalidOperationException($"Output formatter failed - Unsupported SyntaxNode: {rootNode.GetType()}\nTail:\n{tail}");
            }
        }

        private static string TailString(StringBuilder sb)
        {
            if (sb.Length > 128)
            {
                sb.Insert(0, "...");
                sb.Remove(0, sb.Length - 128);
            }
            
            return sb.ToString();
        }
    }
}