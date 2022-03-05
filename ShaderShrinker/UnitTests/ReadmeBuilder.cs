// -----------------------------------------------------------------------
//  <copyright file="ReadmeBuilder.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
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

            var tocIndex = readmeLines.Count;
            var toc = new List<string>();

            // Find options XAML.
            var optionsXamlFile = rootDir.EnumerateFiles("OptionsDialog.xaml", SearchOption.AllDirectories).FirstOrDefault();
            Assert.That(optionsXamlFile, Is.Not.Null);

            // Read all the Markdown tooltip content.
            var inTooltip = false;
            var indent = 0;
            foreach (var xamlLine in File.ReadAllLines(optionsXamlFile.FullName))
            {
                if (xamlLine.Contains("<MdXaml:MarkdownScrollViewer"))
                {
                    inTooltip = true;
                    indent = xamlLine.IndexOf('<') + 4;
                }
                else if (xamlLine.Contains("</MdXaml:MarkdownScrollViewer"))
                {
                    inTooltip = false;
                    readmeLines.Add("\n---");
                    continue;
                }

                if (inTooltip && !xamlLine.TrimStart().StartsWith("<"))
                {
                    var mdLine = xamlLine
                        .Substring(Math.Min(xamlLine.Length, indent))
                        .TrimEnd()
                        .Replace("&lt;", "<");

                    if (xamlLine.TrimStart().StartsWith("### "))
                    {
                        // Build 'features' TOC.
                        var heading = xamlLine.Trim(' ', '#');
                        toc.Add($"* [{heading}](#{ToHeaderLink(heading)})");

                        mdLine = mdLine.Replace("### ", "## "); // Elevate the header level one-above what we use in the XAML.
                    }

                    readmeLines.Add(mdLine);
                }
            }

            readmeLines.InsertRange(tocIndex, toc);
            File.WriteAllLines(readmeFile.FullName, readmeLines);
        }

        private static string ToHeaderLink(string s)
        {
            s = s.Replace(' ', '-');
            return s.ToLower();
        }
    }
}