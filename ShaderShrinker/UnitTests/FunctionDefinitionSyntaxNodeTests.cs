// -----------------------------------------------------------------------
//  <copyright file="FunctionDefinitionSyntaxNodeTests.cs">
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

namespace UnitTests
{
    [TestFixture]
    public class FunctionDefinitionSyntaxNodeTests : UnitTestBase
    {
        [Test]
        public void GivenFunctionWithNoVariablesCheckGettingLocalVariables()
        {
            var lexer = new Lexer();
            lexer.Load("void main() { return; }");
            var functionNode = new Parser(lexer).Parse().FindFunctionDefinitions().Single();

            Assert.That(functionNode.LocalVariables().Select(o => o.Name), Is.Empty);
        }

        [Test]
        public void GivenFunctionUsingGlobalVariableCheckGettingLocalVariables()
        {
            var lexer = new Lexer();
            lexer.Load("int a = 1; void main() { a = 2; }");
            var functionNode = new Parser(lexer).Parse().FindFunctionDefinitions().Single();

            Assert.That(functionNode.LocalVariables().Select(o => o.Name), Is.Empty);
        }

        [Test]
        public void GivenFunctionWithUninitializedLocalVariablesCheckGettingLocalVariables()
        {
            var lexer = new Lexer();
            lexer.Load("void main() { float one; int two; }");
            var functionNode = new Parser(lexer).Parse().FindFunctionDefinitions().Single();

            Assert.That(functionNode.LocalVariables().Select(o => o.Name), Is.EqualTo(new[] { "one", "two" }));
        }

        [Test]
        public void GivenFunctionWithInitializedLocalVariablesCheckGettingLocalVariables()
        {
            var lexer = new Lexer();
            lexer.Load("void main() { int a = 1, b, c; b = 2; }");
            var functionNode = new Parser(lexer).Parse().FindFunctionDefinitions().Single();

            Assert.That(functionNode.LocalVariables().Select(o => o.Name), Is.EqualTo(new[] { "a", "b", "c" }));
        }

        [Test]
        public void GivenFunctionWithParamsCheckGettingLocalVariablesDoesNotIncludeThem()
        {
            var lexer = new Lexer();
            lexer.Load("void main(int a) { }");
            var functionNode = new Parser(lexer).Parse().FindFunctionDefinitions().Single();

            Assert.That(functionNode.LocalVariables().Select(o => o.Name), Is.Empty);
        }
    }
}