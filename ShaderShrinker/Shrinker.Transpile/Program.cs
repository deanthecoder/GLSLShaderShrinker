using System.Reflection;
using System.Text;
using CommandLine;
using Shrinker.Parser;
using TextCopy;

namespace Shrinker.Transpile
{
    // todo - add command line args.
    internal static class Program
    {
        private static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<CmdOptions>(args).WithParsed(Shrink);
        }

        private static void Shrink(CmdOptions args)
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
                var options = CustomOptions.TranspileOptions();
                rootNode.Simplify(options);
                
                Console.WriteLine("Creating CSharp...");
                var newGlsl = rootNode.ToCode(options);

                const string ResourceName = "Shrinker.Transpile.Templates.GLSLProg.template";
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);
                if (stream == null)
                    throw new InvalidOperationException($"Missing C# resource template: {ResourceName}");

                Console.WriteLine($"Writing {args.CSharpOutputPath}...");
                using var reader = new StreamReader(stream);
                var text = new StringBuilder(reader.ReadToEnd());
                text.Replace("{code}", newGlsl);
                File.WriteAllText(args.CSharpOutputPath, text.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error - {ex.Message}");
            }
        }
    }
}