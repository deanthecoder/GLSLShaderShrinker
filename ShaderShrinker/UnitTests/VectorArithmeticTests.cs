// -----------------------------------------------------------------------
//  <copyright file="VectorArithmeticTests.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using NUnit.Framework;
using Shrinker.Lexer;
using Shrinker.Parser;

namespace UnitTests
{
    [TestFixture]
    public class VectorArithmeticTests : UnitTestBase
    {
        [Test, Sequential]
        public void CheckSingleElementVectorArithmeticSimplifiesToFloat(
            [Values(
                       "vec2 main() { return vec2(1, 2) + vec2(3); }",
                       "vec2 main() { return vec2(1) + vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) - vec2(3); }",
                       "vec2 main() { return vec2(1) - vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) - vec2(-3); }",
                       "vec2 main() { return vec2(-1) - vec2(2, 3); }",
                       "vec2 main() { return -vec2(-1) - vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) * vec2(3); }",
                       "vec2 main() { return vec2(1) * vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) / vec2(3); }",
                       "vec2 main() { return vec2(1) / vec2(2, 3); }",
                       "vec2 main() { return vec2(1) + vec2(2); }",
                       "vec4 main() { return vec4(1) + vec4(2) * vec4(3.0); }"
                   )] string code,
            [Values(
                       "vec2 main() { return vec2(1, 2) + 3.; }",
                       "vec2 main() { return 1. + vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) - 3.; }",
                       "vec2 main() { return 1. - vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) + 3.; }",
                       "vec2 main() { return -1. - vec2(2, 3); }",
                       "vec2 main() { return 1. - vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) * 3.; }",
                       "vec2 main() { return 1. * vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) / 3.; }",
                       "vec2 main() { return 1. / vec2(2, 3); }",
                       "vec2 main() { return 1. + vec2(2); }",
                       "vec4 main() { return 1. + 2. * vec4(3.0); }"
                   )] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.SimplifyArithmetic = true;

            var rootNode = new Parser(lexer).Parse().Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        // todo - handle vector & float multiply (and divide. if shrinks)
    }
}