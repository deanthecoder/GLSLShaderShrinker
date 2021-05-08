// -----------------------------------------------------------------------
//  <copyright file="RemoveUnusedFunctionsExtension.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;

namespace Shrinker.Parser.Optimizations
{
    public static class RemoveUnusedFunctionsExtension
    {
        public static void RemoveUnusedFunctions(this SyntaxNode rootNode)
        {
            while (true)
            {
                var functionRemoved = false;

                var localFunctions = rootNode.FindFunctionDefinitions().ToList();
                foreach (var testFunction in localFunctions.Where(o => !o.IsMain()))
                {
                    var otherFunctions = localFunctions.Where(o => o != testFunction).ToList();
                    if (otherFunctions.SelectMany(o => o.Braces.TheTree)
                        .OfType<FunctionCallSyntaxNode>()
                        .Any(o => o.Name == testFunction.Name))
                        continue; // Function was used.

                    // Function not used - Remove it (and any matching declaration).
                    testFunction.Remove();
                    rootNode.Children.OfType<FunctionDeclarationSyntaxNode>().FirstOrDefault(o => o.Name == testFunction.Name)?.Remove();
                    functionRemoved = true;
                }

                if (!functionRemoved)
                    return;
            }
        }
    }
}