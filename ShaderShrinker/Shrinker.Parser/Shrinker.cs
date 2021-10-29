// -----------------------------------------------------------------------
//  <copyright file="Shrinker.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using Shrinker.Parser.Optimizations;
using Shrinker.Parser.SyntaxNodes;

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

                if (options.SimplifyNumberFormat)
                    rootNode.SimplifyNumberFormat();

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
                    var anyChanges = rootNode.CombineAssignmentWithReturn();
                    anyChanges |= rootNode.CombineAssignmentWithSingleUse();
                    if (anyChanges && options.RemoveUnusedVariables)
                        rootNode.RemoveUnusedVariables();
                }

                if (options.SimplifyBranching)
                    rootNode.SimplifyBranching();

                if (options.IntroduceMathOperators)
                    repeatSimplifications |= rootNode.IntroduceMathOperators();

                if (options.SimplifyArithmetic)
                    repeatSimplifications |= rootNode.SimplifyArithmetic();

                if (options.PerformArithmetic)
                    repeatSimplifications |= rootNode.PerformArithmetic();

                if (options.ReplaceFunctionCallsWithResult)
                    repeatSimplifications |= rootNode.ReplaceFunctionCallsWithResult();

                if (options.MoveConstantParametersIntoCalledFunctions)
                    repeatSimplifications |= rootNode.MoveConstantParametersIntoCalledFunctions();
            }

            return rootNode;
        }
    }
}