// -----------------------------------------------------------------------
//  <copyright file="FileSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shrinker.Lexer;

namespace Shrinker.Parser
{
    public class FileSyntaxNode : SyntaxNode
    {
        public override string UiName => "<ROOT>";

        private FileSyntaxNode(IEnumerable<GenericSyntaxNode> flatSyntaxNodes)
        {
            foreach (var syntaxNode in flatSyntaxNodes)
                AddChild(syntaxNode);
        }

        public static FileSyntaxNode Create(IEnumerable<IToken> tokens)
        {
            var node = new FileSyntaxNode(tokens.Where(o => o is not WhitespaceToken).Select(o => new GenericSyntaxNode(o)));

            node.ApplyLineJoiningOperators();
            node.RemoveUnnecessaryNewlines();
            node.FindComments();
            node.FoldBrackets();
            node.FindVerbatimLines();
            node.FindNegativeNumbers();
            node.FindPreprocessorDefines();
            node.FindStructDefinitions();
            node.FindFunctionDeclarations();
            node.FindFunctionDefinitions();
            node.FindPragmaIfsInRoot();
            node.FindPragmaIfsInFunctionBodies();
            node.FindVectorComponents();
            node.FindIfStatements();
            node.FindSwitchStatements();
            node.FindVariableDeclarations();
            node.FindVariableDefinitions();
            node.FindForStatements();
            node.FindFunctionCalls();
            node.FindReturnStatements();

            return node;
        }

        private void FindVerbatimLines()
        {
            foreach (var node in Children.OfType<GenericSyntaxNode>().Where(o => o.Token is VerbatimToken).ToList())
                node.ReplaceWith(new VerbatimLineSyntaxNode(node.Token));
        }

        private void FindFunctionDeclarations()
        {
            for (var i = 0; i < Children.Count; i++)
            {
                var matches = TryGetMatchingChildren(i, typeof(TypeToken), typeof(GenericSyntaxNode), typeof(RoundBracketSyntaxNode), typeof(SemicolonToken));
                if (matches == null)
                    continue;

                Children[i].ReplaceWith(
                                        new FunctionDeclarationSyntaxNode(
                                                                          (GenericSyntaxNode)matches[0],
                                                                          (GenericSyntaxNode)matches[1],
                                                                          (RoundBracketSyntaxNode)matches[2]));
                matches[3].Remove();
            }
        }

        private void FindFunctionDefinitions()
        {
            for (var i = 0; i < Children.Count; i++)
            {
                var matches = TryGetMatchingChildren(i, typeof(TypeToken), typeof(GenericSyntaxNode), typeof(RoundBracketSyntaxNode), typeof(BraceSyntaxNode));
                if (matches == null)
                    continue;

                Children[i].ReplaceWith(
                                        new FunctionDefinitionSyntaxNode(
                                                                         (GenericSyntaxNode)matches[0],
                                                                         (GenericSyntaxNode)matches[1],
                                                                         (RoundBracketSyntaxNode)matches[2],
                                                                         (BraceSyntaxNode)matches[3]));
            }
        }

        private void FindPreprocessorDefines()
        {
            var nodes = TheTree.Where(o => o.Token is PreprocessorDefineToken).ToList();
            if (nodes.Any(o => TryGetMatchingChildren(o.NodeIndex, typeof(PreprocessorDefineToken), typeof(AlphaNumToken)) == null))
                throw new SyntaxErrorException("Expected name after #define.");

            foreach (var node in nodes)
            {
                var siblings = node.TakeSiblingsWhile(o => o.Token is not LineEndToken).ToList();
                siblings.LastOrDefault()?.Next?.Remove();

                var tokenCount = siblings.Count;

                if (tokenCount == 1)
                {
                    // #define NAME
                    node.ReplaceWith(new PragmaDefineSyntaxNode((GenericSyntaxNode)siblings.First()));
                    continue;
                }

                if (((PreprocessorDefineToken)node.Token).HasParams)
                {
                    var matches = node.TryGetMatchingSiblings(typeof(AlphaNumToken), typeof(RoundBracketSyntaxNode));
                    if (matches != null)
                    {
                        // #define NAME(...) ...
                        node.ReplaceWith(
                                         new PragmaDefineSyntaxNode(
                                                                    (GenericSyntaxNode)matches[0],
                                                                    (RoundBracketSyntaxNode)matches[1],
                                                                    node.TakeSiblings(tokenCount).Skip(2).ToList()));
                        continue;
                    }
                }

                // #define NAME ...
                node.ReplaceWith(
                                 new PragmaDefineSyntaxNode(
                                                            (GenericSyntaxNode)node.Next,
                                                            null,
                                                            node.TakeSiblings(tokenCount).Skip(1).ToList()));
            }
        }

        private void FindPragmaIfsInRoot() => FindPragmaIfs(this);

        private void FindPragmaIfsInFunctionBodies()
        {
            foreach (var functionNode in Children.OfType<FunctionDefinitionSyntaxNode>())
                FindPragmaIfs(functionNode.Braces);
        }

        private static void FindPragmaIfs(SyntaxNode rootNode)
        {
            while (true)
            {
                // Find start of #if section.
                var ifStartNode = rootNode.FindLastChild(o => new[] { "#if", "#ifdef", "#ifndef" }.Contains((o.Token as PreprocessorToken)?.Content));
                if (ifStartNode == null)
                    break;

                // Read '#if' arguments.
                var ifArgs = ifStartNode.TakeSiblingsWhile(o => o.Token is not LineEndToken).ToList();
                var ifName = new StringBuilder();
                new[] { ifStartNode }.Union(ifArgs).ToList().ForEach(o => OutputFormatter.AppendCode(ifName, o));

                // Find 'true' section.
                var ifTrueSection = ifArgs.Last().Next.TakeSiblingsWhile(o => !new[] { "#else", "#endif" }.Contains((o.Token as PreprocessorToken)?.Content)).ToList();

                // Is there an 'else' section?
                IList<SyntaxNode> ifFalseSection = null;
                if (ifTrueSection.Last().Next.Token.Content == "#else")
                {
                    ifFalseSection = ifTrueSection.Last().Next.Next.TakeSiblingsWhile(o => (o.Token as PreprocessorToken)?.Content != "#endif").ToList();
                }

                // Remove all nodes in the #if...#endif section.
                var lastNode = (ifFalseSection != null ? ifFalseSection.Last() : ifTrueSection.Last()).Next.Next;

                var toRemove = ifStartNode.TakeSiblingsWhile(o => o != lastNode).ToList();
                toRemove.ForEach(o => o.Remove());

                // Replace with single syntax node.
                if (ifStartNode.Next?.Token is LineEndToken)
                    ifStartNode.Next.Remove();
                ifStartNode.ReplaceWith(new PragmaIfSyntaxNode(ifName.ToString(), ifTrueSection, ifFalseSection));
            }
        }

        private void FindStructDefinitions()
        {
            var structs = new List<StructDefinitionSyntaxNode>();

            for (var i = 0; i < Children.Count; i++)
            {
                var matches = TryGetMatchingChildren(i, typeof(KeywordToken), typeof(AlphaNumToken), typeof(BraceSyntaxNode), typeof(SemicolonToken));
                if (matches == null)
                    continue;
                if (matches[0].Token.Content != "struct")
                    continue;

                matches[3].Remove(); // Remove the ';'
                structs.Add(new StructDefinitionSyntaxNode((GenericSyntaxNode)matches[1], (BraceSyntaxNode)matches[2]));
                Children[i].ReplaceWith(structs.Last());
            }

            // Replace all references to the struct type with a 'type' token.
            var didChange = true;
            while (didChange)
            {
                didChange = false;
                WalkTree(
                         node =>
                         {
                             if (node.Token is not AlphaNumToken)
                                 return true;

                             var structMatch = structs.FirstOrDefault(o => o.Name == node.Token.Content);
                             if (structMatch == null)
                                 return true;

                             node.ReplaceWith(new GenericSyntaxNode(new TypeToken(structMatch.Name)));

                             didChange = true;
                             return false;
                         });
            }
        }

        private void FindComments()
        {
            foreach (var node in TheTree.Where(o => o.IsComment()).ToList())
            {
                if (node.Token is SingleLineCommentToken)
                    node.ReplaceWith(new SingleLineCommentSyntaxNode(node));
                else
                    node.ReplaceWith(new MultiLineCommentSyntaxNode(node));
            }
        }

        private void FindNegativeNumbers()
        {
            WalkTree(
                     n =>
                     {
                         for (var i = n.Children.Count - 1; i >= 0; i--)
                         {
                             var node = n.Children[i];

                             if (node.Token?.Content != "-")
                                 continue;

                             if (node.Next?.Token is not INumberToken numberToken)
                                 continue;

                             if (node.Previous == null ||
                                 node.Previous?.Token is SymbolOperatorToken ||
                                 node.Previous?.Token is CommaToken)
                             {
                                 node.Remove();
                                 numberToken.MakeNegative();
                             }
                         }

                         return true;
                     }
                    );
        }

        private void FindIfStatements()
        {
            WalkTree(
                     n =>
                     {
                         var isInPragmaDefine = n.HasAncestor<PragmaDefineSyntaxNode>();
                         if (isInPragmaDefine)
                             return true;

                         for (var i = n.Children.Count - 1; i >= 0; i--)
                         {
                             var node = n.Children[i];

                             if (node.Token is not KeywordToken || node.Token.Content != "if")
                                 continue;

                             if (node.Next is not RoundBracketSyntaxNode conditions)
                                 continue;

                             var peekNode = conditions.Next;
                             BraceSyntaxNode trueBranch = null;
                             BraceSyntaxNode falseBranch = null;
                             foreach (var inTrueBranch in new[] { true, false })
                             {
                                 var branch = ReadNodesIntoBraceSyntaxNode(ref peekNode);
                                 if (inTrueBranch)
                                     trueBranch = branch;
                                 else
                                     falseBranch = branch;

                                 if (inTrueBranch)
                                 {
                                     var elseNode = peekNode;
                                     if ((elseNode?.Token as KeywordToken)?.Content != "else")
                                         break; // No 'else' branch.

                                     peekNode = elseNode.Next;
                                     elseNode.Remove();
                                 }
                             }

                             node.ReplaceWith(new IfSyntaxNode(conditions, trueBranch, falseBranch));
                         }

                         return true;
                     }
                    );
        }

        private void FindSwitchStatements()
        {
            WalkTree(
                     n =>
                     {
                         var isInPragmaDefine = n.HasAncestor<PragmaDefineSyntaxNode>();
                         if (isInPragmaDefine)
                             return true;

                         for (var i = n.Children.Count - 1; i >= 0; i--)
                         {
                             var node = n.Children[i];

                             if ((node.Token as KeywordToken)?.Content != "switch")
                                 continue;

                             if (node.Next is not RoundBracketSyntaxNode conditions)
                                 continue;

                             var peekNode = conditions.Next;
                             var braces = ReadNodesIntoBraceSyntaxNode(ref peekNode);

                             node.ReplaceWith(new SwitchSyntaxNode(conditions, braces));
                         }

                         return true;
                     }
                    );
        }

        private void FindForStatements()
        {
            WalkTree(
                     n =>
                     {
                         var isInPragmaDefine = n.HasAncestor<PragmaDefineSyntaxNode>();
                         if (isInPragmaDefine)
                             return true;

                         for (var i = n.Children.Count - 1; i >= 0; i--)
                         {
                             var node = n.Children[i];

                             if (node.Token is not KeywordToken || node.Token.Content != "for")
                                 continue;

                             if (node.Next is not RoundBracketSyntaxNode loopSetup)
                                 continue;

                             var peekNode = loopSetup.Next;
                             var loopCode = ReadNodesIntoBraceSyntaxNode(ref peekNode);

                             node.ReplaceWith(new ForSyntaxNode(loopSetup, loopCode));
                         }

                         return true;
                     }
                    );
        }

        private void FoldBrackets()
        {
            var didChange = true;
            while (didChange)
            {
                didChange = false;
                WalkTree(
                         node =>
                         {
                             var didFold1 = FoldBrackets("{", "}", node.m_children, () => new BraceSyntaxNode());
                             var didFold2 = FoldBrackets("(", ")", node.m_children, () => new RoundBracketSyntaxNode());
                             var didFold3 = FoldBrackets("[", "]", node.m_children, () => new SquareBracketSyntaxNode());
                             didChange = didFold1 || didFold2 || didFold3;
                             return !didChange;
                         }
                        );
            }
        }

        private void FindVectorComponents()
        {
            var didChange = true;
            while (didChange)
            {
                didChange = false;
                WalkTree(
                         node =>
                         {
                             // Combining components into 'v.xyz'.
                             if (node.Token is not AlphaNumToken)
                                 return true;

                             var matches = node.TryGetMatchingSiblings(typeof(DotToken), typeof(AlphaNumToken));
                             if (matches == null)
                                 return true;

                             node.ReplaceWith(new GenericSyntaxNode($"{node.Token.Content}.{matches[1].Token.Content}"));
                             matches.ForEach(o => o.Remove());

                             didChange = true;
                             return false;
                         }
                        );
            }
        }

        private void FindVariableDeclarations()
        {
            // Find declarations of the for '<type> <name>, <name2> = <value>, ...;
            foreach (var typeNode in TheTree.OfType<GenericSyntaxNode>()
                .Where(o => o.Token is TypeToken && o.Next?.Token is AlphaNumToken)
                .Where(o => !o.HasAncestor<FunctionDeclarationSyntaxNode>()) // We can ignore function declarations.
                .ToList())
            {
                // Ignore declarations within a 'for (...)'.
                if (typeNode.Parent is RoundBracketSyntaxNode && typeNode.Parent.Previous.Token.Content == "for")
                    continue;

                var namesAndValues = new List<List<SyntaxNode>>();
                var isFunctionParam = typeNode.Parent is RoundBracketSyntaxNode;
                if (isFunctionParam)
                {
                    // We don't need to process these - Leave them as nodes.
                    continue;
                }

                // Check for '<type> <name1>, <name2> = <value>, ...;'
                var nameNode = typeNode.NextNonComment;
                while (nameNode != null)
                {
                    if (nameNode.NextNonComment.Token is AssignmentOperatorToken)
                    {
                        var valueNodes = nameNode.NextSiblings.Where(o => !o.IsComment())
                            .Skip(1) // Skip the '='
                            .TakeWhile(o => o != null && o.Token is not CommaToken && o.Token is not SemicolonToken)
                            .ToList();
                        valueNodes.Insert(0, nameNode);
                        namesAndValues.Add(valueNodes);
                    }
                    else
                    {
                        // No assignment - Just a declaration. (E.g. <type> <name>)
                        namesAndValues.Add(new List<SyntaxNode> { nameNode });
                    }

                    // Multiple names?
                    var commaNode = namesAndValues.Last().Last().NextNonComment;
                    nameNode = commaNode?.Token is CommaToken ? commaNode.NextNonComment : null;
                }

                // Create declaration node.
                var terminatorNode = namesAndValues.Last().Last().Next;
                var declNode = new VariableDeclarationSyntaxNode(typeNode);
                var assignments = namesAndValues.Select(o => new VariableAssignmentSyntaxNode(new GenericSyntaxNode(o.First().Token.Content), o.Count > 1 ? o.Skip(1).ToList() : null)).OfType<SyntaxNode>().ToArray();
                declNode.Adopt(assignments);

                // Replace original nodes with the new versions.
                var countToReplace = typeNode.NextSiblings.TakeWhile(o => o != null && o != terminatorNode).Count();
                var toReplace = typeNode.NextSiblings.Take(countToReplace + 1).ToList();
                toReplace.ForEach(o => o.Remove());
                typeNode.ReplaceWith(declNode);
            }
        }

        private void FindVariableDefinitions()
        {
            var didChange = true;
            while (didChange)
            {
                didChange = false;
                WalkTree(
                         node =>
                         {
                             if (node.Token is not AlphaNumToken)
                                 return true;
                             if (node.TryGetMatchingSiblings(typeof(AssignmentOperatorToken)) == null)
                                 return true;

                             // Find terminating ';'
                             var valueNodes = new List<SyntaxNode>();
                             var semicolonNode = node.Next;
                             while (semicolonNode != null)
                             {
                                 semicolonNode = semicolonNode.Next;

                                 if (semicolonNode?.Token is SemicolonToken)
                                     break;

                                 valueNodes.Add(semicolonNode);
                             }

                             if (semicolonNode == null)
                                 return true;

                             // Remove the '=' and ';' nodes.
                             var equalsNode = node.Next;
                             equalsNode.Remove();
                             semicolonNode.Remove();

                             node.ReplaceWith(new VariableAssignmentSyntaxNode((GenericSyntaxNode)node, valueNodes));
                             didChange = true;

                             return false;
                         }
                        );
            }
        }

        private void FindFunctionCalls()
        {
            var knownFunctionNames = new List<string>();
            WalkTree(
                     node =>
                     {
                         if (node is FunctionDeclarationSyntaxNode declarationNode)
                             knownFunctionNames.Add(declarationNode.Name);
                         else if (node is FunctionDefinitionSyntaxNode definitionNode)
                             knownFunctionNames.Add(definitionNode.Name);

                         return true;
                     });
            knownFunctionNames = knownFunctionNames.Distinct().ToList();

            var didChange = true;
            while (didChange)
            {
                didChange = false;
                WalkTree(
                         node =>
                         {
                             if (node.Token is not AlphaNumToken)
                                 return true;
                             var match = node.TryGetMatchingSiblings(typeof(RoundBracketSyntaxNode));
                             if (match == null)
                                 return true;
                             if (node.Parent is FunctionDeclarationSyntaxNode || node.Parent is FunctionDefinitionSyntaxNode)
                                 return true;
                             if (!knownFunctionNames.Contains(node.Token.Content))
                                 return true;

                             node.ReplaceWith(new FunctionCallSyntaxNode((GenericSyntaxNode)node, (RoundBracketSyntaxNode)match.Single()));
                             didChange = true;

                             return false;
                         }
                        );
            }
        }

        private void FindReturnStatements()
        {
            var nodes = TheTree.OfType<GenericSyntaxNode>()
                .Where(o => o.Token?.Content == "return")
                .ToList();
            foreach (var node in nodes)
            {
                var returnNode = new ReturnSyntaxNode();

                // Read to terminating ';'.
                var returnArgs = node.TakeSiblingsWhile(o => o.Token is not SemicolonToken).ToArray();
                returnNode.Adopt(returnArgs);

                node.ReplaceWith(returnNode);
            }
        }

        private void RemoveUnnecessaryNewlines()
        {
            for (var i = Children.Count - 1; i >= 0; i--)
            {
                // Find a newline.
                if (Children[i].Token is not LineEndToken)
                    continue;

                // Keep newline if preceded by a #pragma
                var pragmaFound = false;
                var index = i;
                while (!pragmaFound && index > 0 && Children[index - 1].Token is not LineEndToken)
                {
                    pragmaFound = Children[index - 1].Token is PreprocessorToken;
                    if (!pragmaFound)
                        index--;
                }

                if (pragmaFound)
                    continue;

                // Newline is not required.
                Children[i].Remove();
            }
        }

        private void ApplyLineJoiningOperators()
        {
            Children
                .Where(o => o.Token is BackslashToken && o.Next.Token is LineEndToken)
                .ToList()
                .ForEach(
                         o =>
                         {
                             o.Next.Remove();
                             o.Remove();
                         });
        }
    }
}