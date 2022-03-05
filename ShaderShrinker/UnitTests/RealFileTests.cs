// -----------------------------------------------------------------------
//  <copyright file="RealFileTests.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Shrinker.Lexer;
using Shrinker.Parser;

namespace UnitTests
{
    [TestFixture]
    public class RealFileTests : UnitTestBase
    {
        private static DirectoryInfo SolutionDir
        {
            get
            {
                var d = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
                while (d != null && !d.EnumerateFiles("*.sln").Any())
                    d = d.Parent;
                return d;
            }
        }

        private static IEnumerable<FileInfo> TestFiles => SolutionDir.EnumerateFiles("UnitTests/TestFiles/*.glsl");

        [Test, Sequential]
        public void CheckSimplifyingReturnsExpectedContent([ValueSource(nameof(TestFiles))] FileInfo testFile)
        {
            var referenceFile = $"{testFile.Directory.FullName}/SimplifiedReference/{testFile.Name}";
            var referenceCode = File.ReadAllText(referenceFile);

            var lexer = new Lexer();
            lexer.Load(testFile);

            var rootNode = new Parser(lexer).Parse();
            var originalCount = File.ReadAllText(testFile.FullName).GetCodeCharCount();
            rootNode = rootNode.Simplify();

            var simplifiedCode = rootNode.ToCode();
            var simpleCount = simplifiedCode.GetCodeCharCount();

            simplifiedCode = simplifiedCode.WithAppMessage(originalCount, simpleCount);
            File.WriteAllText(referenceFile, simplifiedCode);

            Assert.That(simplifiedCode.ToSimple(), Is.EqualTo(referenceCode.ToSimple()));
        }
   }
}