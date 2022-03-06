// -----------------------------------------------------------------------
//  <copyright file="GolfExtensions.cs">
//      Copyright (c) 2022 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class GolfExtensions
    {
        /// <summary>
        /// Replace named objects (functions/variables) with short 'golfed' versions.
        /// </summary>
        public static void GolfNames(this SyntaxNode rootNode)
        {
            var nameMap = BuildGolfRenameMap(rootNode).Where(o => o.Key != o.Value).ToDictionary(o => o.Key, o => o.Value);

            foreach (var node in rootNode.TheTree.OfType<IRenamable>().Where(o => nameMap.ContainsKey(NameWithoutDotSuffix(o.Name))).ToList())
            {
                if (!((SyntaxNode)node).HasAncestor<StructDefinitionSyntaxNode>())
                    node.Rename(nameMap[NameWithoutDotSuffix(node.Name)]);
            }
        }

        /// <summary>
        /// Add #defines for commonly-used terms (E.g. smoothstep, et).
        /// </summary>
        public static void GolfDefineCommonTerms(this SyntaxNode rootNode)
        {
            var userDefinedNames = rootNode.FindUserDefinedNames();
            var defineMap = new Dictionary<string, string[]>
            {
                { "float", new[] { "F", "f", "_f" } },
                { "abs", new[] { "A" } },
                { "acos", new[] { "AC" } },
                { "acosh", new[] { "ACH" } },
                { "asin", new[] { "AS" } },
                { "asinh", new[] { "ASH" } },
                { "atan", new[] { "AT" } },
                { "atanh", new[] { "ATH" } },
                { "BitsToInt", new[] { "B2I" } },
                { "BitsToUint", new[] { "B2U" } },
                { "ceil", new[] { "CL" } },
                { "clamp", new[] { "C" } },
                { "cos", new[] { "CS" } },
                { "cosh", new[] { "CH" } },
                { "cross", new[] { "X" } },
                { "degrees", new[] { "DG" } },
                { "determinant", new[] { "DT" } },
                { "dFdx", new[] { "DX" } },
                { "dFdy", new[] { "DY" } },
                { "distance", new[] { "DST" } },
                { "dot", new[] { "D" } },
                { "faceforward", new[] { "FF" } },
                { "floor", new[] { "FL" } },
                { "fract", new[] { "FC" } },
                { "fwidth", new[] { "FW" } },
                { "greaterThan", new[] { "GT" } },
                { "greaterThanEqual", new[] { "GTE" } },
                { "intBitsTo", new[] { "IB2" } },
                { "inverse", new[] { "I" } },
                { "inversesqrt", new[] { "IS" } },
                { "length", new[] { "L" } },
                { "lessThan", new[] { "LT" } },
                { "lessThanEqual", new[] { "LTE" } },
                { "matrixCompMult", new[] { "MCM" } },
                { "max", new[] { "MX" } },
                { "min", new[] { "MN" } },
                { "modf", new[] { "MF" } },
                { "normalize", new[] { "N", "U", "NM" } },
                { "notEqual", new[] { "NE" } },
                { "radians", new[] { "R" } },
                { "reflect", new[] { "RL" } },
                { "refract", new[] { "RR" } },
                { "round", new[] { "RND" } },
                { "sin", new[] { "SN" } },
                { "sinh", new[] { "SNH" } },
                { "smoothstep", new[] { "S", "SS" } },
                { "sqrt", new[] { "SQ" } },
                { "tan", new[] { "TN" } },
                { "tanh", new[] { "TH" } },
                { "texelFetch", new[] { "TF" } },
                { "texelFetchOffset", new[] { "TFO" } },
                { "texture", new[] { "T" } },
                { "textureGrad", new[] { "TG" } },
                { "textureGradOffset", new[] { "TGO" } },
                { "textureLod", new[] { "TL" } },
                { "textureLodOffset", new[] { "TLO" } },
                { "textureProj", new[] { "TP" } },
                { "textureProjGrad", new[] { "TPG" } },
                { "textureProjLod", new[] { "TPL" } },
                { "textureProjLodOffset", new[] { "TPLO" } },
                { "textureSize", new[] { "TS" } },
                { "transpose", new[] { "TNS" } },
                { "trunc", new[] { "TC" } },
                { "uintBitsTo", new[] { "UB2" } }
            };

            foreach (var n in new[] { 2, 3, 4 })
            {
                defineMap.Add($"ivec{n}", new[] { $"iv{n}", $"IV{n}" });
                defineMap.Add($"vec{n}", new[] { $"v{n}", $"V{n}", $"_v{n}" });
                defineMap.Add($"mat{n}", new[] { $"m{n}", $"M{n}", $"_m{n}" });
            }

            // Check for duplicates.
            var allKeys = defineMap.SelectMany(o => o.Value).ToList();
            var duplicates = allKeys.Where(o => allKeys.Count(s => s == o) > 1).ToList();
            if (duplicates.Any())
                throw new InvalidOperationException("Duplicate substitutes detected: " + duplicates.Aggregate((a, b) => $"{a}, {b}"));

            var keywordNodes = 
                rootNode.TheTree
                    .Where(o => o is IRenamable or VariableDeclarationSyntaxNode && defineMap.ContainsKey(GetSupportedBuiltinName(o)))
                    .ToList();
            foreach (var keyword in defineMap.Keys)
            {
                // How many occurrences are in the code?
                var nodes = keywordNodes.Where(o => GetSupportedBuiltinName(o) == keyword).ToList();
                if (nodes.Count <= 1)
                    continue; // Not worth it.

                // What would the substitute be?
                var replacement = defineMap[keyword].FirstOrDefault(o => !userDefinedNames.Contains(o));
                if (replacement == null)
                    continue; // Replacement term is already in use in the code. Oh well - we tried...

                // What would the saving be, should we #define it?
                var toAdd = $"#define {replacement} {keyword} ".Length + nodes.Count * replacement.Length;
                var toRemove = nodes.Count * keyword.Length;
                if (toAdd >= toRemove)
                    continue; // Could replace with #define, but not worth it.

                // We'll get a space saving - Replace the nodes...
                foreach (var node in nodes)
                {
                    if (node is IRenamable r)
                        r.Rename(replacement);
                    else
                        ((VariableDeclarationSyntaxNode)node).RenameType(replacement);
                }

                // ...and add a #define.
                var existingDefine = rootNode.Children.OfType<PragmaDefineSyntaxNode>().FirstOrDefault();
                var insertBefore = existingDefine ?? rootNode.Children.First();
                insertBefore.Parent.InsertChild(insertBefore.NodeIndex, new PragmaDefineSyntaxNode(replacement, null, new SyntaxNode[] { new GenericSyntaxNode(keyword) }));
            }
        }

        private static string GetSupportedBuiltinName(SyntaxNode node)
        {
            if (node is IRenamable n)
                return n.Name;
            if (node is VariableDeclarationSyntaxNode decl)
            {
                var name = decl.VariableType.Content;
                if (name.StartsWith("const"))
                    name = name.Substring(5).TrimStart();
                return name;
            }

            return null;
        }

        private static string NameWithoutDotSuffix(string name) => name.Split('.').First();

        private static Dictionary<string, string> BuildGolfRenameMap(SyntaxNode rootNode)
        {
            var originalNames = rootNode.FindUserDefinedNames();
            var nameMap = originalNames.ToDictionary(o => o, o => o);

            foreach (var kvp in nameMap.Where(o => o.Value.Length > 1))
            {
                // Try to abbreviate to one letter.
                var candidate = $"{kvp.Value.First()}";

                const string Letters = "abcdefghijlkmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                if (nameMap.ContainsValue(candidate))
                {
                    // Name is used - Assign unused single-letter.
                    var ch = Letters.FirstOrDefault(o => !nameMap.ContainsValue($"{o}"));
                    if (ch != 0)
                        candidate = $"{ch}";
                }

                if (nameMap.ContainsValue(candidate))
                {
                    // Name is used - Concatenate first and last characters.
                    candidate = $"{kvp.Value.First()}{kvp.Value.Last()}";
                }

                if (nameMap.ContainsValue(candidate))
                {
                    // Name is used - Try upper case.
                    candidate = candidate.ToUpper();
                }

                if (nameMap.ContainsValue(candidate))
                {
                    // Name is used - Try lower case.
                    candidate = candidate.ToLower();
                }

                var n = 0;
                while (nameMap.ContainsValue(candidate))
                {
                    // Name is used - Try letter and number.
                    foreach (var ch in Letters)
                    {
                        candidate = $"{ch}{n}";
                        if (!nameMap.ContainsValue(candidate))
                            break;
                    }

                    n++;
                }

                // Update the map.
                nameMap[kvp.Key] = $"{candidate}";
            }

            return nameMap;
        }
    }
}