// -----------------------------------------------------------------------
//  <copyright file="Shrinker.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Shrinker.Lexer;

// todo - bigwings cradle bad
// todo - keep header comments.
// todo - auto-reshrink after custom options modded.
// todo - https://www.shadertoy.com/view/7s2XWh nullref
// todo - 1e3 form can be used if with vecN(...)
// todo - Bonzo has 'void main(void)'
// todo - 'fragColor = vec4(col, 1.0)' - Inline 'col'.
// todo - 'vec3 p = vec3(r, t, ph), f = fract(p * 1.59) - .5;' <- Inline p?
// todo - ED-209 (float d = .01 * t * .33;)
// todo - Remove 'return;' specifically at end of function.
namespace Shrinker.Parser
{
    /// <summary>
    /// Takes a syntax tree produced by the Parser class, and optimizes it.
    /// </summary>
    /// <remarks>The output from this stage can be passed to the OutputFormatter.</remarks>
    public static class Shrinker
    {
        private const int MaxDp = 5;

        public static SyntaxNode Simplify(SyntaxNode rootNode, CustomOptions options = null)
        {
            options ??= new CustomOptions();

            var repeatSimplifications = true;
            while (repeatSimplifications)
            {
                repeatSimplifications = false;

                if (options.RemoveDisabledCode)
                {
                    // Remove #if 0...#endif code.
                    rootNode.WalkTree(
                                      node =>
                                      {
                                          var pragmaNodes = node.Children
                                              .OfType<PragmaIfSyntaxNode>()
                                              .Where(o => o.Is0() || o.Is1())
                                              .ToList();
                                          foreach (var pragmaNode in pragmaNodes)
                                          {
                                              if (pragmaNode.Is1())
                                              {
                                                  pragmaNode.ReplaceWith(pragmaNode.TrueBranch);
                                                  continue;
                                              }

                                              if (pragmaNode.FalseBranch != null)
                                                  pragmaNode.ReplaceWith(pragmaNode.FalseBranch);
                                              else
                                                  pragmaNode.Remove();
                                          }

                                          return true;
                                      });

                    // Remove commented-out code.
                    rootNode.TheTree
                        .OfType<CommentSyntaxNodeBase>()
                        .Where(o => o.Comment.Contains(';'))
                        .ToList()
                        .ForEach(o => o.Remove());
                }

                if (options.RemoveUnusedFunctions)
                {
                    while (true)
                    {
                        var functionRemoved = false;

                        var localFunctions = rootNode.FindFunctionDefinitions().ToList();
                        foreach (var testFunction in localFunctions.Where(o => !o.IsMain()))
                        {
                            var otherFunctions = localFunctions.Where(o => o != testFunction).ToList();
                            if (otherFunctions.SelectMany(o => o.Braces.TheTree)
                                .OfType<FunctionCallSyntaxNode>()
                                .Any(o => o.Name == testFunction.Name))
                                continue; // Function was used.

                            // Function not used - Remove it (and any matching declaration).
                            testFunction.Remove();
                            rootNode.Children.OfType<FunctionDeclarationSyntaxNode>().FirstOrDefault(o => o.Name == testFunction.Name)?.Remove();
                            functionRemoved = true;
                        }

                        if (!functionRemoved)
                            break;
                    }
                }

                if (options.RemoveComments)
                    rootNode.TheTree.OfType<CommentSyntaxNodeBase>().ToList().ForEach(o => o.Remove());
                
                if (options.RemoveUnusedVariables)
                {
                    foreach (var decl in rootNode.TheTree
                        .OfType<VariableDeclarationSyntaxNode>()
                        .Where(o => !o.HasAncestor<StructDefinitionSyntaxNode>() &&
                                    (o.VariableType.InOut == TypeToken.InOutType.In ||
                                     o.VariableType.InOut == TypeToken.InOutType.None)) // inout/out variables should not be removed.
                        .Where(o => o.FindAncestor<FunctionDefinitionSyntaxNode>()?.Params.Children.Contains(o) != true) // Don't remove function params.
                        .ToList())
                    {
                        foreach (var varName in decl.Definitions.Select(o => o.Name).ToList())
                        {
                            // Any references?
                            if (decl.Parent.TheTree
                                .OfType<GenericSyntaxNode>()
                                .Where(o => o.Token?.Content != null)
                                .Any(o => o.StartsWithVarName(varName)))
                                continue; // Variable was used.

                            // Variable not used - Remove any definitions using it.
                            var defs = decl.Parent.TheTree
                                .OfType<VariableAssignmentSyntaxNode>()
                                .Where(o => o.Name == varName).ToList();
                            defs.ForEach(o => o.Remove());

                            if (!decl.Definitions.Any())
                                decl.Remove();
                        }
                    }
                }

                if (options.SimplifyFunctionDeclarations)
                {
                    var functionDeclarations = rootNode.Children.OfType<FunctionDeclarationSyntaxNode>().ToList();

                    // Remove param names.
                    foreach (var declaration in functionDeclarations.Where(o => o.Params.Children.Any()))
                    {
                        var paramNames = declaration.Params.Children
                            .OfType<GenericSyntaxNode>()
                            .Where(o => o.Token is AlphaNumToken)
                            .ToList();
                        paramNames.ForEach(o => o.Remove());
                    }

                    // Remove declaration if no matching definition.
                    var functionDefinitionNames = rootNode.FindFunctionDefinitions().Select(o => o.Name);
                    var surplusDeclarations = functionDeclarations.Where(o => !functionDefinitionNames.Contains(o.Name)).ToList();

                    // Remove declaration if next reference is the definition.
                    var flatNodes = rootNode.TheTree.ToList();
                    foreach (var declaration in functionDeclarations)
                    {
                        var definitionSite = rootNode.FindFunctionDefinitions().FirstOrDefault(o => o.Name == declaration.Name);
                        var definitionIndex = flatNodes.IndexOf(definitionSite);

                        var firstCallSite = flatNodes.OfType<FunctionCallSyntaxNode>().FirstOrDefault(o => o.Name == declaration.Name);
                        var firstCallIndex = flatNodes.IndexOf(firstCallSite);

                        if (firstCallIndex > definitionIndex)
                            surplusDeclarations.Add(declaration);
                    }

                    surplusDeclarations.ForEach(o => o.Remove());
                }

                if (options.GroupVariableDeclarations)
                {
                    rootNode.WalkTree(
                                      node =>
                                      {
                                          if (!node.Children.OfType<VariableDeclarationSyntaxNode>().Any())
                                              return true;

                                          if (node.HasAncestor<PragmaIfSyntaxNode>())
                                              return true;

                                          if (!node.HasAncestor<StructDefinitionSyntaxNode>())
                                          {
                                              // Float all const declarations to the top.
                                              var constDecls =
                                                  node.Children.OfType<VariableDeclarationSyntaxNode>()
                                                      .Where(o => o.VariableType.IsConst && !o.IsWithinIfPragma())
                                                      .Reverse()
                                                      .ToList();
                                              var firstNonCommentLine = 0;
                                              if (node == rootNode && constDecls.Any())
                                              {
                                                  // Const is defined at the global scope - Move after any file header comments.
                                                  while (firstNonCommentLine < node.Children.Count && node.Children[firstNonCommentLine].IsComment())
                                                      firstNonCommentLine++;
                                              }

                                              constDecls.ForEach(o => node.InsertChild(firstNonCommentLine, o));

                                              // Groups of non-const definitions can be placed together.
                                              var i = 0;
                                              while (i < node.Children.Count)
                                              {
                                                  // Find a declaration.
                                                  var decl = node.Children
                                                      .Skip(i)
                                                      .OfType<VariableDeclarationSyntaxNode>()
                                                      .FirstOrDefault(o => !o.IsWithinIfPragma());
                                                  if (decl == null)
                                                      break;

                                                  // Find following declarations which have the same type.
                                                  var similarDecls =
                                                      decl.NextSiblings
                                                          .OfType<VariableDeclarationSyntaxNode>()
                                                          .Where(o => !o.IsWithinIfPragma())
                                                          .Where(o => o.VariableType.Content == decl.VariableType.Content)
                                                          .ToList();

                                                  // Append the variable names to the first declaration variable list.
                                                  var declNames = similarDecls.SelectMany(o => o.Definitions).Select(o => o.Name).ToList();
                                                  declNames.ForEach(o => decl.Adopt(new VariableAssignmentSyntaxNode(o)));

                                                  // ...and make the assignment stand on its own (outside of its declaration).
                                                  similarDecls.SelectMany(o => o.Definitions).Where(o => !o.HasValue).ToList().ForEach(o => o.Remove());
                                                  similarDecls.ForEach(o => o.ReplaceWith(o.Definitions));

                                                  i = decl.NodeIndex + 1;
                                              }
                                          }
                                          else
                                          {
                                              // Combine consecutive struct fields of the same type.
                                              var n = node.Children.FirstOrDefault();
                                              while (n != null)
                                              {
                                                  if (n is VariableDeclarationSyntaxNode decl1 &&
                                                      n.Next is VariableDeclarationSyntaxNode decl2 &&
                                                      decl1.VariableType.Content == decl2.VariableType.Content)
                                                  {
                                                      decl1.Adopt(decl2.Definitions.Cast<SyntaxNode>().ToArray());
                                                      decl2.Remove();
                                                      continue;
                                                  }

                                                  n = n.Next;
                                              }
                                          }

                                          return true;
                                      });
                }

                if (options.JoinVariableDeclarationsWithAssignments || options.InlineConstantVariables)
                {
                    rootNode.WalkTree(
                                      node =>
                                      {
                                          while (true)
                                          {
                                              var i = 0;
                                              while (i < node.Children.Count)
                                              {
                                                  var n = node.Children[i++];

                                                  // Find declaration.
                                                  if (n is not VariableDeclarationSyntaxNode decl)
                                                      continue;

                                                  while (n != null)
                                                  {
                                                      var isSimpleAssignment = false;
                                                      VariableAssignmentSyntaxNode defn;
                                                      if (decl.VariableType.IsConst || decl.Parent?.Parent == null)
                                                      {
                                                          // Pull in the const definition wherever it is.
                                                          var declVarNames = decl.Definitions.Where(o => !o.HasValue).Select(o => o.Name).ToList();
                                                          defn = node.Children.Skip(n.NodeIndex).OfType<VariableAssignmentSyntaxNode>().FirstOrDefault(o => declVarNames.Contains(o.Name));
                                                      }
                                                      else
                                                      {
                                                          // The definition must closely follow the declaration.
                                                          defn = GetGroupOfType<VariableAssignmentSyntaxNode>(decl.Next).FirstOrDefault();

                                                          // UNLESS the assignment does not depend on anything else!
                                                          if (defn == null)
                                                          {
                                                              var candidateDefn = decl.NextSiblings.OfType<VariableAssignmentSyntaxNode>().FirstOrDefault();
                                                              if (candidateDefn?.HasValue == true &&
                                                                  decl.Definitions.Any(o => o.Name == candidateDefn.Name) &&
                                                                  candidateDefn.IsSimpleAssignment())
                                                              {
                                                                  var nodesInBetween = decl.TakeSiblingsWhile(o => o != candidateDefn);
                                                                  if (!nodesInBetween.All(o => o is BraceSyntaxNode || o is IfSyntaxNode || o is SwitchSyntaxNode))
                                                                  {
                                                                      isSimpleAssignment = true;
                                                                      defn = candidateDefn;
                                                                  }
                                                              }
                                                          }
                                                      }

                                                      if (defn == null)
                                                          break;

                                                      // Definition found - Does it match declaration?
                                                      var declDef = decl.Definitions.FirstOrDefault(o => o.Name == defn.Name && !o.HasValue);
                                                      if (declDef != null)
                                                      {
                                                          // If the (non-const) decl variable list has a defined value _after_
                                                          // the name we just found, we can't join them.
                                                          var indexOfDeclName = decl.Definitions.ToList().IndexOf(declDef);
                                                          if (isSimpleAssignment || decl.VariableType.IsConst || !decl.Definitions.Skip(indexOfDeclName + 1).Any(o => o.HasValue))
                                                          {
                                                              // Join them.
                                                              declDef.Adopt(defn.Children.ToArray());
                                                              defn.Remove();
                                                              continue;
                                                          }
                                                      }

                                                      // Advance to next node.
                                                      n = n.Next;
                                                  }
                                              }

                                              // Move any decls which completely unassigned nearer the definition.
                                              var declWithNoDefs = node.Children
                                                  .OfType<VariableDeclarationSyntaxNode>()
                                                  .Where(o => !o.IsWithinIfPragma())
                                                  .FirstOrDefault(o => o.Definitions.All(d => !d.HasValue));
                                              if (declWithNoDefs == null)
                                                  break; // Nope - Give up.

                                              // Yes - Move it to nearer the first suitable definition.
                                              var unassignedVariableNames = declWithNoDefs.Definitions.Select(o => o.Name).ToList();
                                              var nextDefNodes = node.TheTree
                                                  .OfType<VariableAssignmentSyntaxNode>()
                                                  .Where(o => unassignedVariableNames.Contains(o.Name) && o.HasValue)
                                                  .ToList();
                                              if (nextDefNodes.Any(o => !node.Children.Contains(o)))
                                              {
                                                  // One of the variables is defined in a sub-tree of code.
                                                  // We won't move the declaration...
                                                  break;
                                              }

                                              var nearestDefNode = nextDefNodes.OrderBy(o => o.NodeIndex).FirstOrDefault();
                                              if (nearestDefNode == null)
                                                  break; // No definition node found - Give up.

                                              // Move declaration to before definition.
                                              declWithNoDefs.Remove();
                                              node.InsertChild(nearestDefNode.NodeIndex, declWithNoDefs);
                                          }

                                          return true;
                                      });

                    foreach (var declNode in rootNode.TheTree
                        .OfType<VariableDeclarationSyntaxNode>()
                        .Where(o => o.Definitions.Any(d => d.HasValue) && o.Definitions.Any(d => !d.HasValue))
                        .ToList())
                    {
                        var reorderedDefs = declNode.Definitions.OrderBy(o => o.HasValue ? 1 : 0).Cast<SyntaxNode>().ToArray();
                        declNode.Adopt(reorderedDefs);
                    }
                }

                if (options.DetectConstants)
                {
                    foreach (var decl in rootNode.TheTree
                        .OfType<VariableDeclarationSyntaxNode>()
                        .Where(o => !o.VariableType.IsConst && !o.VariableType.IsUniform))
                    {
                        foreach (var defCandidate in decl.Definitions.Where(o => o.IsSimpleAssignment()))
                        {
                            // Is this variable assigned anywhere else?
                            var isReassigned = decl.Parent.TheTree
                                .OfType<VariableAssignmentSyntaxNode>()
                                .Where(o => o != defCandidate)
                                .Any(o => o.Name == defCandidate.Name);

                            if (!isReassigned)
                            {
                                // Perhaps modified using an operator?
                                var isModified = decl.Parent.TheTree
                                    .OfType<GenericSyntaxNode>()
                                    .Any(o => o.Token?.Content.StartsWithVarName(defCandidate.Name) == true &&
                                              SymbolOperatorToken.ModifyingOperator.Contains(o.Next?.Token?.Content));
                                isReassigned = isModified;
                            }

                            if (isReassigned)
                                continue;

                            // Make a const version.
                            var newType = new TypeToken(decl.VariableType.Content)
                            {
                                IsConst = true
                            };
                            var newDecl = new VariableDeclarationSyntaxNode(new GenericSyntaxNode(newType), new GenericSyntaxNode(defCandidate.Name));
                            newDecl.Children.Single().Adopt(defCandidate.Children.ToArray());
                            defCandidate.Remove();

                            decl.Parent.InsertChild(decl.NodeIndex, newDecl);

                            // Remove declaration if has no other content.
                            if (!decl.Definitions.Any())
                                decl.Remove();

                            repeatSimplifications = true;
                        }
                    }
                }

                if (options.InlineDefines)
                {
                    // #defines.
                    var pragmaIfs = rootNode.TheTree.OfType<PragmaIfSyntaxNode>().Select(o => o.Name).ToList();
                    var foo1 = rootNode.TheTree
                        .OfType<PragmaDefineSyntaxNode>();
                    foreach (var define in foo1
                        .Where(
                               o => o.Params == null &&
                                    o.ValueNodes?.Count == 1 &&
                                    !o.HasAncestor<PragmaIfSyntaxNode>() &&
                                    !pragmaIfs.Any(p => p.Contains(o.Name)))
                        .ToList())
                    {
                        var usages = rootNode.TheTree
                            .OfType<GenericSyntaxNode>()
                            .Where(o => (o.Token as AlphaNumToken)?.Content.StartsWithVarName(define.Name) == true)
                            .Where(o => o.Parent != define)
                            .ToList();

                        var countIfKept = define.ToCode().GetCodeCharCount() + usages.Count * define.Name.Length;
                        var countIfRemoved = usages.Count * define.ValueNodes.Sum(o => o.ToCode().Length);

                        if (countIfRemoved < countIfKept)
                        {
                            define.Remove();
                            foreach (var usage in usages)
                            {
                                var newContent = define.ValueNodes.Select(o => new GenericSyntaxNode(o.Token)).ToList();
                                if (usage.Token.Content != define.Name)
                                {
                                    // We have a reference of the form DEFINE.xy, so keep the last '.xy'.
                                    var newLastContent = newContent.Last().Token.Content + usage.Token.Content.Substring(define.Name.Length);
                                    newContent[newContent.Count - 1] = new GenericSyntaxNode(newLastContent);
                                }

                                usage.ReplaceWith(newContent);
                            }
                        }
                    }
                }

                if (options.InlineConstantVariables)
                {
                    // Const variables.
                    var consts = rootNode.TheTree
                        .OfType<VariableDeclarationSyntaxNode>()
                        .Where(o => o.VariableType.IsConst && o.Definitions.Any())
                        .ToList();
                    foreach (var constDeclNode in consts)
                    {
                        var potentialUsage = constDeclNode.Parent.TheTree.ToList()
                            .OfType<GenericSyntaxNode>()
                            .Where(o => o.Token is AlphaNumToken)
                            .ToList();
                        foreach (var definition in constDeclNode.Definitions.Where(o => o.IsSimpleAssignment()).ToList())
                        {
                            var usages = potentialUsage.Where(o => o.Token.Content.StartsWithVarName(definition.Name)).ToList();
                            var anyDotReferences = usages.Any(o => o.Token.Content.Contains('.'));

                            if (anyDotReferences)
                                continue;

                            if (usages.Count == 1)
                            {
                                // No copying needed - Just move definition to it's single use.
                                usages.ForEach(o => o.ReplaceWith(definition.Children.ToList()));
                                definition.Remove();
                            }
                            else if (definition.TheTree.Count == 2)
                            {
                                // The const is just a number - Copy the node.
                                usages.ForEach(o => o.ReplaceWith(definition.Children.Select(d => new GenericSyntaxNode(d.Token)).ToList()));
                                definition.Remove();
                            }
                        }

                        if (!constDeclNode.Definitions.Any())
                            constDeclNode.Remove();
                    }
                }

                if (options.SimplifyFloatFormat)
                    rootNode.TheTree.ToList().ForEach(o => (o.Token as DoubleNumberToken)?.Simplify());

                if (options.SimplifyVectorConstructors)
                {
                    // Simplify numbers to integers.
                    foreach (var brackets in rootNode.TheTree
                        .Where(
                               node => node.Token is TypeToken token &&
                                       TypeToken.MultiValueTypes.Any(o => o == token.Content) &&
                                       node.Next is RoundBracketSyntaxNode)
                        .Select(syntaxNode => (RoundBracketSyntaxNode)syntaxNode.Next)
                        .ToList())
                    {
                        foreach (var child in brackets.Children.Where(o => o.Token is DoubleNumberToken).ToList())
                        {
                            var hasCommaPrefix = child.Previous?.Token is CommaToken;
                            var hasCommaSuffix = child.Next?.Token is CommaToken;
                            if (child.Previous == null && hasCommaSuffix ||
                                hasCommaPrefix && hasCommaSuffix ||
                                hasCommaPrefix && child.Next == null ||
                                child.Previous == null && child.Next == null)
                            {
                                if (!double.TryParse(child.Token.Content, out var asDouble))
                                    continue;

                                // vec3 (etc) can be constructed with integer params.
                                if (Math.Abs((int)asDouble - asDouble) < 0.0000001)
                                    child.ReplaceWith(new GenericSyntaxNode(new IntegerNumberToken((int)asDouble)));
                            }
                        }

                        // If all components are the same number, just list it once.
                        if (brackets.Children.Count <= 1)
                            continue;
                        var components = brackets.Children.Where(o => o.Token is not CommaToken).Select(o => o.Token?.Content).Distinct().ToList();
                        if (components.Count == 1 && brackets.Children.First()?.Token is INumberToken)
                        {
                            while (brackets.Children.Count > 1)
                                brackets.Children.Last().Remove();
                        }
                    }
                }

                if (options.SimplifyVectorReferences)
                {
                    // Simplify construction from vector .rgba components. (E.g. vec3(v.x, v.y, v.z) => v.xyz)
                    foreach (var vectorLength in new[] { 2, 3, 4 })
                    {
                        var vecType = $"vec{vectorLength}";

                        var vectorNodes = rootNode.TheTree
                            .OfType<GenericSyntaxNode>()
                            .Where(o => (o.Token as TypeToken)?.Content == vecType)
                            .Where(o => o.Next is RoundBracketSyntaxNode);
                        foreach (var vectorNode in vectorNodes)
                        {
                            while (true)
                            {
                                var didChange = false;

                                // Replace 'vec3(v.x, v.y, v.z)' with 'vec3(v.xyz)', etc.
                                var csv = ((RoundBracketSyntaxNode)vectorNode.Next).GetCsv().ToList();
                                var valuesAsVecNames = csv
                                    .Select(o => o.Count == 1 ? o.Single() as GenericSyntaxNode : null)
                                    .Select(o => o?.Token.Content)
                                    .Select(
                                            o =>
                                            {
                                                if (o == null || o.Split('.').Length != 2)
                                                    return null;
                                                var indexOfDot = o.IndexOf(".", StringComparison.Ordinal);
                                                if (indexOfDot + 1 >= o.Length || !char.IsLetter(o[indexOfDot + 1]))
                                                    return null;
                                                return o;
                                            })
                                    .ToList();

                                var newBrackets = new RoundBracketSyntaxNode();
                                for (var i = 0; i < valuesAsVecNames.Count; i++)
                                {
                                    var lhsRhs1 = valuesAsVecNames[i]?.Split('.');
                                    var lhsRhs2 = i + 1 < valuesAsVecNames.Count ? valuesAsVecNames[i + 1]?.Split('.') : null;
                                    if (lhsRhs1 == null || lhsRhs2 == null || lhsRhs1[0] != lhsRhs2[0])
                                    {
                                        // No change.
                                        newBrackets.Adopt(csv[i].ToArray());
                                        newBrackets.Adopt(new GenericSyntaxNode(new CommaToken()));
                                        continue;
                                    }

                                    // Merge vector components.
                                    var newValue = new GenericSyntaxNode($"{lhsRhs1[0]}.{lhsRhs1[1]}{lhsRhs2[1]}");
                                    newBrackets.Adopt(newValue, new GenericSyntaxNode(new CommaToken()));

                                    i++;
                                    didChange = true;
                                }

                                newBrackets.Children.LastOrDefault()?.Remove(); // Remove trailing ','.
                                vectorNode.Next.ReplaceWith(newBrackets);
                                if (!didChange)
                                    break; // Stop.
                            }
                        }
                    }

                    // Simplify vectors referencing all their own components. (I.e. v.rgba => v)
                    foreach (var node in rootNode.TheTree
                        .OfType<GenericSyntaxNode>()
                        .Where(o => (o.Token as AlphaNumToken)?.Content.Contains(".") == true)
                        .ToList())
                    {
                        var lhsRhs = node.Token.Content.Split('.');
                        if (lhsRhs.Length != 2)
                            continue;

                        // Find type of vec.
                        var vecType = node.FindVarDeclaration();
                        if (vecType == null)
                            continue; // Can't find what type of 'vec' we have.

                        // Can we simplify by removing the components?
                        var canSimplify = false;
                        if (vecType.VariableType.Content == "vec2")
                            canSimplify = new[] { "xy", "rg" }.Any(o => lhsRhs[1] == o);
                        else if (vecType.VariableType.Content == "vec3")
                            canSimplify = new[] { "xyz", "rgb" }.Any(o => lhsRhs[1] == o);
                        else if (vecType.VariableType.Content == "vec4")
                            canSimplify = new[] { "xyzw", "rgba" }.Any(o => lhsRhs[1] == o);

                        if (canSimplify)
                            node.ReplaceWith(new GenericSyntaxNode(lhsRhs[0]));
                    }
                }

                if (options.SimplifyVectorConstructors)
                {
                    // Simplify vectors with redundant copy constructor. (I.e. vec3(v) => v)
                    foreach (var vectorLength in new[] { 2, 3, 4 })
                    {
                        var vecType = $"vec{vectorLength}";

                        foreach (var vectorNode in rootNode.TheTree
                            .OfType<GenericSyntaxNode>()
                            .Where(
                                   o => (o.Token as TypeToken)?.Content == vecType &&
                                        (o.Next as RoundBracketSyntaxNode)?.Children.Count == 1))
                        {
                            // Find type of vec parameter.
                            var brackets = (RoundBracketSyntaxNode)vectorNode.Next;
                            var vecParam = brackets.Children.Single() as GenericSyntaxNode;

                            var lhsRhs = vecParam?.Token?.Content.Split('.');
                            if (lhsRhs?.Length != 2 || lhsRhs[1].Any(ch => !"rgbaxyzw".Contains(ch)))
                            {
                                var vecParamType = vecParam?.FindVarDeclaration();
                                if (vecParamType?.VariableType.Content != vecType)
                                    continue; // Can't find what type of 'vec' we have, or different types.
                            }

                            // They match - Remove the surrounding vecN keyword.
                            brackets.Remove();
                            vectorNode.ReplaceWith(vecParam);
                        }
                    }
                }

                if (options.SimplifyFunctionParams)
                {
                    var inTypes = rootNode.TheTree.Select(o => o.Token).OfType<TypeToken>().Where(o => o.InOut == TypeToken.InOutType.In);
                    inTypes.ToList().ForEach(o => o.SetInOut());
                }

                if (options.RemoveUnreachableCode)
                {
                    // Remove unreachable code after a 'return' statement.
                    foreach (var returnNode in rootNode.TheTree
                        .OfType<ReturnSyntaxNode>()
                        .Where(o => !o.HasAncestor<SwitchSyntaxNode>())
                        .Where(o => o.Next?.Next != null)
                        .ToList())
                    {
                        // Note: returnNode.Next should be a ';', which we need to keep.
                        while (returnNode.Next.Next != null)
                            returnNode.Next.Next.Remove();
                    }
                }

                if (options.CombineConsecutiveAssignments)
                {
                    // Merge consecutive assignments of the same variable.
                    var didChange = true;
                    while (didChange)
                    {
                        didChange = false;

                        foreach (var functionNode in rootNode.FindFunctionDefinitions())
                        {
                            foreach (var braces in functionNode.TheTree.OfType<BraceSyntaxNode>().ToList())
                            {
                                foreach (var localVariable in functionNode.LocalVariables())
                                {
                                    // Find consecutive assignment of same variable.
                                    var assignment = braces.Children.OfType<VariableAssignmentSyntaxNode>()
                                        .Where(o => o.HasValue && o.Parent is not VariableDeclarationSyntaxNode)
                                        .Where(o => o.Next is VariableAssignmentSyntaxNode)
                                        .Where(o => o.Name == localVariable.Name && ((VariableAssignmentSyntaxNode)o.Next).Name == localVariable.Name)
                                        .FirstOrDefault(o => o.Next.TheTree.OfType<GenericSyntaxNode>().Count(n => n.IsVarName(localVariable.Name)) == 1);

                                    if (assignment == null)
                                        continue;

                                    // Inline the definition (adding (...) if necessary).
                                    var usage = assignment.Next.TheTree.OfType<GenericSyntaxNode>().Single(o => o.IsVarName(localVariable.Name));
                                    var addBrackets = assignment.Children.Any(o => o.Token is SymbolOperatorToken);
                                    if (addBrackets)
                                        usage.ReplaceWith(new RoundBracketSyntaxNode(assignment.Children));
                                    else
                                        usage.ReplaceWith(assignment.Children.ToArray());

                                    assignment.Remove();

                                    didChange = true;

                                    if (addBrackets && assignment.Next != null)
                                    {
                                        var customOptions = CustomOptions.Disabled();
                                        customOptions.SimplifyArithmetic = true;
                                        Simplify(assignment.Next, customOptions);
                                    }
                                }
                            }
                        }
                    }
                }

                if (options.CombineAssignmentWithReturn)
                {
                    // Merge variable assignment with single use in return statement.
                    var didChange = true;
                    while (didChange)
                    {
                        didChange = false;

                        foreach (var functionNode in rootNode.FindFunctionDefinitions())
                        {
                            foreach (var braces in functionNode.TheTree.OfType<BraceSyntaxNode>().ToList())
                            {
                                foreach (var localVariable in functionNode.LocalVariables())
                                {
                                    // Single use in return statement?
                                    var returnNode = braces.FindLastChild<ReturnSyntaxNode>();
                                    if (returnNode == null)
                                        continue;

                                    var usagesWithSuffix = returnNode
                                        .TheTree
                                        .OfType<GenericSyntaxNode>()
                                        .Where(o => o.StartsWithVarName(localVariable.Name) && !o.IsVarName(localVariable.Name));
                                    if (usagesWithSuffix.Any())
                                        continue;

                                    var usages = returnNode
                                        .TheTree
                                        .OfType<GenericSyntaxNode>()
                                        .Where(o => o.IsVarName(localVariable.Name))
                                        .ToList();
                                    if (usages.Count != 1)
                                        continue;

                                    // Find the last assignment of the variable.
                                    var assignment = braces
                                        .FindLastChild<VariableAssignmentSyntaxNode>(o => o.Name.StartsWithVarName(localVariable.Name));
                                    if (assignment != null && assignment.Name != localVariable.Name)
                                        continue; // Assignment has '.' suffix part.

                                    assignment ??= localVariable;

                                    // If defined in a variable declaration, bail if it's not the last one.
                                    var assignmentDecl = assignment.Parent as VariableDeclarationSyntaxNode;
                                    if (assignmentDecl != null && assignmentDecl.Definitions.Last() != assignment)
                                        continue;

                                    // Are we assigning from a non-const global variable?
                                    // Bad idea - It might be modified by a function call in the 'return'.
                                    var globals = rootNode.FindGlobalVariables()
                                        .Where(o => (o.Parent as VariableDeclarationSyntaxNode)?.VariableType.IsConst != true)
                                        .Select(o => o.Name);
                                    if (globals.Any(g => assignment.TheTree.Any(o => o.Token?.Content?.StartsWithVarName(g) == true)))
                                        continue;

                                    // Any usages between the two locations?
                                    var assignmentLine = assignment.Parent is VariableDeclarationSyntaxNode ? assignment.Parent : assignment;
                                    var middleNodes = assignmentLine.TakeSiblingsWhile(o => o != returnNode)
                                        .SelectMany(o => o.TheTree)
                                        .Distinct();
                                    if (middleNodes.Any())
                                        continue;

                                    // Inline the definition (adding (...) if necessary).
                                    var addBrackets = assignment.Children.Any(o => o.Token is SymbolOperatorToken);
                                    if (addBrackets)
                                        usages.Single().ReplaceWith(new RoundBracketSyntaxNode(assignment.Children));
                                    else
                                        usages.Single().ReplaceWith(assignment.Children.ToArray());

                                    assignment.Remove();
                                    if (assignmentDecl?.Children.Any() == false)
                                        assignmentDecl.Remove(); // Declaration is now empty - Remove it.

                                    didChange = true;

                                    if (addBrackets)
                                    {
                                        var customOptions = CustomOptions.Disabled();
                                        customOptions.SimplifyArithmetic = true;
                                        Simplify(braces, customOptions);
                                    }

                                    {
                                        var customOptions = CustomOptions.Disabled();
                                        customOptions.RemoveUnusedVariables = true;
                                        Simplify(braces, customOptions);
                                    }
                                }
                            }
                        }
                    }
                }

                if (options.SimplifyBranching)
                {
                    // Remove 'else' keyword if the 'true' branch terminates with return/break/continue.
                    foreach (var ifElseNode in rootNode.TheTree
                        .OfType<IfSyntaxNode>()
                        .Where(
                               o => o.FalseBranch != null &&
                                    (o.TrueBranch.Children.OfType<ReturnSyntaxNode>().Any() ||
                                     o.TrueBranch.FindLastChild(n => new[] { "break", "continue" }.Contains((n as GenericSyntaxNode)?.Token?.Content)) != null))
                        .ToList())
                    {
                        var falseBranch = ifElseNode.FalseBranch;

                        // Don't inline if 'false' branch declares variables. (As their scope would change.)
                        if (falseBranch.Children.OfType<VariableDeclarationSyntaxNode>().Any())
                            continue;

                        var replacements = new List<SyntaxNode> { new IfSyntaxNode(ifElseNode.Conditions, ifElseNode.TrueBranch, null) };
                        replacements.AddRange(falseBranch.Children);
                        ifElseNode.ReplaceWith(replacements);
                    }
                }

                if (options.IntroduceMathOperators)
                {
                    // Join simple arithmetic into +=, -=, /=, *=.
                    foreach (var functionContent in rootNode.FindFunctionDefinitions().Select(o => o.Braces))
                    {
                        while (true)
                        {
                            var didChange = false;

                            foreach (var assignment in functionContent.TheTree
                                .OfType<VariableAssignmentSyntaxNode>()
                                .Where(o => o.TheTree.OfType<GenericSyntaxNode>().Any(n => n.Token.Content == o.Name)))
                            {
                                var rhs = assignment.Children.First().IsOnlyChild && assignment.Children.Single() is RoundBracketSyntaxNode bracketNode ? bracketNode.Children : assignment.Children;

                                // Must be at least three nodes (Variable name, math op, and more).
                                if (rhs.Count < 3)
                                    continue;

                                // First expression on rhs must be the variable name.
                                if ((rhs[0] as GenericSyntaxNode)?.Token.Content.StartsWithVarName(assignment.Name) != true)
                                    continue;

                                // Second expression must be a math operator.
                                var symbolType = rhs[1].Token.GetMathSymbolType();
                                if (symbolType == TokenExtensions.MathSymbolType.Unknown)
                                    continue;

                                // All maths ops should be the same type.
                                if (symbolType != TokenExtensions.MathSymbolType.AddSubtract)
                                {
                                    var symbolTypes = assignment.TheTree
                                        .Where(o => o.Token is SymbolOperatorToken)
                                        .Select(o => o.Token.GetMathSymbolType()).Distinct();
                                    if (symbolTypes.Count() != 1)
                                        continue;
                                }

                                // Replace with +=, -=, etc.
                                var newNodes = new List<SyntaxNode>
                                {
                                    new GenericSyntaxNode(assignment.Name),
                                    new GenericSyntaxNode(new SymbolOperatorToken($"{rhs[1].Token.Content}="))
                                };

                                var newRhs = rhs.Skip(2).ToList();
                                while (newRhs.Count == 1 && newRhs[0] is RoundBracketSyntaxNode && newRhs[0].Children.Any())
                                    newRhs = newRhs[0].Children.ToList();
                                newNodes.AddRange(newRhs);
                                newNodes.Add(new GenericSyntaxNode(new SemicolonToken()));
                                assignment.ReplaceWith(newNodes);

                                repeatSimplifications = didChange = true;
                            }

                            if (!didChange)
                                break;
                        }
                    }
                }

                if (options.SimplifyArithmetic)
                {
                    while (true)
                    {
                        var didChange = false;

                        // Brackets within brackets.
                        foreach (var toRemove in
                            rootNode.TheTree
                                .OfType<RoundBracketSyntaxNode>()
                                .Where(o => o.IsOnlyChild && o.Parent is RoundBracketSyntaxNode)
                                .ToList())
                        {
                            toRemove.ReplaceWith(toRemove.Children.ToArray());
                            didChange = true;
                        }

                        // Brackets containing just a number.
                        foreach (var toRemove in
                            rootNode.TheTree
                                .OfType<RoundBracketSyntaxNode>()
                                .Where(
                                       o => o.Children.Count == 1
                                            && o.Children.Single()?.Token is INumberToken
                                            && o.Parent is not FunctionCallSyntaxNode
                                            && (o.IsOnlyChild || o.Previous?.Token is SymbolOperatorToken))
                                .ToList())
                        {
                            toRemove.ReplaceWith(toRemove.Children.Single());
                            didChange = true;
                        }

                        // Assignments/return statements of the form 'return (...);'
                        foreach (var toRemove in
                            rootNode.TheTree
                                .OfType<RoundBracketSyntaxNode>()
                                .Where(
                                       o => o.IsOnlyChild
                                            && (o.Parent is ReturnSyntaxNode || o.Parent is VariableAssignmentSyntaxNode))
                                .ToList())
                        {
                            toRemove.ReplaceWith(toRemove.Children.ToArray());
                            didChange = true;
                        }

                        // Multiplication/division sequence.
                        // E.g (a * 2.0 / b) => a * 2.0 / b
                        foreach (var toRemove in
                            rootNode.TheTree
                                .OfType<RoundBracketSyntaxNode>()
                                .Where(
                                       o => o.Parent is not FunctionCallSyntaxNode &&
                                            o.Parent is not IfSyntaxNode &&
                                            o.Parent is not PragmaDefineSyntaxNode &&
                                            o.Previous?.Token is not AlphaNumToken &&
                                            o.Previous?.Token is not TypeToken &&
                                            o.Previous?.Token?.Content != "<<" &&
                                            o.Previous?.Token?.Content != ">>" &&
                                            o.Previous?.Token?.Content != "/" &&
                                            o.Next?.Token?.Content != "?" &&
                                            o.GetCsv().Count() == 1)
                                .ToList())
                        {
                            var symbols = toRemove.Children
                                .Where(o => o.Token is SymbolOperatorToken || o.Token is AssignmentOperatorToken || o.Token is EqualityOperatorToken)
                                .Select(o => o.Token.Content)
                                .Distinct()
                                .Except(new[] { "*", "/", "<", "<=", ">", ">=", "!" }); // Remove the 'safe' operators.

                            // If no symbols remain, it's safe to remove the brackets.
                            if (!symbols.Any())
                            {
                                toRemove.ReplaceWith(toRemove.Children.ToArray());
                                didChange = true;
                            }
                        }

                        if (!didChange)
                            break;

                        repeatSimplifications = true;
                    }
                }

                if (options.PerformArithmetic)
                {
                    while (true)
                    {
                        var didChange = false;

                        // 'a = b + -c' => 'a = b - c'
                        foreach (var symbolNode in rootNode.TheTree
                            .OfType<GenericSyntaxNode>()
                            .Where(o => (o.Token as SymbolOperatorToken)?.Content == "+" &&
                                        (o.Next?.Token as SymbolOperatorToken)?.Content == "-")
                            .ToList())
                        {
                            symbolNode.Remove();
                            didChange = true;
                        }

                        // 'f + -2.3' => 'f - 2.3'
                        foreach (var numNode in rootNode.TheTree
                            .OfType<GenericSyntaxNode>()
                            .Where(o => o.Token is INumberToken &&
                                        o.Token.Content.StartsWith("-") &&
                                        o.Previous?.Token.GetMathSymbolType() == TokenExtensions.MathSymbolType.AddSubtract)
                            .ToList())
                        {
                            var symbol = numNode.Previous.Token.Content[0] == '-' ? "+" : "-";
                            numNode.Previous.ReplaceWith(new GenericSyntaxNode(new SymbolOperatorToken(symbol)));
                            ((INumberToken)numNode.Token).MakePositive();
                            didChange = true;
                        }
                        
                        // 'f * 1.0' or 'f / 1.0' => 'f'
                        foreach (var oneNode in rootNode.TheTree
                            .OfType<GenericSyntaxNode>()
                            .Where(o => (o.Token as INumberToken)?.IsOne() == true &&
                                        o.Previous?.Token.GetMathSymbolType() == TokenExtensions.MathSymbolType.MultiplyDivide)
                            .ToList())
                        {
                            oneNode.Previous.Remove();
                            oneNode.Remove();
                            didChange = true;
                        }

                        // Perform simple arithmetic calculations.
                        foreach (var numNodeA in rootNode.TheTree
                            .OfType<GenericSyntaxNode>()
                            .Where(
                                   o => o.Token is INumberToken &&
                                        o.Next?.Token?.GetMathSymbolType() != TokenExtensions.MathSymbolType.Unknown &&
                                        o.Next?.Next?.Token is INumberToken)
                            .Reverse()
                            .ToList())
                        {
                            var symbolNode = numNodeA.Next;
                            var symbolType = symbolNode.Token.GetMathSymbolType();

                            if (symbolType != TokenExtensions.MathSymbolType.MultiplyDivide &&
                                numNodeA.Previous != null &&
                                numNodeA.Previous.Token?.GetMathSymbolType() != symbolType)
                            {
                                continue;
                            }

                            var numNodeB = symbolNode.Next;
                            if (numNodeB?.Next?.Token is SymbolOperatorToken &&
                                numNodeB.Next.Token.GetMathSymbolType() != TokenExtensions.MathSymbolType.AddSubtract)
                            {
                                continue;
                            }

                            var a = double.Parse(numNodeA.Token.Content);
                            var b = double.Parse(numNodeB.Token.Content);

                            var symbol = symbolNode.Token.Content[0];

                            // Invert * or / if preceded by a /.
                            // E.g. 1.2 / 2.3 * 4.5 = 1.2 / (2.3 / 4.5)
                            //                      = 1.2 / 0.51111
                            //                      = 2.3478
                            if (numNodeA.Previous?.Token?.GetMathSymbolType() == TokenExtensions.MathSymbolType.MultiplyDivide &&
                                numNodeA.Previous.Token.Content == "/")
                            {
                                symbol = symbol == '*' ? '/' : '*';
                            }

                            double c;
                            switch (symbol)
                            {
                                case '+':
                                    c = a + b;
                                    break;
                                case '-':
                                    c = a - b;
                                    break;
                                case '*':
                                    c = a * b;
                                    break;
                                case '/':
                                    c = a / b;
                                    break;
                                default:
                                    throw new InvalidOperationException($"Unrecognized math operation '{symbol}'.");
                            }

                            var isFloatResult = numNodeA.Token is DoubleNumberToken || numNodeB.Token is DoubleNumberToken;
                            numNodeB.Remove();
                            symbolNode.Remove();

                            if (isFloatResult)
                            {
                                var numberToken = Math.Abs(c) < 0.0001 && Math.Abs(c) > 0.0 ? new DoubleNumberToken(c.ToString($".#{new string('#', MaxDp - 1)}e0")) : new DoubleNumberToken(c.ToString($"F{MaxDp}"));
                                numNodeA.ReplaceWith(new GenericSyntaxNode(numberToken.Simplify()));
                            }
                            else
                            {
                                numNodeA.ReplaceWith(new GenericSyntaxNode(new IntegerNumberToken((int)c)));
                            }

                            didChange = true;
                        }

                        if (!didChange)
                            break;

                        repeatSimplifications = true;
                    }
                }
            }

            return rootNode;
        }

        /// <summary>
        /// Build a list from consecutive nodes of the specified type (skipping comments).
        /// </summary>
        private static List<T> GetGroupOfType<T>(SyntaxNode node)
        {
            var group = new List<T>();
            while (node is T match)
            {
                group.Add(match);
                node = node.Next;

                while (node is CommentSyntaxNodeBase)
                    node = node.Next;
            }

            return group;
        }
    }
}