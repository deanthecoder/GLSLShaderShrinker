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
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class RemoveUnusedFunctionsExtension
    {
        public static void RemoveUnusedFunctions(this SyntaxNode rootNode)
        {
            // There must be an entry point function defined, otherwise do nothing.
            // (E.g. Prevents the 'common' code from all being removed.)
            if (!rootNode.HasEntryPointFunction())
                return;

            while (true)
            {
                var functionRemoved = false;

                var localFunctions = rootNode.FunctionDefinitions().ToList();
                foreach (var testFunction in localFunctions.Where(o => !o.IsMain()))
                {
                    var otherFunctions = localFunctions.Where(o => o != testFunction).ToList();
                    if (otherFunctions.Any(o => o.DoesCall(testFunction)))
                        continue; // Function was used.

                    // Perhaps used by a #define?
                    if (rootNode.TheTree.OfType<PragmaDefineSyntaxNode>().Any(o => o.ToCode().Contains(testFunction.Name)))
                        continue; // Yup - Used.

                    // Function not used - Remove it (and any matching declaration).
                    testFunction.GetDeclaration()?.Remove();
                    testFunction.Remove();
                    functionRemoved = true;
                }

                if (!functionRemoved)
                    return;
            }
        }
    }
}