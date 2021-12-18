// -----------------------------------------------------------------------
//  <copyright file="RemoveUnusedFunctionsExtension.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;
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
                    if (rootNode.TheTree.OfType<PragmaDefineSyntaxNode>().Any(o => DoesPragmaDefineReferenceFunction(o, testFunction)))
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

        private static bool DoesPragmaDefineReferenceFunction(PragmaDefineSyntaxNode define, FunctionDefinitionSyntaxNode function)
        {
            var defineCode = define.ToCode();
            var functionIndex = defineCode.IndexOf(function.Name, StringComparison.Ordinal);
            if (functionIndex == -1)
                return false;

            var cppCommentIndex = defineCode.IndexOf("//", StringComparison.Ordinal);
            var cCommentIndex = defineCode.IndexOf("/*", StringComparison.Ordinal);
            var comments = new[] { cppCommentIndex, cCommentIndex }.Where(o => o >= 0).ToList();
            if (comments.Any() && comments.Min() < functionIndex)
                return false; // Reference found, but in a comment.

            return true;
        }
    }
}