// -----------------------------------------------------------------------
//  <copyright file="FileSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
{
    public class FileSyntaxNode : SyntaxNode
    {
        public override string UiName => "<ROOT>";

        protected override SyntaxNode CreateSelf() => new FileSyntaxNode();

        private FileSyntaxNode()
        {
        }

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
            node.FindPragmaIfs();
            node.FindVectorComponents();
            node.FindIfStatements();
            node.FindSwitchStatements();
            node.FindVariableDeclarations();
            node.FindVariableAssignments();
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
                // Non-array return type.
                var matches = TryGetMatchingChildren(i, typeof(TypeToken), typeof(GenericSyntaxNode), typeof(RoundBracketSyntaxNode), typeof(BraceSyntaxNode));
                if (matches == null)
                {
                    // Try array return type.
                    matches = TryGetMatchingChildren(i, typeof(TypeToken), typeof(SquareBracketSyntaxNode), typeof(GenericSyntaxNode), typeof(RoundBracketSyntaxNode), typeof(BraceSyntaxNode));
                    if (matches != null)
                    {
                        // Fold the brackets into the 'type' node.
                        matches[0].Adopt(matches[1]);
                        matches.RemoveAt(1);
                    }
                }

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
            if (nodes.Any(o => o.TryGetMatchingSiblings(typeof(AlphaNumToken)) == null))
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

        private void FindPragmaIfs()
        {
            var startNodes = new List<(int i, SyntaxNode node)>();
            var elseNodes = new List<(int i, SyntaxNode node)>();
            var endNodes = new List<(int i, SyntaxNode node)>();

            var theTree = TheTree;
            for (var i = 0; i < theTree.Count; i++)
            {
                if (PragmaIfSyntaxNode.IsStart(theTree[i]))
                    startNodes.Add((i, theTree[i]));
                else if (PragmaIfSyntaxNode.IsElse(theTree[i]))
                    elseNodes.Add((i, theTree[i]));
                else if (PragmaIfSyntaxNode.IsEnd(theTree[i]))
                    endNodes.Add((i, theTree[i]));
            }

            if (startNodes.Count != endNodes.Count)
                throw new SyntaxErrorException($"Unbalanced #if...#endif statements ({startNodes.Count} vs {endNodes.Count}).");

            foreach (var startNode in startNodes.AsEnumerable().Reverse())
            {
                // Find matching end node.
                var endNode = endNodes.First(o => o.i > startNode.i);
                endNodes.Remove(endNode);

                // Is there an 'else'?
                var elseNode = elseNodes.FirstOrDefault(o => o.i > startNode.i && o.i < endNode.i);
                if (elseNode.node != null)
                    elseNodes.Remove(elseNode);

                startNode.node.ReplaceWith(new PragmaIfSyntaxNode(startNode.node, elseNode.node, endNode.node));
            }
        }

        private void FindStructDefinitions()
        {
            var structs = new List<StructDefinitionSyntaxNode>();

            for (var i = 0; i < Children.Count; i++)
            {
                var matches = TryGetMatchingChildren(i, typeof(KeywordToken), typeof(AlphaNumToken), typeof(BraceSyntaxNode));
                if (matches == null)
                    continue;
                if (matches[0].Token.Content != "struct")
                    continue;

                // Does struct declare variable instance too?
                var declaredVarNodes = new List<SyntaxNode>();
                var nextNonComment = matches.Last().NextNonComment;
                if (nextNonComment?.Token is SemicolonToken)
                {
                    // Nope.
                    nextNonComment.Remove(); // Remove the ';'
                }
                else
                {
                    // Yes - Remove it (for now).
                    declaredVarNodes.AddRange(
                                              nextNonComment
                                                  .SelfAndNextSiblings
                                                  .Where(o => o is not CommentSyntaxNodeBase)
                                                  .TakeWhile(o => o.Token is not SemicolonToken));
                    declaredVarNodes.Last().Next.Remove(); // Remove the ';'
                    declaredVarNodes.ForEach(o => o.Remove());
                }

                // Make a struct node.
                var nameNode = (GenericSyntaxNode)matches[1];
                var structNode = new StructDefinitionSyntaxNode(nameNode, (BraceSyntaxNode)matches[2]);
                structs.Add(structNode);
                TypeToken.RegisterUserStruct(nameNode.Name);
                Children[i].ReplaceWith(structs.Last());

                // Re-add the variable declaration (if there was one).
                if (declaredVarNodes.Any())
                {
                    var replacementDeclaration = new VariableDeclarationSyntaxNode(new GenericSyntaxNode(new TypeToken(structNode.Name)));

                    var flatNodeList = new GroupSyntaxNode(declaredVarNodes);
                    replacementDeclaration.Adopt(new VariableAssignmentSyntaxNode((GenericSyntaxNode)flatNodeList.Children.First()));
                    structNode.InsertNextSibling(replacementDeclaration);
                }

                // Replace later occurrences of the struct name with a type token.
                var references = Children[i].NextSiblings.SelectMany(o => o.TheTree).OfType<GenericSyntaxNode>().Where(o => o.Token?.Content == structNode.Name).ToList();
                foreach (var reference in references)
                {
                    var typeToken = new TypeToken(structNode.Name);

                    if (reference.Previous?.Token is ConstToken)
                    {
                        typeToken.IsConst = true;
                        reference.Previous.Remove();
                    }
                    
                    if (typeToken.SetInOut((reference.Previous?.Token as KeywordToken)?.Content))
                        reference.Previous.Remove();

                    reference.ReplaceWith(new GenericSyntaxNode(typeToken));
                }
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
                                 node.Previous.Token is SymbolOperatorToken or CommaToken or AssignmentOperatorToken)
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

                             if (node.NextNonComment is not RoundBracketSyntaxNode conditions)
                                 continue;

                             var peekNode = conditions.NextNonComment;
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

                                     peekNode = elseNode.NextNonComment;
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

                             if ((node.Token as KeywordToken)?.Content != "for")
                                 continue;

                             if (node.Next is not RoundBracketSyntaxNode loopSetup)
                                 continue;

                             var peekNode = loopSetup.NextNonComment;
                             while (peekNode.Previous != loopSetup) // Remove any comments between () and {}.
                                 peekNode.Previous.Remove();

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
                .Where(o => o.Token is TypeToken &&
                            o.Next?.Token is AlphaNumToken &&
                            !o.HasAncestor<PragmaDefineSyntaxNode>())
                .Where(o => !o.HasAncestor<FunctionDeclarationSyntaxNode>()) // We can ignore function declarations.
                .ToList())
            {
                // Ignore declarations within a 'for (...)'.
                if (typeNode.Parent is RoundBracketSyntaxNode && typeNode.Parent.Previous.HasNodeContent("for"))
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
                    // Does name have [] suffix?
                    var nameArraySuffix = nameNode.NextNonComment as SquareBracketSyntaxNode;

                    // Is the variable assigned too?
                    if ((nameArraySuffix ?? nameNode).NextNonComment.Token is AssignmentOperatorToken)
                    {
                        var valueNodes = (nameArraySuffix ?? nameNode).NextSiblings.Where(o => !o.IsComment())
                            .Skip(1) // Skip the '='
                            .TakeWhile(o => o != null && o.Token is not CommaToken && o.Token is not SemicolonToken)
                            .ToList();
                        valueNodes.Insert(0, nameNode);
                        namesAndValues.Add(valueNodes);
                    }
                    else
                    {
                        // No assignment - Just a declaration. (E.g. <type> <name>)
                        namesAndValues.Add(nameArraySuffix != null ? new List<SyntaxNode> { nameNode, nameArraySuffix } : new List<SyntaxNode> { nameNode });
                    }

                    // Include the [].
                    if (nameArraySuffix != null)
                        namesAndValues.Last().Insert(1, nameArraySuffix);

                    // Multiple names?
                    var commaNode = namesAndValues.Last().Last().NextNonComment;
                    nameNode = commaNode?.Token is CommaToken ? commaNode.NextNonComment : null;
                }

                // Create declaration node.
                var terminatorNode = namesAndValues.Last().Last().Next;
                var declNode = new VariableDeclarationSyntaxNode(typeNode);
                var assignments = namesAndValues.Select(o => new VariableAssignmentSyntaxNode(new GenericSyntaxNode(o.First().Token.Clone()), o.Count > 1 ? o.Skip(1).ToList() : null)).OfType<SyntaxNode>().ToArray();
                declNode.Adopt(assignments);

                // Replace original nodes with the new versions.
                var countToReplace = typeNode.NextSiblings.TakeWhile(o => o != null && o != terminatorNode).Count();
                var toReplace = typeNode.NextSiblings.Take(countToReplace + 1).ToList();
                toReplace.ForEach(o => o.Remove());
                typeNode.ReplaceWith(declNode);
            }
        }

        private void FindVariableAssignments()
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

                             var assignmentNode = node.NextSiblings.SkipWhile(o => o is CommentSyntaxNodeBase || o is SquareBracketSyntaxNode).FirstOrDefault();
                             if (assignmentNode == null || assignmentNode.Token is not AssignmentOperatorToken)
                                 return true;

                             // Find terminating ';'
                             var semicolonNode = assignmentNode.NextSiblings.FirstOrDefault(o => o.Token is SemicolonToken);
                             if (semicolonNode == null)
                                 return true; // None found.

                             var valueNodes = assignmentNode.NextSiblings.TakeWhile(o => o != semicolonNode).ToList();

                             // Remove the '=' and ';' nodes.
                             assignmentNode.Remove();
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
            var userDefinedFunctions = this.FunctionDefinitions().Select(o => o.Name).ToList();
            userDefinedFunctions.AddRange(this.FunctionDeclarations().Select(o => o.Name));

            var macros = TheTree.OfType<PragmaDefineSyntaxNode>().Select(o => o.Name).ToList();

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

                             if (!FunctionCallSyntaxNode.IsNodeFunctionLike(node))
                                 return true;

                             var functionName = node.Token.Content;
                             var roundBrackets = (RoundBracketSyntaxNode)match.Single();

                             FunctionCallSyntaxNode functionCall = null;
                             if (userDefinedFunctions.Contains(functionName))
                             {
                                 // User-defined function.
                                 functionCall = new FunctionCallSyntaxNode((GenericSyntaxNode)node, roundBrackets);
                             }
                             else if (GlslFunctionCallSyntaxNode.IsGlslFunction(functionName))
                             {
                                 // GLSL function.
                                 functionCall = new GlslFunctionCallSyntaxNode((GenericSyntaxNode)node, roundBrackets);
                             }
                             else if (macros.Contains(functionName))
                             {
                                 // #defined macro - Ignore.
                                 return true;
                             }
                             else
                             {
                                 // External function - Hopefully defined in another buffer.
                                 functionCall = new ExternalFunctionCallSyntaxNode((GenericSyntaxNode)node, roundBrackets);
                             }

                             node.ReplaceWith(functionCall);
                             didChange = true;
                             return false;
                         }
                        );
            }
        }

        private void FindReturnStatements()
        {
            var nodes = TheTree.OfType<GenericSyntaxNode>()
                .Where(o => o.HasNodeContent("return"))
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