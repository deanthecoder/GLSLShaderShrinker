// -----------------------------------------------------------------------
//  <copyright file="TranspileToCSharpExtension.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Shrinker.Lexer;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations;

public static class TranspileToCSharpExtension
{
    public static void TranspileToCSharp(this SyntaxNode rootNode)
    {
        EnsureSafeNumericCasts(rootNode);
        EnsureVectorsAreInitialized(rootNode);
        RemoveNeverDefinedPragmaIfDefs(rootNode);
        RemoveVrEntryPoint(rootNode);
        ApplyRefKeywordToInOutFunctionCallParams(rootNode);
        InlineMacrosThatLooksLikeFunctions(rootNode);
        CloneDuringVariableAssignments(rootNode);
    }

    /// <summary>
    /// 'vec2 v2 = v1' will clone in GLSL but not in C#, so we need an explicit '.Clone()' call.
    /// </summary>
    private static void CloneDuringVariableAssignments(SyntaxNode rootNode)
    {
        var assignments = rootNode.TheTree
            .OfType<VariableAssignmentSyntaxNode>()
            .Where(
                   o => o.ValueNodes.Count() == 1 &&
                        o.ValueNodes.Single() is GenericSyntaxNode { Token: AlphaNumToken })
            .Select(o => (GenericSyntaxNode)o.ValueNodes.Single())
            .ToArray();
        foreach (var rhs in assignments)
        {
            var rhsDeclType = rhs.FindVarDeclaration()?.VariableType;
            if (rhsDeclType == null)
            {
                // Couldn't find it - Perhaps it's a function parameter?
                var funcDef = rhs.FindAncestor<FunctionDefinitionSyntaxNode>();
                if (funcDef != null)
                {
                    var paramMatch = funcDef.Params.GetCsv().FirstOrDefault(o => o.Any(t => t.Token?.Content == rhs.Token.Content));
                    rhsDeclType = paramMatch?.Select(o => o.Token).OfType<TypeToken>().FirstOrDefault();
                }

                if (rhsDeclType == null)
                {
                    // Don't know that the type is, so can't safely 'clone'.
                    continue;
                }
            }

            if (rhsDeclType.IsVector() || rhsDeclType.IsMatrix() || rhsDeclType.IsStruct())
            {
                // Replace the rhs with a clone.
                rhs.ReplaceWith(new GenericSyntaxNode($"{rhs.Token.Content}.Clone()"));
            }
        }
    }

    private static void InlineMacrosThatLooksLikeFunctions(SyntaxNode rootNode)
    {
        var defineNodes = rootNode.TheTree
            .OfType<PragmaDefineSyntaxNode>()
            .Where(o => o.HasParams && o.HasValues)
            .Where(o => o.ValueNodes.Count == 2 && o.ValueNodes[1] is RoundBracketSyntaxNode)
            .ToList();
        var macroNames = defineNodes.Select(o => o.Name).ToArray();
        var callSites = rootNode.TheTree
            .OfType<GenericSyntaxNode>()
            .Where(
                   o => (o.Parent is not PragmaDefineSyntaxNode parent || parent.Name != o.Name) &&
                        o.Next is RoundBracketSyntaxNode && macroNames.Contains(o.Name))
            .ToArray();

        var didInline = false;
        foreach (var callSite in callSites)
        {
            var defineNode = defineNodes.First(o => o.Name == callSite.Name);
            var macroParamNames = defineNode.Params.GetCsv().Select(o => o[0].UiName).ToList();
            var defineValueBrackets = (RoundBracketSyntaxNode)defineNode.ValueNodes[1];

            // Replace the name part of the define.
            var originalCallSiteParamNode = (RoundBracketSyntaxNode)callSite.ReplaceWith(defineNode.ValueNodes[0].Clone()).Next;

            // Now replace the params section.
            var newCallSiteParamNode = (RoundBracketSyntaxNode)originalCallSiteParamNode.ReplaceWith(defineValueBrackets.Clone());

            // ...and replace the macro param names with original content from the call site.
            foreach (var paramName in macroParamNames)
            {
                var toRename = newCallSiteParamNode.Children.OfType<GenericSyntaxNode>().Where(o => o.Name == paramName).ToArray();
                foreach (var node in toRename)
                {
                    var paramIndex = macroParamNames.IndexOf(node.Name);
                    if (paramIndex == -1)
                        continue;
                    
                    node.ReplaceWith(originalCallSiteParamNode.GetCsv().ElementAt(paramIndex).Select(o => o.Clone()));
                    didInline = true;
                }
            }
        }

        if (didInline)
            defineNodes.ForEach(o => o.Remove());
    }

    /// <summary>
    /// GLSL inout params map to 'ref' in C#, so the param at the call site needs prepending with 'ref'.
    /// </summary>
    private static void ApplyRefKeywordToInOutFunctionCallParams(SyntaxNode rootNode)
    {
        var functionCalls = rootNode.TheTree
            .OfType<FunctionCallSyntaxNode>()
            .Where(o => o.HasOutParam);
        foreach (var functionCall in functionCalls)
        {
            var callee = functionCall.GetCallee();
            if (callee == null)
                continue;

            var outParamIndices = new List<int>();
            var i = 0;
            foreach (var paramNodes in callee.Params.GetCsv())
            {
                if (paramNodes.Any(o => o.Token is TypeToken { InOut: TypeToken.InOutType.InOut }))
                    outParamIndices.Add(i);
                i++;
            }

            var paramsNodes = functionCall.Params.GetCsv().ToArray();
            foreach (var paramNode in outParamIndices.Select(outParamIndex => paramsNodes[outParamIndex][0]))
                paramNode.InsertBefore(new GenericSyntaxNode("ref"));
        }
    }

    /// <summary>
    /// mainVR is just not needed in C#.
    /// </summary>
    private static void RemoveVrEntryPoint(SyntaxNode rootNode)
    {
        rootNode.Children
            .OfType<FunctionDefinitionSyntaxNode>()
            .Where(o => o.Name == "mainVR")
            .ToList()
            .ForEach(o => o.Remove());
    }

    /// <summary>
    /// There's no #ifdef in C#, so apply the relevant branch instead.
    /// </summary>
    private static void RemoveNeverDefinedPragmaIfDefs(SyntaxNode rootNode)
    {
        var ifdefNodes = rootNode.TheTree
            .OfType<PragmaIfSyntaxNode>()
            .Where(o => o.Name.StartsWith("#ifdef"))
            .ToList();
        foreach (var ifdefNode in ifdefNodes)
        {
            if (ifdefNode.HasFalseBranch)
                ifdefNode.ReplaceWithFalseBranch();
            else
                ifdefNode.RemoveAll();
        }
    }

    /// <summary>
    /// GLSL vectors (and matrices) are value types and initialized by default. Not so in GLSL, so we must 'new()' them.
    /// </summary>
    private static void EnsureVectorsAreInitialized(SyntaxNode rootNode) =>
        rootNode.TheTree
            .OfType<VariableDeclarationSyntaxNode>()
            .Where(o => !o.HasAncestor<StructDefinitionSyntaxNode>())
            .Where(o => o.VariableType.IsVector() || o.VariableType.IsMatrix() || o.VariableType.IsStruct())
            .SelectMany(o => o.Definitions.Where(def => !def.HasValue))
            .ToList()
            .ForEach(def => def.Adopt(new GenericSyntaxNode("new()")));

    /// <summary>
    /// GLSL used int(1.2), but C# needs toInt(1.2).
    /// </summary>
    private static void EnsureSafeNumericCasts(SyntaxNode rootNode) =>
        rootNode.TheTree
            .OfType<GenericSyntaxNode>()
            .Where(
                   o => o.Token is TypeToken { IsGlslType: true } typeToken &&
                        typeToken.IsAnyOf("int", "float"))
            .Where(o => o.Next is RoundBracketSyntaxNode)
            .ToList()
            .ForEach(o => o.Rename(o.Name, $"to{o.Name.CapitalizeFirst()}"));
}