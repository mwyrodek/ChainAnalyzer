using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using ChainAnalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
namespace ChainAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ChainReturnTypeAnalyzer : DiagnosticAnalyzer
    {
        public const int MaxReturnTypes = 2;
        public const string DiagnosticId = "CHAIN002";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.ChainReturnTypesAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.ChainReturnTypesAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.ChainReturnTypesAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Readablity";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSemanticModelAction(Semantic);
        }

        //based on https://stackoverflow.com/questions/29176579/get-a-list-of-referenced-types-within-a-project-with-roslyn
        private static void Semantic(SemanticModelAnalysisContext context)
        {
            var root = context.SemanticModel.SyntaxTree.GetRoot();
            var nodes = root.DescendantNodes(n => true);
            
            var st = root.SyntaxTree;
            var sm = context.SemanticModel.Compilation.GetSemanticModel(st);
            if(nodes!=null)
            {
                var syntaxNodes = nodes as SyntaxNode[] ?? nodes.ToArray();

                foreach (var node in syntaxNodes)
                {
                    var x = node.GetType();
                    if(node.GetType() != typeof(LocalDeclarationStatementSyntax) && node.GetType() != typeof(ExpressionStatementSyntax))
                    {
                        continue;
                    }
                    var invocations = node.DescendantNodesAndSelf().Where(n => !IsInArgumentTree(n))
                        .Select(ma => sm.GetTypeInfo(ma).Type)
                        .OfType<INamedTypeSymbol>()
                        .Distinct()
                        .ToArray();
                    
                    if (invocations.Count() > MaxReturnTypes) 
                    {                        
                        context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
                    }
                }
            }
        }
        private static bool IsInArgumentTree(SyntaxNode node)
        {
            var nodeParent = node.Parent;
            while (true)
            {
                if (nodeParent.GetType() == typeof(ArgumentSyntax))
                {
                    return true;
                }
                else if (nodeParent.Parent == null)
                {
                    return false;
                }
                else
                {
                    nodeParent = nodeParent.Parent;
                }
            }
        }
    }
}
