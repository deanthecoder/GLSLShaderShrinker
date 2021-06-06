// -----------------------------------------------------------------------
//  <copyright file="ReadmeBuilder.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2021 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class ReadmeBuilder
    {
        [Test]
        public void BuildTheReadme()
        {
            var rootDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
            while (!rootDir.EnumerateFiles("*.md").Any())
                rootDir = rootDir.Parent;

            // Open Readme.md
            var readmeFile = rootDir.EnumerateFiles("readme.md", SearchOption.TopDirectoryOnly).FirstOrDefault();
            Assert.That(readmeFile, Is.Not.Null);
            var readmeLines = File.ReadAllLines(readmeFile.FullName).TakeWhile(o => !o.StartsWith("# Features")).ToList();
            readmeLines.Add("# Features");

            // Find options XAML.
            var optionsXamlFile = rootDir.EnumerateFiles("OptionsDialog.xaml", SearchOption.AllDirectories).FirstOrDefault();
            Assert.That(optionsXamlFile, Is.Not.Null);

            // Read all the Markdown tooltip content.
            var inTooltip = false;
            foreach (var xamlLine in File.ReadAllLines(optionsXamlFile.FullName))
            {
                if (xamlLine.Contains("<MdXaml:MarkdownScrollViewer"))
                {
                    inTooltip = true;
                }
                else if (xamlLine.Contains("</MdXaml:MarkdownScrollViewer"))
                {
                    inTooltip = false;
                    readmeLines.Add("\n---");
                    continue;
                }

                if (inTooltip && !xamlLine.TrimStart().StartsWith("<"))
                    readmeLines.Add(xamlLine.Trim());
            }

            File.WriteAllLines(readmeFile.FullName, readmeLines);
        }
    }
}