// -----------------------------------------------------------------------
//  <copyright file="ParserTests.cs">
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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Shrinker.Lexer;
using Shrinker.Parser;
using Shrinker.Parser.SyntaxNodes;

namespace UnitTests
{
    [TestFixture]
    public class ParserTests : UnitTestBase
    {
        private static IEnumerable<FileInfo> TestFiles =>
            new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.EnumerateFiles("TestFiles/*.glsl");

        [Test, Sequential]
        public void CheckBuildingSyntaxTreeSucceeds([ValueSource(nameof(TestFiles))] FileInfo testFile)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(testFile), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);
        }

        [Test, Sequential]
        public void CheckRecombineSyntaxTreeToTextSucceeds([ValueSource(nameof(TestFiles))] FileInfo testFile)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(testFile), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            var newCode = parser.RootNode.ToCode();
            Assert.That(newCode, Is.Not.Null);
            Assert.That(newCode, Is.Not.Empty);
        }

        [Test, Sequential]
        public void CheckPragmaDefineWithNoNameThrowsSyntaxError(
            [Values("#define", "#define   ", "#define\n", "#define  \n")]
            string code)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(code), Is.True);

            Assert.That(() => new Parser(lexer).Parse(), Throws.TypeOf<SyntaxErrorException>());
        }

        [Test, Sequential]
        public void CheckValidPragmaDefineDoesNotThrow(
            [Values("#define FOO", "#define FOO 1", "#define FOO(x) x\n", "#define FOO(x, y) (x + y)", "#   define FOO")]
            string code)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(code), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            Assert.That(parser.RootNode.Children.Single(), Is.TypeOf<PragmaDefineSyntaxNode>());
            Assert.That(((PragmaDefineSyntaxNode)parser.RootNode.Children.Single()).Name, Is.EqualTo("FOO"));
        }

        [Test]
        public void CheckPragmaDefineWithNoArgsAndBracketedExpressionDoesNotMergeBracketsWithName()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("#define ZERO (min(iFrame,0))"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            Assert.That(parser.RootNode.ToCode().ToSimple(), Is.EqualTo("#define ZERO (min(iFrame, 0))"));
        }

        [Test]
        public void CheckPragmaDefineWithCodeValueIsNotSplitOntoNewLines()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("#define FOO(x) if (d < dMin) { dMin = d;  idObj = x; }"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            Assert.That(parser.RootNode.ToCode(), Is.EqualTo("#define FOO(x)\tif (d < dMin) { dMin = d; idObj = x; }"));
        }

        [Test]
        public void CheckDefineWithValueDefiningAFunctionDoesNotHaveFunctionRemoved()
        {
            const string Code = "#define F() bool inside(){ }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            Assert.That(parser.RootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }

        [Test]
        public void CheckMultiLinePragmaDefineIsReducedToSingleLine()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("#  define FOO(x)\\\n if (d < dMin) \\ \n{ dMin = d; idObj = x; }"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            Assert.That(parser.RootNode.ToCode(), Is.EqualTo("#define FOO(x)\tif (d < dMin) { dMin = d; idObj = x; }"));
        }

        [Test]
        public void CheckDetectingPragmaIfInRoot()
        {
            var lexer = new Lexer();
            lexer.Load("#if A\nint i = 12; #endif");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.Children.First(), Is.TypeOf<PragmaIfSyntaxNode>());
        }

        [Test]
        public void CheckDetectingPragmaIfInFunction()
        {
            var lexer = new Lexer();
            lexer.Load("void foo() {\n#if A\nreturn;\n#endif\n}");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.Children.Single(), Is.TypeOf<FunctionDefinitionSyntaxNode>());
            Assert.That(((FunctionDefinitionSyntaxNode)rootNode.Children.Single()).Braces.Children.First(), Is.TypeOf<PragmaIfSyntaxNode>());
        }

        [Test]
        public void CheckFoldingBraces()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("bool foo() { return true; }"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var functionNode = (FunctionDefinitionSyntaxNode)parser.RootNode.Children.Single();
            Assert.That(functionNode.Braces.Children, Has.Count.EqualTo(2));
        }

        [Test]
        public void CheckFoldingRoundBrackets()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("return ((1.0 + 2.0) * 3.0);"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            Assert.That(parser.RootNode.Children, Has.Count.EqualTo(2));
            Assert.That(parser.RootNode.Children[0].Children, Has.Count.EqualTo(1));
            Assert.That(parser.RootNode.Children[0].Children[0].Children, Has.Count.EqualTo(3));
        }

        [Test]
        public void CheckFoldingMixtureOfBrackets()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("if (true) { return (1 + 2); }"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var ifNode = (IfSyntaxNode)parser.RootNode.Children.Single();
            Assert.That(ifNode.Conditions.Children, Has.Count.EqualTo(1));
            Assert.That(ifNode.TrueBranch.Children, Has.Count.EqualTo(2));
            Assert.That(ifNode.FalseBranch, Is.Null);
        }

        [Test, Sequential]
        public void CheckNewlinesAreRemoved([Values("\n", "\r", "\r\n")] string newline)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load($"{newline}#define FOO  {newline} void p() {{}}{newline}"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            Assert.That(parser.RootNode.Children, Has.Count.EqualTo(2));
        }

        [Test, Sequential]
        public void CheckDetectingFunctionDefinitionWithNoParams(
            [Values("vec3 f() { return vec3(1); }", "void func( ) { glow++; }", "void construct() { }")]
            string code)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(code), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            Assert.That(parser.RootNode.Children.Single(), Is.TypeOf<FunctionDefinitionSyntaxNode>());
        }

        [Test]
        public void CheckDetectingStructDefinitionsWithNoVariableInstance()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("struct FOO { int i; vec2 xy; };"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            Assert.That(parser.RootNode.Children.Single(), Is.TypeOf<StructDefinitionSyntaxNode>());
        }

        [Test]
        public void CheckDetectingStructDefinitionsWithNonArrayVariableInstance()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("struct FOO { int i; vec2 xy; } foo;"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            Assert.That(parser.RootNode.Children, Has.Count.EqualTo(2));
            Assert.That(parser.RootNode.Children[0], Is.TypeOf<StructDefinitionSyntaxNode>());
            Assert.That(parser.RootNode.Children[1], Is.TypeOf<VariableDeclarationSyntaxNode>());
        }

        [Test]
        public void CheckDetectingStructDefinitionsWithArrayVariableInstance()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("struct FOO { int i; } foo[2];"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var declaration = (VariableDeclarationSyntaxNode)parser.RootNode.Children[1];
            Assert.That(declaration.Definitions.Single().FullName, Is.EqualTo("foo[2]"));
        }

        [Test]
        public void CheckDetectingSingleVariableDeclaration()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("int foo;"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var node = parser.RootNode.Children.Single();
            Assert.That(node, Is.TypeOf<VariableDeclarationSyntaxNode>());
            Assert.That(((VariableDeclarationSyntaxNode)node).VariableType.Content, Is.EqualTo("int"));
            Assert.That(((VariableDeclarationSyntaxNode)node).Definitions.Single().Name, Is.EqualTo("foo"));
        }

        [Test]
        public void CheckDetectingVariableDeclarationWithinFor()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("for (int i = 0; i < 4; i++) {}"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            Assert.That(parser.RootNode.Children.Single(), Is.TypeOf<ForSyntaxNode>());
        }

        [Test]
        public void CheckDetectingMultipleVariableDeclarations()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("vec3 a, b, c;"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes.Single(), Is.TypeOf<VariableDeclarationSyntaxNode>());
            Assert.That(((VariableDeclarationSyntaxNode)nodes.Single()).VariableType.Content, Is.EqualTo("vec3"));
            Assert.That(((VariableDeclarationSyntaxNode)nodes.Single()).Definitions.Select(o => o.Name), Is.EqualTo(new[] { "a", "b", "c" }));
        }

        [Test, Sequential]
        public void CheckDetectingVariableDefinition(
            [Values("int num; num = 23;", "int num; num = 23 + (1 + 2);", "int num; num = 23 + 3;")]
            string code)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(code), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes, Has.Count.EqualTo(2));
            Assert.That(nodes[0], Is.TypeOf<VariableDeclarationSyntaxNode>());
            Assert.That(nodes[1], Is.TypeOf<VariableAssignmentSyntaxNode>());
        }

        [Test]
        public void CheckDetectingVariableDefinitionWithinDeclarations1()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("int a, num = 23, b, c = 10;"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes.Single(), Is.TypeOf<VariableDeclarationSyntaxNode>());

            var declaration = (VariableDeclarationSyntaxNode)nodes.Single();
            Assert.That(declaration.Definitions.ToList(), Has.Count.EqualTo(4));
            Assert.That(declaration.Definitions.Where(o => o.HasValue).ToList(), Has.Count.EqualTo(2));
        }

        [Test]
        public void CheckDetectingVariableDefinitionWithinDeclarations2()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("int num = (2 * (3 + 2)) * 3, v = 10;"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes.Single(), Is.TypeOf<VariableDeclarationSyntaxNode>());

            var declaration = (VariableDeclarationSyntaxNode)nodes.Single();
            Assert.That(declaration.Definitions.ToList(), Has.Count.EqualTo(2));
            Assert.That(declaration.Definitions.Where(o => o.HasValue).ToList(), Has.Count.EqualTo(2));
        }

        [Test]
        public void CheckDetectingVariableDefinitionWithStructType()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("struct Foo { int a; }; Foo foo;"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes, Has.Count.EqualTo(2));
            Assert.That(nodes[0], Is.TypeOf<StructDefinitionSyntaxNode>());
            Assert.That(nodes[1], Is.TypeOf<VariableDeclarationSyntaxNode>());
        }

        [Test, Sequential]
        public void CheckDetectingFunctionDeclarations(
            [Values("void foo();", "vec3 bar(out vec2 uv);")]
            string code)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(code), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes.Single(), Is.TypeOf<FunctionDeclarationSyntaxNode>());
        }

        [Test]
        public void CheckDetectingFunctionCallWithinFunctionBody()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("void func() { } void main() { func(); }"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes, Has.Count.EqualTo(2));
            Assert.That(((FunctionDefinitionSyntaxNode)nodes[1]).Braces.Children[0], Is.TypeOf<FunctionCallSyntaxNode>());
            Assert.That(((FunctionDefinitionSyntaxNode)nodes[1]).Braces.Children[1].Token, Is.TypeOf<SemicolonToken>());
        }

        [Test]
        public void CheckDetectingFunctionCallWithinExpression()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("int func(int i) { return i + 1; } void main() { int i = (func(23) * 2); }"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes, Has.Count.EqualTo(2));

            var funcMainContent = ((FunctionDefinitionSyntaxNode)nodes[1]).Braces;

            var declaration = (VariableDeclarationSyntaxNode)funcMainContent.Children.Single();
            var roundBrackets = (RoundBracketSyntaxNode)declaration.Definitions.Single().Children.Single();
            Assert.That(roundBrackets.Children, Has.Count.EqualTo(3));
            Assert.That(roundBrackets.Children[0], Is.TypeOf<FunctionCallSyntaxNode>());
        }

        [Test]
        public void CheckDetectingFunctionCallWithinFunctionArgs()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("int a(int i) { return i; } int b() { return 2; } void main() { int i = a(b()); }"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes, Has.Count.EqualTo(3));

            var funcMainContent = ((FunctionDefinitionSyntaxNode)nodes[2]).Braces;
            var declaration = (VariableDeclarationSyntaxNode)funcMainContent.Children.Single();

            var callA = (FunctionCallSyntaxNode)declaration.Definitions.Single().Children.Single();
            Assert.That(callA.Params.Children.Single(), Is.TypeOf<FunctionCallSyntaxNode>());
        }

        [Test]
        public void CheckDetectingSingleLineCommentOnOwnLine()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("void main() {\n// a comment\n }"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes.Single(), Is.TypeOf<FunctionDefinitionSyntaxNode>());

            var functionNode = (FunctionDefinitionSyntaxNode)nodes.Single();
            Assert.That(functionNode.Braces.Children.Single(), Is.TypeOf<SingleLineCommentSyntaxNode>());
            Assert.That(((SingleLineCommentSyntaxNode)functionNode.Braces.Children.Single()).IsAppendedToLine, Is.False);
        }

        [Test]
        public void CheckDetectingSingleLineCommentContainingSymbols()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("// Word #~@:¬`|\\\nvoid main() { }"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes[0], Is.TypeOf<SingleLineCommentSyntaxNode>());
            Assert.That(nodes[1], Is.TypeOf<FunctionDefinitionSyntaxNode>());
        }

        [Test]
        public void CheckCommentFormatIsNotPostProcessed()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("// foo  - bar"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            Assert.That(parser.RootNode.ToCode(), Is.EqualTo("// foo  - bar"));
        }

        [Test]
        public void CheckDetectingSingleLineCommentAppendedToLine()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("int foo; // an int.\n"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes, Has.Count.EqualTo(2));
            Assert.That(nodes[0], Is.TypeOf<VariableDeclarationSyntaxNode>());
            Assert.That(nodes[1], Is.TypeOf<SingleLineCommentSyntaxNode>());
            Assert.That(((SingleLineCommentSyntaxNode)nodes[1]).IsAppendedToLine, Is.True);
        }

        [Test]
        public void CheckBraceExpressionEndingWithCommentIsNotRepresentedInASingleLine()
        {
            const string Code = "int f() {\n\treturn 1; // comment\n}";
            var lexer = new Lexer();
            Assert.That(lexer.Load(Code), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            Assert.That(parser.RootNode.ToCode(), Is.EqualTo(Code));
        }

        [Test, Sequential]
        public void CheckDetectingIntegers([Values("0", "1", "10", "-3")] string code)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(code), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            Assert.That(parser.RootNode.Children.Single(), Is.TypeOf<GenericSyntaxNode>());
            Assert.That(parser.RootNode.Children.Single().Token, Is.TypeOf<IntToken>());
        }

        [Test]
        public void CheckDetectingNegativeIntegers1()
        {
            var lexer = new Lexer();
            lexer.Load("-4");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.Children.Single().Token, Is.TypeOf<IntToken>());
            Assert.That(((IntToken)rootNode.Children.Single().Token).Content, Is.EqualTo("-4"));
        }

        [Test]
        public void CheckDetectingNegativeIntegers2()
        {
            var lexer = new Lexer();
            lexer.Load("(-4)");

            var rootNode = new Parser(lexer).Parse();

            var numberToken = rootNode.TheTree.Single(o => o.Token is IntToken).Token;
            Assert.That(((IntToken)numberToken).Content, Is.EqualTo("-4"));
        }

        [Test, Sequential]
        public void CheckDetectingNegativeIntegers3([Values("*", "/", "+", "-", "+=", "-=", "/=", "*=")] string mathOp)
        {
            var lexer = new Lexer();
            lexer.Load($"{mathOp} -4");

            var rootNode = new Parser(lexer).Parse();

            var numberToken = rootNode.TheTree.Single(o => o.Token is IntToken).Token;
            Assert.That(((IntToken)numberToken).Content, Is.EqualTo("-4"));
        }

        [Test]
        public void CheckDetectingNegativeIntegers4()
        {
            var lexer = new Lexer();
            lexer.Load("int f; int g = (f) - 4;");

            var rootNode = new Parser(lexer).Parse();

            var numberToken = rootNode.TheTree.Single(o => o.Token is IntToken).Token;
            Assert.That(((IntToken)numberToken).Content, Is.EqualTo("4"));
        }

        [Test, Sequential]
        public void CheckDetectingFloats([Values("0.", "1.", ".1", "1.1", "1e3", "1e-2", "-12.3", "1.1e4", "00.0")] string code)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(code), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            Assert.That(parser.RootNode.Children.Single(), Is.TypeOf<GenericSyntaxNode>());
            Assert.That(parser.RootNode.Children.Single().Token, Is.TypeOf<FloatToken>());
        }

        [Test]
        public void CheckDetectingNegativeFloat()
        {
            var lexer = new Lexer();
            lexer.Load("-4.1");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.Children.Single().Token, Is.TypeOf<FloatToken>());
            Assert.That(((FloatToken)rootNode.Children.Single().Token).Content, Is.EqualTo("-4.1"));
        }

        [Test]
        public void CheckDetectingNegativeFloatsInBrackets()
        {
            var lexer = new Lexer();
            lexer.Load("(-4.2)");

            var rootNode = new Parser(lexer).Parse();

            var numberToken = rootNode.TheTree.Single(o => o.Token is FloatToken).Token;
            Assert.That(((FloatToken)numberToken).Content, Is.EqualTo("-4.2"));
        }

        [Test, Sequential]
        public void CheckDetectingNegativeFloatWithPrecedingSymbol([Values("*", "/", "+", "-")] string mathOp)
        {
            var lexer = new Lexer();
            lexer.Load($"{mathOp} -4.3");

            var rootNode = new Parser(lexer).Parse();

            var numberToken = rootNode.TheTree.Single(o => o.Token is FloatToken).Token;
            Assert.That(((FloatToken)numberToken).Content, Is.EqualTo("-4.3"));
        }
        
        [Test, Sequential]
        public void CheckDetectingMathSymbols([Values("+=", "-=", "/=", "*=", ">>=", "<<=", "|=")] string mathOp)
        {
            var code = $"int i = 1;\ni {mathOp} -1;";

            var lexer = new Lexer();
            lexer.Load(code);

            var rootNode = new Parser(lexer).Parse();
            Assert.That(rootNode.ToCode(), Is.EqualTo(code));
        }

        [Test]
        public void CheckDetectingNegativeFloat4()
        {
            var lexer = new Lexer();
            lexer.Load("int f; int g = (f) - 4.4;");

            var rootNode = new Parser(lexer).Parse();

            var numberToken = rootNode.TheTree.Single(o => o.Token is FloatToken).Token;
            Assert.That(((FloatToken)numberToken).Content, Is.EqualTo("4.4"));
        }

        [Test]
        public void CheckDetectingVectorComponents()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("vec3 v; v.x = 12.3;"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes, Has.Count.EqualTo(2));
            Assert.That(nodes[0], Is.TypeOf<VariableDeclarationSyntaxNode>());
            Assert.That(nodes[1], Is.TypeOf<VariableAssignmentSyntaxNode>());
        }

        [Test, Sequential]
        public void CheckDetectingIfWithSingleBranch([Values("if (true) { break; }", "if (true) break;", "if (true) // comment\n break;", "if (true) // comment\n { break; }")] string code)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(code), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes[0], Is.TypeOf<IfSyntaxNode>());

            var ifSyntaxNode = (IfSyntaxNode)nodes[0];
            Assert.That(ifSyntaxNode.Conditions, Is.Not.Null);
            Assert.That(ifSyntaxNode.TrueBranch, Is.Not.Null);
            Assert.That(ifSyntaxNode.TrueBranch.Children, Has.Count.EqualTo(2)); // 'break' and ';'
            Assert.That(ifSyntaxNode.FalseBranch, Is.Null);
        }

        [Test, Sequential]
        public void CheckDetectingIfWithDoubleBranches([Values("if (true) { break; } else { return; }", "if (true) break; else { return; }", "if (true) { break; } else return;", "if (true) break; else return;", "if (true) // an if\nbreak; else return;")] string code)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(code), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes[0], Is.TypeOf<IfSyntaxNode>());

            var ifSyntaxNode = (IfSyntaxNode)nodes[0];
            Assert.That(ifSyntaxNode.Conditions, Is.Not.Null);
            Assert.That(ifSyntaxNode.TrueBranch, Is.Not.Null);
            Assert.That(ifSyntaxNode.FalseBranch, Is.Not.Null);
        }

        [Test]
        public void CheckDetectingNestedIfs()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("if (1) { return 1; } else if (2) return 2; else return 3;"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes.Single().ToCode().ToSimple(), Is.EqualTo("if (1) return 1; else if (2) return 2; else return 3;"));
        }

        [Test]
        public void CheckDetectingIfsWithCommaSeparatedStatements()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("if (1) break,continue;"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var nodes = parser.RootNode.Children;
            Assert.That(nodes.Single().ToCode().ToSimple(), Is.EqualTo("if (1) { break; continue; }"));
        }

        [Test, Sequential]
        public void CheckDetectingForStatement([Values("for (i = 0; i < 1; i++) { break; }", "for (i = 0; i < 1; i++) // comment\n{ break; }")] string code)
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load(code), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var forSyntaxNode = (ForSyntaxNode)parser.RootNode.Children.Single();
            Assert.That(forSyntaxNode.ToCode().ToSimple(), Is.EqualTo("for (i = 0; i < 1; i++) break;"));
            Assert.That(forSyntaxNode.LoopCode.Children, Has.Count.EqualTo(2));
            Assert.That(forSyntaxNode.LoopCode.Children[0].Token.Content, Is.EqualTo("break"));
            Assert.That(forSyntaxNode.LoopCode.Children[1].Token, Is.TypeOf<SemicolonToken>());
        }

        [Test]
        public void GivenVoidReturnCheckReturnSyntaxNodeCreated()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("void main() { return; }"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var functionNode = (FunctionDefinitionSyntaxNode)parser.RootNode.Children.Single();
            Assert.That(functionNode.Braces.Children, Has.Count.EqualTo(2));

            var returnNode = functionNode.Braces.Children[0];
            Assert.That(returnNode, Is.TypeOf<ReturnSyntaxNode>());
            Assert.That(functionNode.Braces.Children[1].Token, Is.TypeOf<SemicolonToken>());
            Assert.That(returnNode.Children, Is.Empty);
        }

        [Test]
        public void GivenNonVoidReturnCheckReturnSyntaxNodeCreated()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("int main() { return 1 + 2; }"), Is.True);

            var parser = new Parser(lexer);
            parser.Parse();

            var functionNode = (FunctionDefinitionSyntaxNode)parser.RootNode.Children.Single();
            Assert.That(functionNode.Braces.Children, Has.Count.EqualTo(2));

            var returnNode = functionNode.Braces.Children[0];
            Assert.That(returnNode, Is.TypeOf<ReturnSyntaxNode>());
            Assert.That(functionNode.Braces.Children[1].Token, Is.TypeOf<SemicolonToken>());
            Assert.That(returnNode.Children, Has.Count.EqualTo(3));
        }

        [Test]
        public void CheckParsingGlslVersion()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("#version 410 core"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            Assert.That(parser.RootNode.Children.Single(), Is.TypeOf<VerbatimLineSyntaxNode>());
            Assert.That(parser.RootNode.ToCode().ToSimple(), Is.EqualTo("#version 410 core"));
        }

        [Test]
        public void CheckParsingFloatPrecision()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("precision mediump float;"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            Assert.That(parser.RootNode.ToCode().ToSimple(), Is.EqualTo("precision mediump float;"));
        }

        [Test]
        public void CheckParsingPragmas()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("#pragma optionNV(fastmath off)"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            Assert.That(parser.RootNode.Children.Single(), Is.TypeOf<VerbatimLineSyntaxNode>());
            Assert.That(parser.RootNode.ToCode().ToSimple(), Is.EqualTo("#pragma optionNV(fastmath off)"));
        }

        [Test]
        public void CheckParsingUniforms()
        {
            var lexer = new Lexer();
            Assert.That(lexer.Load("uniform float fGlobalTime;\nuniform vec2 v2Resolution;"), Is.True);

            var parser = new Parser(lexer);
            Assert.That(() => parser.Parse(), Throws.Nothing);

            Assert.That(parser.RootNode.ToCode(), Is.EqualTo("uniform float fGlobalTime;\nuniform vec2 v2Resolution;"));
        }
    }
}