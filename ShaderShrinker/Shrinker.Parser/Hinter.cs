// -----------------------------------------------------------------------
//  <copyright file="Hinter.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Shrinker.Lexer;
using Shrinker.Parser.Hints;
using Shrinker.Parser.Optimizations;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser
{
    /// <summary>
    /// Takes a syntax tree produced by the Parser class, and creates a collection of helpful hints.
    /// </summary>
    public static partial class Hinter
    {
        // todo - If function called in 'if' (or 'else') blocks, suggest parameterizing.
        public static IEnumerable<CodeHint> GetHints(this SyntaxNode rootNode)
        {
            if (rootNode.HasEntryPointFunction())
            {
                foreach (var codeHint in DetectFunctionsToInline(rootNode))
                    yield return codeHint;

                foreach (var codeHint in DetectUnusedFunctionParam(rootNode))
                    yield return codeHint;

                foreach (var codeHint in CreateSameConstantParamPassedToFunctionHints(rootNode))
                    yield return codeHint;
            }

            foreach (var codeHint in DetectDefinableReferences(rootNode))
                yield return codeHint;

            foreach (var codeHint in DetectFunctionsCalledWithAllConstArguments(rootNode))
                yield return codeHint;
        }

        private static IEnumerable<CodeHint> DetectFunctionsToInline(SyntaxNode rootNode)
        {
            var localFunctions = rootNode.FunctionDefinitions().ToList();
            foreach (var function in localFunctions)
            {
                var callSites =
                    localFunctions
                        .Where(o => o != function)
                        .Sum(potentialCaller => potentialCaller.CallCount(function));

                // Called from a #define? If so, include that...
                callSites += rootNode.TheTree.OfType<PragmaDefineSyntaxNode>().Count(o => o.ToCode().Contains(function.Name));

                if (callSites == 0 && !function.IsMain())
                    yield return new UnusedFunctionHint(function.UiName);

                if (callSites == 1)
                    yield return new FunctionToInlineHint(function.UiName);
            }
        }

        private static IEnumerable<CodeHint> DetectUnusedFunctionParam(SyntaxNode rootNode)
        {
            var localFunctions = rootNode.FunctionDefinitions().ToList();
            foreach (var function in localFunctions.Where(o => !o.IsMain()))
            {
                foreach (var paramName in function.ParamNames.Select(o => o.Token.Content))
                {
                    var isUsed = function.Braces.TheTree
                        .Select(o => o.Token?.Content ?? (o as VariableAssignmentSyntaxNode)?.Name)
                        .Any(o => o?.StartsWithVarName(paramName) == true);
                    if (!isUsed)
                        yield return new FunctionHasUnusedParamHint(function.UiName, paramName);
                }
            }
        }

        private static IEnumerable<CodeHint> DetectDefinableReferences(SyntaxNode rootNode)
        {
            var candidates = new[] { "smoothstep", "iResolution", "normalize", "iTime", "iMouse" };
            var replacement = new[] { "S smoothstep", "R iResolution", "N normalize", "T time", "M iMouse" };
            var usages = rootNode.TheTree
                .Select(o => o.Token?.Content ?? (o as FunctionCallSyntaxNode)?.Name)
                .Where(o => candidates.Any(o.StartsWithVarName))
                .ToList();
            foreach (var candidate in candidates)
            {
                var usageCount = usages.Count(o => o.StartsWithVarName(candidate));
                var oldSize = candidate.Length * usageCount;
                var newSize = 8 + candidate.Length + usageCount;

                if (newSize < oldSize)
                    yield return new IntroduceDefineHint(candidate, replacement[candidates.ToList().IndexOf(candidate)]);
            }
        }

        /// <summary>
        /// If a function is called with all-constant params, the result might be inlined directly.
        /// </summary>
        private static IEnumerable<CodeHint> DetectFunctionsCalledWithAllConstArguments(SyntaxNode rootNode) =>
            rootNode
                .FunctionDefinitions()
                .SelectMany(o => o.FunctionCalls().FunctionCallsMadeWithConstParams())
                .Select(function => new FunctionCalledWithAllConstParamsHint(function));

        /// <summary>
        /// If all calls to a function have the same constant parameter, that parameter could be inlined.
        /// </summary>
        private static IEnumerable<CodeHint> CreateSameConstantParamPassedToFunctionHints(SyntaxNode rootNode)
        {
            var hints = new List<CodeHint>();
            MoveConstantParametersIntoCalledFunctionsExtension.DetectIssues(
                                                                            rootNode,
                                                                            (callers, paramIndex) => hints.Add(new AllCallsToFunctionMadeWithSameParamHint(callers[0].GetCallee(), paramIndex)));

            return hints.Distinct();
        }
    }
}