using System.Diagnostics;
using System.Reflection;
using System.Text;
using CommandLine;
using DiffEngine;
using Shrinker.Parser;
using TextCopy;

namespace Shrinker.Cmd
{
    // todo - add command line args.
    internal static class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<CmdOptions>(args).WithParsed(Shrink);
        }
        
        static void Shrink(CmdOptions args)
        {
            var transpileToCSharp = !string.IsNullOrEmpty(args.CSharpOutputPath);
            
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
                CustomOptions options;
                if (transpileToCSharp)
                {
                    options = CustomOptions.TranspileOptions();
                }
                else
                {
                    options = CustomOptions.All();
                    options.CombineConsecutiveAssignments = false;
                    options.RemoveComments = false;
                }
                
                rootNode.Simplify(options);                    
                
                Console.WriteLine($"Creating {(transpileToCSharp ? "CSharp" : "GLSL")}...");
                var newGlsl = rootNode.ToCode(options);
                
                Console.WriteLine("Success.");

                if (transpileToCSharp)
                {
                    const string ResourceName = "Shrinker.Cmd.Templates.GLSLProg.template";
                    using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);
                    if (stream == null)
                        throw new InvalidOperationException($"Missing C# resource template: {ResourceName}");

                    Console.WriteLine($"Writing {args.CSharpOutputPath}...");
                    using var reader = new StreamReader(stream);
                    var text = new StringBuilder(reader.ReadToEnd());
                    text.Replace("{code}", newGlsl);
                    File.WriteAllText(args.CSharpOutputPath, text.ToString());
                    return;
                }

                ClipboardService.SetText(newGlsl);
                if (!args.Diff)
                    return;
                
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