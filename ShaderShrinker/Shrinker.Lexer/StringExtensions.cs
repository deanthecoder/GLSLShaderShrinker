// -----------------------------------------------------------------------
//  <copyright file="StringExtensions.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

// ReSharper disable StringIndexOfIsCultureSpecific.1

using System.Linq;

namespace Shrinker.Lexer
{
    public static class StringExtensions
    {
        public static int GetCodeCharCount(this string glsl)
        {
            // Remove comment blocks.
            int startComment;
            while ((startComment = glsl.IndexOf("/*")) >= 0)
            {
                var endComment = glsl.IndexOf("*/");
                if (endComment > startComment)
                    glsl = glsl.Remove(startComment, endComment - startComment + 2);
            }

            return glsl.Split('\r', '\n').Sum(o => o.GetCodeCharCountForLine());
        }

        private static int GetCodeCharCountForLine(this string line)
        {
            var commentIndex = line.IndexOf("//");
            if (commentIndex >= 0)
                line = line.Substring(0, commentIndex);
            commentIndex = line.IndexOf("/*");
            if (commentIndex >= 0)
                line = line.Substring(0, commentIndex);

            var isEmpty = string.IsNullOrEmpty(line.Trim('\r', '\n', ' '));

            int l;
            do
            {
                l = line.Length;
                line = line.Replace(" ", null);
                line = line.Replace("\r", null);
                line = line.Replace("\n", null);
            } while (line.Length != l);

            return line.Trim().Length + (isEmpty ? 0 : 1);
        }

        public static bool IsNewline(this char ch) => ch == '\n' || ch == '\r';

        public static string AllowBraceRemoval(this string s)
        {
            s = s.Trim();
            if (s.StartsWith("{") && s.EndsWith("}") && s.Count(ch => ch == ';') == 1)
                return s.Trim('{', '}', ' ');
            return s;
        }

        public static bool StartsWithVarName(this string s, string varName) =>
            s != null && (s == varName || s.StartsWith($"{varName}."));

        public static string ToSimple(this string s)
        {
            s = s.Replace('\n', ' ').Replace('\t', ' ');
            while (s.Contains("  "))
                s = s.Replace("  ", " ");
            return s.Trim();
        }

        public static string WithAppMessage(this string glsl)
        {
            const string Message = "Processed with 'GLSL Shader Shrinker'";
            return glsl.Contains(Message) ? glsl : $"// {Message}\n{glsl}";
        }
    }
}