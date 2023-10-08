// -----------------------------------------------------------------------
//  <copyright file="ReadmeBuInstallerVersionUpdater.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class VersionUpdater
    {
        /// <summary>
        /// Update version numbers, using 'packageMe.sh' as the source.
        /// </summary>
        [Test]
        public void UpdateVersionNumbers()
        {
            var rootDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
            while (!rootDir.EnumerateFiles("*.md").Any())
                rootDir = rootDir.Parent;

            // Get app version.
            var assemblyInfoFile = rootDir.EnumerateFiles("ShaderShrinker/Shrinker.Avalonia/packageMe.sh").First();
            var assemblyVersionLine = File.ReadAllLines(assemblyInfoFile.FullName).First(o => o.StartsWith("APP_VERSION="));
            var assemblyVersion = assemblyVersionLine.Substring(13).Replace(".0\"", null);
            
            // Update project.
            var csproj = rootDir.EnumerateFiles("ShaderShrinker/Shrinker.Avalonia/Shrinker.Avalonia.csproj").First();
            var csprojLines = File.ReadAllLines(csproj.FullName);
            for (var i = 0; i < csprojLines.Length; i++)
            {
                if (csprojLines[i].TrimStart().StartsWith("<AssemblyVersion>"))
                    csprojLines[i] = $"        <AssemblyVersion>{assemblyVersion}.0.0</AssemblyVersion>";
                if (csprojLines[i].TrimStart().StartsWith("<FileVersion>"))
                    csprojLines[i] = $"        <FileVersion>{assemblyVersion}.0.0</FileVersion>";
            }
            
            File.WriteAllLines(csproj.FullName, csprojLines);
            
            // Open installer .iss.
            var issFile = rootDir.EnumerateFiles("InstallScript.iss", SearchOption.AllDirectories).FirstOrDefault();
            Assert.That(issFile, Is.Not.Null);
            var issLines = File.ReadAllLines(issFile.FullName);

            for (var i = 0; i < issLines.Length; i++)
            {
                if (issLines[i].StartsWith("AppVersion="))
                    issLines[i] = $"AppVersion={assemblyVersion}";
            }

            File.WriteAllLines(issFile.FullName, issLines);
        }
    }
}