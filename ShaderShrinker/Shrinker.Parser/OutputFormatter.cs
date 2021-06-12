// -----------------------------------------------------------------------
//  <copyright file="OutputFormatter.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
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
    public static class OutputFormatter
    {
        public static string ToCode(this SyntaxNode rootNode)
        {
            if (rootNode == null)
                throw new ArgumentNullException(nameof(rootNode));

            var sb = new StringBuilder();
            AppendCode(sb, rootNode);

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

        public static void AppendCode(StringBuilder sb, SyntaxNode rootNode)
        {
            switch (rootNode)
            {
                case FileSyntaxNode o:
                    var nodeBuilder = new StringBuilder();
                    foreach (var child in o.Children)
                    {
                        nodeBuilder.Clear();
                        AppendCode(nodeBuilder, child);

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
                                sb.Append(t.Content);
                                if (o.Next is not RoundBracketSyntaxNode && o.Next is not SquareBracketSyntaxNode)
                                    sb.Append(' ');
                                break;

                            case FloatToken:
                            case IntToken:
                            case DotToken:
                            case LineEndToken:
                                sb.Append(o.Token.Content);
                                break;

                            case AlphaNumToken:
                                // Re-insert spaces between words.
                                if (o.Previous?.Token is AlphaNumToken || o.Previous?.Token?.Content == ")")
                                    sb.Append(' ');

                                sb.Append(o.Token.Content);

                                if (o.Next is VariableAssignmentSyntaxNode or FunctionCallSyntaxNode or VariableDeclarationSyntaxNode)
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
                        AppendCode(sb, o.Params);

                    if (o.ValueNodes?.Any() == true)
                    {
                        var valueString = new StringBuilder();
                        o.ValueNodes.ToList().ForEach(child => AppendCode(valueString, child));
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
                    o.Children.ToList().ForEach(child => AppendCode(sb, child));
                    sb.Append(']');
                    if (o.Next is VariableAssignmentSyntaxNode)
                        sb.Append(' ');
                    break;

                case RoundBracketSyntaxNode o:
                    sb.Append('(');
                    o.Children.ToList().ForEach(child => AppendCode(sb, child));
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
                    o.Children.ToList().ForEach(child => AppendCode(subExpr, child));
                    var s = subExpr.ToString().Trim();

                    var strings = s.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    var lineCount = strings.Length;

                    if (lineCount < 2)
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
                    o.Children.ToList().ForEach(child => AppendCode(subExpr, child));
                    var s = subExpr.ToString().Trim();

                    sb.Append(string.IsNullOrEmpty(s) ? "return" : $"return {s}");
                    break;
                }

                case GroupSyntaxNode o:
                    o.Children.ToList().ForEach(child => AppendCode(sb, child));
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

                        AppendCode(sb, childDefinition);

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
                    o.ValueNodes.ToList().ForEach(child => AppendCode(rhs, child));
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
                    o.Braces.Children.ToList().ForEach(child => AppendCode(sb, child));
                    sb.AppendLine("};\n");
                    break;

                case IfSyntaxNode o:
                    sb.Append("if ");
                    AppendCode(sb, o.Conditions);
                    sb.Append(' ');

                    var braceCode = new StringBuilder();
                    AppendCode(braceCode, o.TrueBranch);
                    var trueBranch = braceCode.ToString().AllowBraceRemoval();
                    sb.AppendLine(trueBranch);

                    if (o.FalseBranch != null)
                    {
                        sb.Append("else ");
                        braceCode.Clear();

                        if (o.FalseBranch.Children.Count == 1 && o.FalseBranch.Children.Single() is IfSyntaxNode subIfNode)
                            AppendCode(braceCode, subIfNode);
                        else
                            AppendCode(braceCode, o.FalseBranch);

                        sb.AppendLine(braceCode.ToString().AllowBraceRemoval());
                    }

                    if (o.Next != null && (o.FalseBranch != null || trueBranch.EndsWith("}")))
                        sb.AppendLine();

                    break;

                case SwitchSyntaxNode o:
                    sb.Append("switch ");
                    AppendCode(sb, o.Condition);
                    sb.Append(' ');
                    var caseCode = new StringBuilder();
                    AppendCode(caseCode, o.Cases);
                    caseCode.Replace(" :", ":");
                    sb.AppendLine(caseCode.ToString());
                    break;

                case PragmaIfSyntaxNode o:
                    sb.Append(o.Name);
                    break;

                case ForSyntaxNode o:
                    var indent = sb.GetColumn();
                    sb.Append("for ");
                    AppendCode(sb, o.LoopSetup);

                    var loopCode = new StringBuilder();
                    AppendCode(loopCode, o.LoopCode);
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
                    AppendCode(paramStr, o.Params);
                    paramStr.Replace(" ,", ",");
                    paramStr.Replace(" )", ")");
                    sb.Append(paramStr);
                    sb.AppendLine(";");
                    break;

                case FunctionDefinitionSyntaxNode o:
                    sb.Append($"{o.ReturnType} {o.Name}");
                    AppendCode(sb, o.Params);
                    sb.Append(' ');
                    AppendCode(sb, o.Braces);
                    sb.AppendLine();
                    break;
                
                case FunctionCallSyntaxNode o:
                    sb.Append(o.Name);
                    AppendCode(sb, o.Params);
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
                    throw new InvalidOperationException($"Unsupported SyntaxNode: {rootNode.GetType()}");
            }
        }
    }
}