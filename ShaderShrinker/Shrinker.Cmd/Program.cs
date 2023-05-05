using System.Diagnostics;
using DiffEngine;
using Shrinker.Parser;
using TextCopy;

namespace Shrinker.Cmd
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var glsl = ClipboardService.GetText();
            if (string.IsNullOrEmpty(glsl))
            {
                Console.WriteLine("Error - Clipboard is empty.");
                return;
            }

            var lexer = new Lexer.Lexer();
            if (!lexer.Load(glsl))
            {
                Console.WriteLine("Error - Unable to process the GLSL.");
                return;
            }

            var parser = new Parser.Parser(lexer);
            try
            {
                Console.WriteLine("Parsing...");
                var rootNode = parser.Parse();

                Console.WriteLine("Simplifying...");
                var options = CustomOptions.All();
                options.CombineConsecutiveAssignments = false;
                options.RemoveComments = false;
                rootNode.Simplify(options);
                
                Console.WriteLine("Creating GLSL...");
                var newGlsl = rootNode.ToCode();
                ClipboardService.SetText(newGlsl);
                
                Console.WriteLine("Success.");

                var oldFile = CreateTempFileFromString(glsl, ".glsl");
                var newFile = CreateTempFileFromString(newGlsl, ".glsl");
                try
                {
                    Console.WriteLine("Launching diff process...");
                    var diffTool = new[] { DiffTool.DiffMerge, DiffTool.VisualStudioCode}.FirstOrDefault(o => DiffRunner.Launch(o, oldFile, newFile) != LaunchResult.NoDiffToolFound);
                    if (diffTool != default)
                    {
                        if (!OperatingSystem.IsWindows())
                        {
                            if (DiffTools.TryFindByName(diffTool, out var resolved))
                            {
                                var processName = Path.GetFileNameWithoutExtension(resolved.ExePath);

                                var attempts = 50;
                                while (!Process.GetProcesses().Any(o => o.ProcessName.StartsWith(processName, StringComparison.OrdinalIgnoreCase)) && --attempts > 0)
                                    Thread.Sleep(100);

                                if (attempts > 0)
                                {
                                    Console.WriteLine("  Waiting for process end...");
                                    while (Process.GetProcesses().Any(o => o.ProcessName.StartsWith(processName, StringComparison.OrdinalIgnoreCase)))
                                        Thread.Sleep(1000);
                                }
                            }
                            else
                            {
                                Console.WriteLine("  Unable to determine diff tool process name.");
                                return;
                            }
                        }

                        Console.WriteLine("  Complete.");
                    }
                    else
                    {
                        Console.WriteLine("  Failed to find a suitable tool.");
                    }
                }
                finally
                {
                    File.Delete(oldFile);
                    File.Delete(newFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error - {ex.Message}");
            }
        }

        private static string CreateTempFileFromString(string s, string fileExtension)
        {
            var fileName = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}{fileExtension}");
            File.WriteAllText(fileName, s);
            return fileName;
        }
    }
}