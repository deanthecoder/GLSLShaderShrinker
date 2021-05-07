// -----------------------------------------------------------------------
//  <copyright file="Program.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Windows;
using Shrinker.Lexer;
using Shrinker.Parser;

namespace ShaderShrinkerCmd
{
    public static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var usingClipboard = false;
            int originalLength;

            var lexer = new Lexer();
            if (args.Length == 0)
            {
                if (Clipboard.ContainsText())
                {
                    if (!lexer.Load(Clipboard.GetText()))
                    {
                        Console.WriteLine("Error parsing GLSL clipboard data.");
                        return;
                    }

                    usingClipboard = true;
                    originalLength = Clipboard.GetText().GetCodeCharCount();
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (!lexer.Load(new FileInfo(args[0])))
                {
                    Console.WriteLine("Error parsing GLSL data from disk.");
                    return;
                }

                originalLength = File.ReadAllText(args[0]).GetCodeCharCount();
            }

            if (!lexer.Tokens.Any())
            {
                ShowUsage();
                return;
            }

            var newCode = new Parser(lexer).Parse().Simplify().ToCode();
            if (usingClipboard)
                Clipboard.SetText(newCode);
            else
                File.WriteAllText(args[1], newCode);

            var newLength = newCode.GetCodeCharCount();
            Console.WriteLine($"Shrunk {originalLength - newLength} characters, from {originalLength} ({BytesToKb(originalLength):F1}KB) to {newLength} ({BytesToKb(newLength):F1}KB)");
        }

        private static double BytesToKb(int bytes) => bytes / 1024.0;

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  ShaderShrinkerCmd.exe <input.txt> <output.txt>");
            Console.WriteLine("or");
            Console.WriteLine("  ShaderShrinkerCmd.exe");
            Console.WriteLine("    ...to read/write GLSL text from the clipboard.");
        }
    }
}