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

// todo - Support [] (https://www.shadertoy.com/view/Nd2XzG)
// todo - 1e3 form can be used if with vecN(...)
// todo - 'fragColor = vec4(col, 1.0)' - Inline 'col'. (Subway)
// todo - ED-209 (float d = .01 * t * .33;)
// todo - Remove 'return;' specifically at end of function.
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
            options ??= new CustomOptions();

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
                    rootNode.TheTree.ToList().ForEach(o => (o.Token as DoubleNumberToken)?.Simplify());

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
                    repeatSimplifications = rootNode.IntroduceMathOperators(repeatSimplifications);

                if (options.SimplifyArithmetic)
                    repeatSimplifications = rootNode.SimplifyArithmetic(repeatSimplifications);

                if (options.PerformArithmetic)
                    repeatSimplifications = rootNode.PerformArithmetic(repeatSimplifications);
            }

            return rootNode;
        }
    }
}