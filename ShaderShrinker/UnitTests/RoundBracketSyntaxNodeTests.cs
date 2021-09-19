// -----------------------------------------------------------------------
//  <copyright file="RoundBracketSyntaxNodeTests.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using NUnit.Framework;
using Shrinker.Lexer;
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
    }
}