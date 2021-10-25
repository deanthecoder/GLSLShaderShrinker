// -----------------------------------------------------------------------
//  <copyright file="ReplaceFunctionCallsWithResultExtension.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2021 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Shrinker.Lexer;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class ReplaceFunctionCallsWithResultExtension
    {
        /// <summary>
        /// Returns true if all optimizations should be re-run.
        /// </summary>
        public static bool ReplaceFunctionCallsWithResult(this SyntaxNode rootNode)
        {
            var repeatSimplifications = false;

            while (true)
            {
                var didChange = false;

                foreach (var functionCall in rootNode.FunctionCalls().Where(o => o.IsFunctionCallWithConstParams()).ToList())
                {
                    // Clone the callee function, so we can attempt to simplify it.
                    var callee = (FunctionDefinitionSyntaxNode)functionCall.GetCallee().Clone();

                    var callerParams = functionCall.Params.Children.Select(o => o.Clone()).ToList();

                    // Convert params to local variables.
                    var nodesToPrefix = new List<VariableDeclarationSyntaxNode>();
                    var paramIndex = 0;
                    var i = 0;
                    while (i < callee.Params.Children.Count)
                    {
                        if (callee.Params.Children[i].Token is CommaToken)
                        {
                            // Skip the commas.
                            i++;
                            continue;
                        }

                        var typeNode = (GenericSyntaxNode)callee.Params.Children[i++].Clone();
                        var nameNode = (GenericSyntaxNode)callee.Params.Children[i++].Clone();

                        var declNode = new VariableDeclarationSyntaxNode(typeNode);
                        declNode.Adopt(new VariableAssignmentSyntaxNode(nameNode, new List<SyntaxNode> { callerParams[paramIndex * 2] }));
                        paramIndex++;

                        nodesToPrefix.Add(declNode);
                    }

                    // Move new nodes to head of function body.
                    callee.Params.ReplaceWith(new RoundBracketSyntaxNode());
                    for (i = 0; i < nodesToPrefix.Count; i++)
                        callee.Braces.InsertChild(i, nodesToPrefix[i]);

                    // Re-parse and our cloned function.
                    var lexer = new Lexer.Lexer();
                    if (!lexer.Load(callee.Braces.ToCode()))
                        continue;
                    var reparsed = new Parser(lexer).Parse();

                    // 'Shrink' our cloned function.
                    var simplified = reparsed.Simplify();
                    if (simplified.Children.FirstOrDefault() is BraceSyntaxNode braces &&
                        braces.Children.FirstOrDefault() is ReturnSyntaxNode returnNode)
                    {
                        // Inline the function call with its result.
                        functionCall.ReplaceWith(returnNode.Children.ToList());

                        didChange = true;
                        break;
                    }
                }

                if (!didChange)
                    break;

                repeatSimplifications = true;
            }

            return repeatSimplifications;
        }

        public static IEnumerable<FunctionCallSyntaxNode> FunctionCallsMadeWithConstParams(this IEnumerable<FunctionCallSyntaxNode> functionCalls) =>
            functionCalls.Where(o => o.IsFunctionCallWithConstParams());

        private static bool IsFunctionCallWithConstParams(this FunctionCallSyntaxNode functionCall)
        {
            if (!functionCall.Params.IsNumericCsv(true) ||
                functionCall.ModifiesGlobalVariables() ||
                functionCall.HasOutParam)
                return false;

            var callee = functionCall.GetCallee();
            return callee != null && callee.ReturnType != "void" && !callee.UsesGlslInputs() && !callee.CallsLocalFunctions();
        }
    }
}