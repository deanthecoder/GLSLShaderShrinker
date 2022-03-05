// -----------------------------------------------------------------------
//  <copyright file="MoveConstantParametersIntoCalledFunctionsExtension.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Shrinker.Lexer;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class MoveConstantParametersIntoCalledFunctionsExtension
    {
        /// <summary>
        /// Returns true if all optimizations should be re-run.
        /// </summary>
        public static bool MoveConstantParametersIntoCalledFunctions(this SyntaxNode rootNode)
        {
            var repeatSimplifications = false;

            while (true)
            {
                List<FunctionCallSyntaxNode> callers = null;
                var paramIndex = -1;
                DetectIssues(rootNode,
                             (issueCallers, issueParamIndex) =>
                             {
                                 callers = issueCallers;
                                 paramIndex = issueParamIndex;
                             });

                if (callers == null)
                    break;

                // Issue found - Get the constant from the call site.
                var constParamNodes = callers[0].Params.GetCsv().ElementAt(paramIndex).Select(o => o.Clone()).ToList();

                // Attempt to inline the parameter.
                var callee = callers[0].GetCallee();

                var paramNodesToMove = callee.Params.GetCsv().ToList()[paramIndex];
                paramNodesToMove.Select(o => o.Token).OfType<TypeToken>().ToList().ForEach(o => o.SetInOut());
                var typeNode = (GenericSyntaxNode)paramNodesToMove[0].Clone();
                var nameNode = (GenericSyntaxNode)paramNodesToMove[1].Clone();

                // Remove parameter from function definition.
                callee.GetDeclaration()?.Params.RemoveCsvEntry(paramIndex);
                callee.Params.RemoveCsvEntry(paramIndex);

                // Add assignment to head of function body.
                var declNode = new VariableDeclarationSyntaxNode(typeNode);
                declNode.Adopt(new VariableAssignmentSyntaxNode(nameNode, constParamNodes.ToList()));
                callee.Braces.InsertChild(0, declNode);

                // Remove param from all call sites.
                foreach (var caller in callers)
                    caller.Params.RemoveCsvEntry(paramIndex);

                repeatSimplifications = true;
            }

            return repeatSimplifications;
        }

        internal static void DetectIssues(SyntaxNode rootNode, Action<List<FunctionCallSyntaxNode>, int> onItemFound)
        {
            var localFunctions = rootNode.FunctionDefinitions().ToList();
            var functionCalls = localFunctions.SelectMany(o => o.FunctionCalls()).Where(o => o.GetCallee() != null).ToList();
            foreach (var functionCall in functionCalls.Where(o => o.Params.Children.Any()))
            {
                var callee = functionCall.GetCallee();
                var allCallsToThatFunction = functionCalls.Where(o => callee == o.GetCallee()).ToList();

                var paramCount = callee.ParamNames.Count;
                for (var paramIndex = 0; paramIndex < paramCount; paramIndex++)
                {
                    // Each nth param must be the same.
                    var paramStrings = allCallsToThatFunction.Select(o => o.Params.GetCsv().ToList()[paramIndex].Select(p => p.ToCode()).Aggregate((a, b) => $"{a} {b}"));
                    if (paramStrings.Distinct().Count() != 1)
                        continue;

                    // Each nth param must be a constant.
                    if (!allCallsToThatFunction.All(call => call.Params.IsNumericParam(paramIndex, true)))
                        continue;

                    // All match!
                    onItemFound(allCallsToThatFunction, paramIndex);
                }
            }
        }
    }
}