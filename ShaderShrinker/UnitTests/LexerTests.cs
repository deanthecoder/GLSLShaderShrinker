// -----------------------------------------------------------------------
//  <copyright file="LexerTests.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Shrinker.Lexer;

namespace UnitTests
{
    [TestFixture]
    public class LexerTests : UnitTestBase
    {
        private static IEnumerable<FileInfo> TestFiles =>
            new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.EnumerateFiles("TestFiles/*.glsl");

        [Test, Sequential]
        public void CheckBuildingLexemeTokensSucceeds([ValueSource(nameof(TestFiles))] FileInfo testFile)
        {
            Assert.That(new Lexer().Load(testFile), Is.True);
        }

        [Test, Sequential]
        public void GivenLexemeTokensCheckConvertingBackToCodeSucceeds([ValueSource(nameof(TestFiles))] FileInfo testFile)
        {
            var lexer = new Lexer();
            lexer.Load(testFile);

            var sb = new StringBuilder();
            lexer.Tokens.ForEach(token => sb.Append(token.Content));

            var input = File.ReadAllText(testFile.FullName);
            var output = sb.ToString();

            Assert.That(output, Is.EqualTo(input));
        }
    }
}