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