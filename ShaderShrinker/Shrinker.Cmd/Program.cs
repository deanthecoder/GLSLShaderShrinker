using System.Diagnostics;
using DiffEngine;
using Shrinker.Parser;
using TextCopy;

namespace Shrinker.Cmd
{
    internal static class Program
    {
        static void Main(string[] args)
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
                    if (DiffRunner.Launch(DiffTool.DiffMerge, oldFile, newFile) != LaunchResult.NoDiffToolFound)
                    {
                        var attempts = 50;
                        while (!Process.GetProcesses().Any(o => o.ProcessName.StartsWith("diffmerge", StringComparison.OrdinalIgnoreCase)) && attempts-- > 0)
                            Thread.Sleep(100);

                        if (attempts == 0)
                        {
                            Console.WriteLine("  Time-out.");
                            return;
                        }

                        Console.WriteLine("  Waiting for process end...");
                        while (Process.GetProcesses().Any(o => o.ProcessName.StartsWith("diffmerge", StringComparison.OrdinalIgnoreCase)))
                            Thread.Sleep(1000);

                        Console.WriteLine("  Complete.");
                    }
                    else
                    {
                        Console.WriteLine("  Failed.");
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