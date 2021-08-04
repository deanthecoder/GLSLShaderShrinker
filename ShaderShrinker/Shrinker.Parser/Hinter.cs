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
            if (!rootNode.HasEntryPointFunction())
                yield break;

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
                    yield return new CodeHint(function.UiName, "Never used - Consider removing.");

                if (callSites == 1)
                    yield return new CodeHint(function.UiName, "Only called once - Consider inlining.");
            }
        }
    }
}