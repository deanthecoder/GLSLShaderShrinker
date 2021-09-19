// -----------------------------------------------------------------------
//  <copyright file="TypeToken.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Shrinker.Lexer
{
    [DebuggerDisplay("{" + nameof(Content) + "}")]
    public class TypeToken : IToken
    {
        private readonly string m_content;

        public enum InOutType
        {
            None,
            In,
            Out,
            InOut
        }

        public string Content
        {
            get
            {
                var s = m_content;
                switch (InOut)
                {
                    case InOutType.None:
                        break;
                    case InOutType.In:
                        s = $"in {s}";
                        break;
                    case InOutType.Out:
                        s = $"out {s}";
                        break;
                    case InOutType.InOut:
                        s = $"inout {s}";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (IsUniform)
                    s = $"uniform {s}";

                if (IsConst)
                    s = $"const {s}";

                return s;
            }
        }

        public bool IsConst { get; set; }
        public bool IsUniform { get; set; }
        public InOutType InOut { get; private set; }
        public bool IsGlslType => MultiValueTypes.Contains(m_content) || Names.Contains(m_content);

        public static IEnumerable<string> MultiValueTypes { get; } = new[]
        {
            "vec2", "vec3", "vec4",
            "uvec2", "uvec3", "uvec4",
            "ivec2", "ivec3", "ivec4",
            "bvec2", "bvec3", "bvec4",
            "mat2", "mat2x2", "mat2x3", "mat2x4",
            "mat3", "mat3x2", "mat3x3", "mat3x4",
            "mat4", "mat4x2", "mat4x3", "mat4x4"
        };
        public static IEnumerable<string> Names { get; } = new[] { "void", "bool", "int",  "uint", "float", "sampler1D", "sampler2D", "sampler3D", "samplerCube" }.Union(MultiValueTypes);

        public static IEnumerable<string> GlslInputs { get; } = new[]
        {
            "iResolution",
            "iTime",
            "iTimeDelta",
            "iFrame",
            "iFrameRate",
            "iChannelTime",
            "iChannelResolution",
            "iMouse",
            "iChannel",
            "iDate",
            "iSampleRate"
        };

        public IToken TryJoin(List<IToken> tokens, int tokenIndex, out int deletePrevious, out int deleteTotal)
        {
            deletePrevious = 0;
            deleteTotal = 0;

            var i = tokenIndex - 1;
            while (i > 0 && tokens[i] is WhitespaceToken)
                i--;
            if (i < 0 || !tokens[i].IsAnyOf("in", "out", "inout"))
                return null;

            // Remove in/out/inout prefix and combine with 'this'.
            deletePrevious = tokenIndex - i;
            deleteTotal = deletePrevious + 1;
            SetInOut(tokens[i].Content);

            return this;
        }

        public TypeToken(string content)
        {
            m_content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public bool SetInOut(string inOutType = null)
        {
            switch (inOutType)
            {
                case "in":
                    InOut = InOutType.In;
                    return true;
                case "out":
                    InOut = InOutType.Out;
                    return true;
                case "inout":
                    InOut = InOutType.InOut;
                    return true;
                default:
                    InOut = InOutType.None;
                    return false;
            }
        }
    }
}