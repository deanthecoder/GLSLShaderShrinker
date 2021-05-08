// -----------------------------------------------------------------------
//  <copyright file="CustomOptions.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Reflection;

namespace Shrinker.Parser
{
    /// <summary>
    /// Options to toggle each of the available optimizations. (Default: Everything enabled)
    /// </summary>
    public class CustomOptions
    {
        public bool RemoveDisabledCode { get; set; } = true;
        public bool RemoveComments { get; set; } = true;

        /// <summary>
        /// Requires RemoveComments to be enabled.
        /// </summary>
        public bool KeepHeaderComments { get; set; }

        public bool RemoveUnusedFunctions { get; set; } = true;
        public bool SimplifyFunctionDeclarations { get; set; } = true;
        public bool SimplifyFunctionParams { get; set; } = true;
        public bool GroupVariableDeclarations { get; set; } = true;
        public bool JoinVariableDeclarationsWithAssignments { get; set; } = true;
        public bool RemoveUnusedVariables { get; set; } = true;
        public bool DetectConstants { get; set; } = true;
        public bool InlineConstantVariables { get; set; } = true;
        public bool InlineDefines { get; set; } = true;
        public bool SimplifyFloatFormat { get; set; } = true;
        public bool SimplifyVectorConstructors { get; set; } = true;
        public bool SimplifyVectorReferences { get; set; } = true;
        public bool RemoveUnreachableCode { get; set; } = true;
        public bool CombineConsecutiveAssignments { get; set; } = true;
        public bool CombineAssignmentWithSingleUse { get; set; } = true;
        public bool IntroduceMathOperators { get; set; } = true;
        public bool SimplifyArithmetic { get; set; } = true;
        public bool PerformArithmetic { get; set; } = true;
        public bool SimplifyBranching { get; set; } = true;

        public static CustomOptions Disabled()
        {
            var options = new CustomOptions();

            foreach (var propertyInfo in typeof(CustomOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                propertyInfo.SetValue(options, false);

            return options;
        }
    }
}