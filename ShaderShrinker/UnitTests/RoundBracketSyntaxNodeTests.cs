// -----------------------------------------------------------------------
//  <copyright file="RoundBracketSyntaxNodeTests.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using NUnit.Framework;
using Shrinker.Lexer;
using Shrinker.Parser;
using Shrinker.Parser.SyntaxNodes;

namespace UnitTests
{
    [TestFixture]
    public class RoundBracketSyntaxNodeTests : UnitTestBase
    {
        [Test, Sequential]
        public void GivenEmptyCsvCheckIsNumericCsvReturnsFalse([Values] bool allowNumericVectors)
        {
            Assert.That(new RoundBracketSyntaxNode().IsNumericCsv(allowNumericVectors), Is.False);
        }

        [Test, Sequential]
        public void GivenSingleNonNumericCsvCheckIsNumericCsvReturnsFalse([Values] bool allowNumericVectors)
        {
            Assert.That(new RoundBracketSyntaxNode(new[] { new GenericSyntaxNode("a") }).IsNumericCsv(allowNumericVectors), Is.False);
        }

        [Test, Sequential]
        public void GivenMultiNonNumericCsvCheckIsNumericCsvReturnsFalse([Values] bool allowNumericVectors)
        {
            Assert.That(new RoundBracketSyntaxNode(new[] { new GenericSyntaxNode("a"), new GenericSyntaxNode(new IntToken(23)) }).IsNumericCsv(allowNumericVectors), Is.False);
        }

        [Test, Sequential]
        public void GivenSingleNumericCsvCheckIsNumericCsvReturnsTrue([Values] bool allowNumericVectors)
        {
            Assert.That(new RoundBracketSyntaxNode(new[] { new GenericSyntaxNode(new IntToken(23)) }).IsNumericCsv(allowNumericVectors), Is.True);
        }

        [Test, Sequential]
        public void GivenMultiNumericCsvCheckIsNumericCsvReturnsTrue([Values] bool allowNumericVectors)
        {
            Assert.That(new RoundBracketSyntaxNode(new[] { new GenericSyntaxNode(new IntToken(23)), new GenericSyntaxNode(FloatToken.From(32.23, 2)) }).IsNumericCsv(allowNumericVectors), Is.True);
        }
        
        [Test, Sequential]
        public void GivenVectorCsvCheckIsNumericCsvReturnsTrueOnlyIfAllowingNumericVectors([Values] bool allowNumericVectors)
        {
            var nodes = new SyntaxNode[]
            {
                new GenericSyntaxNode(new TypeToken("vec3")),
                new RoundBracketSyntaxNode(new []
                {
                    new GenericSyntaxNode(new IntToken(1)),
                    new GenericSyntaxNode(new CommaToken()),
                    new GenericSyntaxNode(FloatToken.From(12.3, 2))
                })
            };

            Assert.That(new RoundBracketSyntaxNode(nodes).IsNumericCsv(allowNumericVectors), Is.EqualTo(allowNumericVectors));
        }

        [Test, Combinatorial]
        public void CheckNonNumericCases([Values("(vec2(iTime))", "(vec3(a, b, c))", "(iTime)", "(a)", "(1 + 2)", "(vec2(1 + 2))", "()", "(())")] string code, [Values] bool allowNumericVectors)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var brackets = (RoundBracketSyntaxNode)new Parser(lexer).Parse().Root().Children.Single();
            Assert.That(brackets.IsNumericCsv(allowNumericVectors), Is.False);
        }

        [Test, Combinatorial]
        public void CheckNumericCasesWithVectorSupport([Values("(vec2(1))", "(vec3(1, 2, 3))", "(vec3(vec2(1, 2), 3))", "(1)", "(1.2)", "(1e1)", "(-1)", "(-1.2)", "(vec2(1e1))", "(vec3(-1))", "(vec4(-1.2))")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var brackets = (RoundBracketSyntaxNode)new Parser(lexer).Parse().Root().Children.Single();
            Assert.That(brackets.IsNumericCsv(true), Is.True);
        }
    }
}