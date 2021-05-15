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
        public bool RemoveDisabledCode { get; set; }
        public bool RemoveComments { get; set; }

        /// <summary>
        /// Requires RemoveComments to be enabled.
        /// </summary>
        public bool KeepHeaderComments { get; set; }

        public bool RemoveUnusedFunctions { get; set; }
        public bool SimplifyFunctionDeclarations { get; set; }
        public bool SimplifyFunctionParams { get; set; }
        public bool GroupVariableDeclarations { get; set; }
        public bool JoinVariableDeclarationsWithAssignments { get; set; }
        public bool RemoveUnusedVariables { get; set; }
        public bool DetectConstants { get; set; }
        public bool InlineConstantVariables { get; set; }
        public bool InlineDefines { get; set; }
        public bool SimplifyFloatFormat { get; set; }
        public bool SimplifyVectorConstructors { get; set; }
        public bool SimplifyVectorReferences { get; set; }
        public bool RemoveUnreachableCode { get; set; }
        public bool CombineConsecutiveAssignments { get; set; }
        public bool CombineAssignmentWithSingleUse { get; set; }
        public bool IntroduceMathOperators { get; set; }
        public bool SimplifyArithmetic { get; set; }
        public bool PerformArithmetic { get; set; }
        public bool SimplifyBranching { get; set; }

        private CustomOptions()
        {
        }

        public static CustomOptions All()
        {
            var options = SetAllOptions(true);
            options.KeepHeaderComments = false;
            return options;
        }

        public static CustomOptions None() => SetAllOptions(false);

        private static CustomOptions SetAllOptions(bool value)
        {
            var options = new CustomOptions();

            foreach (var propertyInfo in typeof(CustomOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                propertyInfo.SetValue(options, value);

            return options;
        }
    }
}