// -----------------------------------------------------------------------
//  <copyright file="OutputFormatterTests.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
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
    public class OutputFormatterTests : UnitTestBase
    {
        [Test]
        public void CheckFormattingSingleLineForStatement()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("for(i=0;i<2;i++) continue;"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            var newCode = parser.RootNode.ToCode().Trim();
            Assert.That(newCode, Is.EqualTo("for (i = 0; i < 2; i++) continue;"));
        }

        [Test]
        public void CheckFormattingMultiLineForStatement()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("for(i=0;i<2;i++) { i *= 2; i++; }"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            var newCode = parser.RootNode.ToCode().Trim();
            var lines = newCode.Split('\n').Select(o => o.Trim()).ToList();
            Assert.That(lines, Is.EqualTo(new[] { "for (i = 0; i < 2; i++) {", "i *= 2;", "i++;", "}" }));
        }

        [Test]
        public void CheckFormattingReturnStatementWithResult()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("return 1;"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            var newCode = parser.RootNode.ToCode().Trim();
            Assert.That(newCode, Is.EqualTo("return 1;"));
        }

        [Test]
        public void CheckFormattingReturnStatementWithNoResult()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("return;"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            var newCode = parser.RootNode.ToCode().Trim();
            Assert.That(newCode, Is.EqualTo("return;"));
        }
        
        [Test]
        public void CheckFormattingSingleLineCommentInCodeBlock()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("int i;\n// comment\nint j;"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            var newCode = parser.RootNode.ToCode();
            var lines = newCode.Split('\n').Select(o => o.Trim()).ToList();
            Assert.That(lines, Is.EqualTo(new[] { "int i;", string.Empty, "// comment", "int j;" }));
        }

        [Test]
        public void CheckFormattingSingleLineCommentAfterOpenBrace()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("{\n// comment\nreturn;\n}"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            var newCode = parser.RootNode.ToCode();
            var lines = newCode.Split('\n').Select(o => o.Trim()).ToList();
            Assert.That(lines, Is.EqualTo(new[] { "{", "// comment", "return;", "}" }));
        }

        [Test, Sequential]
        public void CheckFormattingSingleLineCommentAfterCloseBraceIsPrependedWithNewline([Values("{\n}\n// comment\n", "{\n}\n\n// comment\n")] string code)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(code), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            var newCode = parser.RootNode.ToCode();
            var lines = newCode.Split('\n').Select(o => o.ToSimple()).ToList();
            Assert.That(lines, Is.EqualTo(new[] { "{ }", string.Empty, "// comment" }));
        }

        [Test, Sequential]
        public void CheckSingleLineIfTrueBranchDoesNotRequireBraces([Values("if (true) return;", "if (true) return true;")] string code)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(code), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            var newCode = parser.RootNode.ToCode();
            Assert.That(newCode.ToSimple(), Is.EqualTo(code));
        }

        [Test, Sequential]
        public void CheckSingleLineIfFalseBranchDoesNotRequireBraces([Values("if (false) { } else return;", "if (false) { } else return true;")] string code)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(code), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            var newCode = parser.RootNode.ToCode();
            Assert.That(newCode.ToSimple(), Is.EqualTo(code));
        }

        [Test]
        public void CheckFormattingMultipleDeclarationsWithDefinitionsSpanMultipleLines()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("int d, a=1, b=2, c;"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            options.JoinVariableDeclarationsWithAssignments = true;
            var newCode = parser.RootNode.Simplify(options).ToCode();
            var lines = newCode.Split('\n').ToList();
            Assert.That(lines, Is.EqualTo(new[] { "int d, c,", "    a = 1,", "    b = 2;" }));
        }
    }
}