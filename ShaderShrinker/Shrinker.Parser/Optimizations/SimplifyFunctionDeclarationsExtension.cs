// -----------------------------------------------------------------------
//  <copyright file="SimplifyFunctionDeclarationsExtension.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using Shrinker.Lexer;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class SimplifyFunctionDeclarationsExtension
    {
        public static void SimplifyFunctionDeclarations(this SyntaxNode rootNode)
        {
            var functionDeclarations = rootNode.FunctionDeclarations().ToList();

            // Remove param names.
            foreach (var declaration in functionDeclarations.Where(o => o.Params.Children.Any()))
            {
                var paramNames = declaration.Params.Children
                    .OfType<GenericSyntaxNode>()
                    .Where(o => o.Token is AlphaNumToken)
                    .ToList();
                paramNames.ForEach(o => o.Remove());
            }

            // Remove declaration if no matching definition.
            var functionDefinitionNames = rootNode.FunctionDefinitions().Select(o => o.Name);
            var surplusDeclarations = functionDeclarations.Where(o => !functionDefinitionNames.Contains(o.Name)).ToList();

            // Remove declaration if next reference is the definition.
            var flatNodes = rootNode.TheTree.ToList();
            foreach (var declaration in functionDeclarations)
            {
                var definitionSite = rootNode.FunctionDefinitions().FirstOrDefault(o => o.Name == declaration.Name);
                var definitionIndex = flatNodes.IndexOf(definitionSite);

                var firstCallSite = flatNodes.OfType<FunctionCallSyntaxNode>().FirstOrDefault(o => o.Name == declaration.Name);
                var firstCallIndex = flatNodes.IndexOf(firstCallSite);

                if (firstCallIndex > definitionIndex)
                    surplusDeclarations.Add(declaration);
            }

            surplusDeclarations.ForEach(o => o.Remove());
        }
    }
}