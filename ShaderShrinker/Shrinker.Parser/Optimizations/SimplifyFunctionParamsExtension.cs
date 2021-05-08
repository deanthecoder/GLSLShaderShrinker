// -----------------------------------------------------------------------
//  <copyright file="SimplifyFunctionParamsExtension.cs">
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

namespace Shrinker.Parser.Optimizations
{
    public static class SimplifyFunctionParamsExtension
    {
        public static void SimplifyFunctionParams(this SyntaxNode rootNode)
        {
            // Remove any 'in' keywords. ('in' is the default in GLSL.)
            var inTypes = rootNode.TheTree.Select(o => o.Token).OfType<TypeToken>().Where(o => o.InOut == TypeToken.InOutType.In);
            inTypes.ToList().ForEach(o => o.SetInOut());

            // Remove void params from function declarations/definitions.
            rootNode.Children
                .OfType<FunctionSyntaxNodeBase>()
                .Where(o => o.IsVoidParam())
                .ToList()
                .ForEach(o => o.Params.Children.Single().Remove());
        }
    }
}