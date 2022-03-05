// -----------------------------------------------------------------------
//  <copyright file="COutputFormatter.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using System.Text;
using Shrinker.Lexer;

namespace Shrinker.Parser
{
    public static class COutputFormatter
    {
        public static string ToCCode(this string glsl, int originalSize, int optimizedSize)
        {
            var output = new StringBuilder();
            output.Append(string.Empty.WithAppMessage(originalSize, optimizedSize));
            output.AppendLine("static const char* fragmentShader =");

            foreach (var line in glsl.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    output.AppendLine();
                    continue;
                }

                var whitespace = new string(line.TakeWhile(char.IsWhiteSpace).ToArray());
                output.Append(whitespace);
                output.Append('\"');
                var s = line.Substring(whitespace.Length);

                var leaveAlone = s.StartsWith("#") || s.StartsWith("//") || s.StartsWith("/*");

                if (!leaveAlone)
                {
                    while (s.Contains('\t'))
                        output.Replace('\t', ' ');
                    while (s.Contains("  "))
                        output.Replace("  ", " ");
                    foreach (var toSquash in new[] { "!", "#", "=", "%", "^", "&", "*", "(", ")", "-", "+", "=", ";", "{", "[", "]", "}", ":", "~", ",", "<", ">", ".", "\\", "/", "|", "?" })
                    {
                        foreach (var toFind in new[] { $"{toSquash} ", $" {toSquash}"})
                        {
                            while (s.Contains(toFind))
                                s = s.Replace(toFind, toSquash);
                        }
                    }
                }

                output.Append(s);

                if (leaveAlone)
                    output.Append("\\n");
                output.AppendLine("\"");
            }

            output.AppendLine(";");
            return output.ToString();
        }
    }
}