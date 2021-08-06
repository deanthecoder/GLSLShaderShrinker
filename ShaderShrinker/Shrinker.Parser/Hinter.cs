﻿// -----------------------------------------------------------------------
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
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser
{
    /// <summary>
    /// Takes a syntax tree produced by the Parser class, and creates a collection of helpful hints.
    /// </summary>
    public static class Hinter
    {
        // todo - Report when single value passed to any function.
        public static IEnumerable<CodeHint> GetHints(this SyntaxNode rootNode)
        {
            if (rootNode.HasEntryPointFunction())
            {
                foreach (var codeHint in DetectFunctionsToInline(rootNode))
                    yield return codeHint;

                foreach (var codeHint in DetectUnusedFunctionParam(rootNode))
                    yield return codeHint;
            }

            foreach (var codeHint in DetectDefinableReferences(rootNode))
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
                        yield return new FunctionHasUnusedParam(function.UiName, paramName);
                }
            }
        }

        private static IEnumerable<CodeHint> DetectDefinableReferences(SyntaxNode rootNode)
        {
            var candidates = new[] { "smoothstep", "iResolution", "normalize" };
            var replacement = new[] { "S smoothstep", "R iResolution", "N normalize" };
            var usages = rootNode.TheTree
                .Select(o => o.Token?.Content ?? (o as FunctionCallSyntaxNode)?.Name)
                .Where(o => candidates.Any(o.StartsWithVarName))
                .ToList();
            foreach (var candidate in candidates)
            {
                var usageCount = usages.Count(o => o.StartsWithVarName(candidate));
                var oldSize = candidate.Length * usageCount;
                var newSize = 8 + candidate.Length + usageCount + usageCount;

                if (newSize < oldSize)
                    yield return new IntroduceDefine(candidate, replacement[candidates.ToList().IndexOf(candidate)]);
            }
        }

        public class UnusedFunctionHint : CodeHint
        {
            public UnusedFunctionHint(string function) : base(function, "Function is never called.")
            {
            }
        }

        public class FunctionToInlineHint : CodeHint
        {
            public FunctionToInlineHint(string function) : base(function, "Function is called only once - Consider inlining.")
            {
            }
        }
        
        public class FunctionHasUnusedParam : CodeHint
        {
            public FunctionHasUnusedParam(string function, string param) : base(function, $"Function parameter '{param}' is unused.")
            {
            }
        }
        
        public class IntroduceDefine : CodeHint
        {
            public IntroduceDefine(string originalName, string defineNameAndValue) : base(originalName, $"[GOLF] Consider adding '#define {defineNameAndValue}'")
            {
            }
        }
    }
}