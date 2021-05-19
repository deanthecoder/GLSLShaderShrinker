// -----------------------------------------------------------------------
//  <copyright file="Shrinker.cs">
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
using Shrinker.Parser.Optimizations;
using Shrinker.Parser.SyntaxNodes;

// todo - (if rhs doesn't use 'a') a *= 2.0; a *= foo; ... => a *= 2.0 * foo; (https://www.shadertoy.com/view/MdXXW2 sound, https://www.shadertoy.com/view/NsSXWd)
// todo - (if rhs doesn't use 'a') a += 2.0; a += foo; ... => a += 2.0 + foo;
// todo - Support ++i
// todo - int(1.2) => 1, int(1) => 1, float(1.2) => 1.2, float(1) => 1.
namespace Shrinker.Parser
{
    /// <summary>
    /// Takes a syntax tree produced by the Parser class, and optimizes it.
    /// </summary>
    /// <remarks>The output from this stage can be passed to the OutputFormatter.</remarks>
    public static class Shrinker
    {
        public static SyntaxNode Simplify(this SyntaxNode rootNode, CustomOptions options = null)
        {
            options ??= CustomOptions.All();

            var repeatSimplifications = true;
            while (repeatSimplifications)
            {
                repeatSimplifications = false;

                if (options.RemoveDisabledCode)
                    rootNode.RemoveDisabledCode();

                if (options.RemoveUnusedFunctions)
                    rootNode.RemoveUnusedFunctions();

                if (options.RemoveComments)
                    rootNode.RemoveComments(options);

                if (options.RemoveUnusedVariables)
                    rootNode.RemoveUnusedVariables();

                if (options.SimplifyFunctionDeclarations)
                    rootNode.SimplifyFunctionDeclarations();

                if (options.GroupVariableDeclarations)
                    rootNode.GroupVariableDeclarations();

                if (options.JoinVariableDeclarationsWithAssignments || options.InlineConstantVariables)
                    rootNode.JoinVariableDeclarationsWithAssignments();

                if (options.DetectConstants)
                    repeatSimplifications = rootNode.DetectConstants();

                if (options.InlineDefines)
                    rootNode.InlineDefines();

                if (options.InlineConstantVariables)
                    rootNode.InlineConstantVariables();

                if (options.SimplifyFloatFormat)
                    rootNode.TheTree.ToList().ForEach(o => (o.Token as FloatToken)?.Simplify());

                if (options.SimplifyVectorConstructors)
                    rootNode.SimplifyVectorConstructors();

                if (options.SimplifyVectorReferences)
                    rootNode.SimplifyVectorReferences();

                if (options.SimplifyVectorConstructors)
                    rootNode.RemoveUnnecessaryVectorConstructors();

                if (options.SimplifyFunctionParams)
                    rootNode.SimplifyFunctionParams();

                if (options.RemoveUnreachableCode)
                    rootNode.RemoveUnreachableCode();

                if (options.CombineConsecutiveAssignments)
                    rootNode.CombineConsecutiveAssignments();

                if (options.CombineAssignmentWithSingleUse)
                {
                    rootNode.CombineAssignmentWithReturn();
                    rootNode.CombineAssignmentWithSingleUse();
                }

                if (options.SimplifyBranching)
                    rootNode.SimplifyBranching();

                if (options.IntroduceMathOperators)
                    repeatSimplifications |= rootNode.IntroduceMathOperators();

                if (options.SimplifyArithmetic)
                    repeatSimplifications |= rootNode.SimplifyArithmetic();

                if (options.PerformArithmetic)
                    repeatSimplifications |= rootNode.PerformArithmetic();
            }

            return rootNode;
        }
    }
}