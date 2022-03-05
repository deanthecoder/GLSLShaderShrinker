// -----------------------------------------------------------------------
//  <copyright file="StringExtensions.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

// ReSharper disable StringIndexOfIsCultureSpecific.1

using System;
using System.Linq;
using System.Text;

namespace Shrinker.Lexer
{
    public static class StringExtensions
    {
        public static int GetCodeCharCount(this string glsl)
        {
            // Remove C++ line comments.
            var sb = new StringBuilder();
            foreach (var line in glsl.Split('\r', '\n'))
            {
                var i = 0;
                while ((i = line.IndexOf("//", i, StringComparison.Ordinal)) > 0 && line[i - 1] == '*')
                    i++; // Ignore '*//'

                sb.AppendLine((i >= 0 ? line.Substring(0, i) : line).Trim());
            }

            glsl = sb.ToString();

            // Remove C comment blocks.
            int startComment;
            while ((startComment = glsl.IndexOf("/*")) >= 0)
            {
                var endComment = glsl.IndexOf("*/", startComment, StringComparison.Ordinal);
                if (endComment > startComment)
                    glsl = glsl.Remove(startComment, endComment - startComment + 2);
            }

            return glsl.Split('\r', '\n').Sum(o => o.GetCodeCharCountForLine());
        }

        private static int GetCodeCharCountForLine(this string line)
        {
            return line.Count(ch => !char.IsWhiteSpace(ch));
            //var commentIndex = line.IndexOf("//");
            //if (commentIndex >= 0)
            //    line = line.Substring(0, commentIndex);
            //commentIndex = line.IndexOf("/*");
            //if (commentIndex >= 0)
            //    line = line.Substring(0, commentIndex);

            //var sb = new StringBuilder(line.Trim());
            //int l;
            //do
            //{
            //    l = sb.Length;
            //    sb.Replace(" ", null)
            //      .Replace("\t", null)
            //      .Replace("\r", null)
            //      .Replace("\n", null);
            //} while (sb.Length != l);

            //return sb.Length;
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

        public static string WithAppMessage(this string glsl, int originalSize, int optimizedSize)
        {
            const string MessageBase = "Processed by 'GLSL Shader Shrinker'";
            if (glsl.Contains(MessageBase))
                return glsl;

            var message = new StringBuilder($"// {MessageBase}");
            var reduction = originalSize - optimizedSize;
            if (reduction > 0)
                message.Append($" (Shrunk by {reduction:N0} characters)");

            message.Append("\n// (https://github.com/deanthecoder/GLSLShaderShrinker)\n\n");
            message.Append(glsl);
            return message.ToString();
        }

        public static string RemoveAllWhitespace(this string s) => new string(s.Where(ch => !char.IsWhiteSpace(ch)).ToArray());

        public static bool IsAnyOf(this string s, params string[] choices) => choices?.Contains(s) == true;
    }
}