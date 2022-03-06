// -----------------------------------------------------------------------
//  <copyright file="PresetsUpdater.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
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
using Newtonsoft.Json;
using NUnit.Framework;
using Shrinker.Parser;

namespace UnitTests
{
    [TestFixture]
    public class PresetsUpdater
    {
        [Test]
        public void UpdateInstallerVersion()
        {
            var rootDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
            while (!rootDir.EnumerateFiles("*.md").Any())
                rootDir = rootDir.Parent;

            var presetDir = rootDir.EnumerateDirectories("ShaderShrinker/Shrinker.WpfApp/Presets").Single();
            presetDir.EnumerateFiles().ToList().ForEach(o => o.Delete());

            File.WriteAllText(Path.Combine(presetDir.FullName, "Maximum"), OptionsAsString(CustomOptions.All()));
            File.WriteAllText(Path.Combine(presetDir.FullName, "Minimum (Reformat)"), OptionsAsString(CustomOptions.None()));
            File.WriteAllText(Path.Combine(presetDir.FullName, "Remove Dead Code"), OptionsAsString(RemoveSurplus()));

            var golfOptions = CustomOptions.SetAllOptions(true);
            golfOptions.KeepHeaderComments = false;
            File.WriteAllText(Path.Combine(presetDir.FullName, "Maximum (Golfed)"), OptionsAsString(golfOptions));
        }

        private static string OptionsAsString(CustomOptions options) =>
            JsonConvert.SerializeObject(options, Formatting.Indented);

        private static CustomOptions RemoveSurplus()
        {
            var options = CustomOptions.None();
            options.RemoveDisabledCode = true;
            options.RemoveUnreachableCode = true;
            options.RemoveUnusedFunctions = true;
            options.RemoveUnusedVariables = true;
            options.RemoveDisabledCode = true;
            return options;
        }
    }
}