// -----------------------------------------------------------------------
//  <copyright file="GlslFunctionCallSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Shrinker.Parser.SyntaxNodes
{
    /// <summary>
    /// Represents a native GLSL function.
    /// </summary>
    public class GlslFunctionCallSyntaxNode : FunctionCallSyntaxNode
    {
        private static readonly HashSet<string> GlslFunctions = new HashSet<string>(
                                                                                    new[]
                                                                                    {
                                                                                        "abs",
                                                                                        "acos",
                                                                                        "acosh",
                                                                                        "all",
                                                                                        "any",
                                                                                        "asin",
                                                                                        "asinh",
                                                                                        "atan",
                                                                                        "atan",
                                                                                        "atanh",
                                                                                        "BitsToInt",
                                                                                        "BitsToUint",
                                                                                        "ceil",
                                                                                        "clamp",
                                                                                        "cos",
                                                                                        "cosh",
                                                                                        "cross",
                                                                                        "degrees",
                                                                                        "determinant",
                                                                                        "dFdx",
                                                                                        "dFdy",
                                                                                        "distance",
                                                                                        "dot",
                                                                                        "equal",
                                                                                        "exp",
                                                                                        "exp2",
                                                                                        "faceforward",
                                                                                        "floor",
                                                                                        "fract",
                                                                                        "fwidth",
                                                                                        "greaterThan",
                                                                                        "greaterThanEqual",
                                                                                        "intBitsTo",
                                                                                        "inverse",
                                                                                        "inversesqrt",
                                                                                        "isinf",
                                                                                        "isnan",
                                                                                        "length",
                                                                                        "lessThan",
                                                                                        "lessThanEqual",
                                                                                        "log",
                                                                                        "log2",
                                                                                        "matrixCompMult",
                                                                                        "max",
                                                                                        "min",
                                                                                        "mix",
                                                                                        "mod",
                                                                                        "modf",
                                                                                        "normalize",
                                                                                        "not",
                                                                                        "notEqual",
                                                                                        "outerProduct",
                                                                                        "packSnorm2x16",
                                                                                        "packUnorm2x16",
                                                                                        "pow",
                                                                                        "radians",
                                                                                        "reflect",
                                                                                        "refract",
                                                                                        "round",
                                                                                        "sign",
                                                                                        "sin",
                                                                                        "sinh",
                                                                                        "smoothstep",
                                                                                        "sqrt",
                                                                                        "step",
                                                                                        "tan",
                                                                                        "tanh",
                                                                                        "texelFetch",
                                                                                        "texelFetchOffset",
                                                                                        "texture",
                                                                                        "textureGrad",
                                                                                        "textureGradOffset",
                                                                                        "textureLod",
                                                                                        "textureLodOffset",
                                                                                        "textureProj",
                                                                                        "textureProjGrad",
                                                                                        "textureProjLod",
                                                                                        "textureProjLodOffset",
                                                                                        "textureSize",
                                                                                        "transpose",
                                                                                        "trunc",
                                                                                        "uintBitsTo",
                                                                                        "unpackSnorm2x16",
                                                                                        "unpackUnorm2x16"
                                                                                    });

        public GlslFunctionCallSyntaxNode(GenericSyntaxNode nameNode, RoundBracketSyntaxNode brackets)
            : base(nameNode, brackets)
        {
            if (!IsGlslFunction(nameNode.Token.Content))
                throw new ArgumentException($"Unknown GLSL function name: {nameNode.Token.Content}");
        }

        public static bool IsGlslFunction(string name) => GlslFunctions.Contains(name);

        public override bool HasOutParam => false;
    }
}