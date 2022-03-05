// -----------------------------------------------------------------------
//  <copyright file="ProjectTests.cs">
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

namespace UnitTests
{
    [TestFixture]
    public class ProjectTests : UnitTestBase
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

        private static IEnumerable<FileInfo> SourceFiles => SolutionDir.EnumerateFiles("*.cs", SearchOption.AllDirectories).Union(SolutionDir.EnumerateFiles("*.xaml", SearchOption.AllDirectories));

        [Test, Sequential]
        public void CheckSimplifyingReturnsExpectedContent([ValueSource(nameof(SourceFiles))] FileInfo sourceFile)
        {
            if (sourceFile.Name.Contains("ProjectTests"))
                Assert.Pass();
            Assert.That(File.ReadAllLines(sourceFile.FullName), Is.All.Not.Contains(" Graphics"), $"{sourceFile}:1");
        }
    }
}