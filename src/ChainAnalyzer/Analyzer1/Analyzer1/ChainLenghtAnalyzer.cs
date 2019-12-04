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
    public class ChainLenghtAnalyzer : DiagnosticAnalyzer
    {
        public const int MaxChainLength = 5;
        public const string DiagnosticId = "CHAIN001";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.ChainLengthAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.ChainLengthAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.ChainLengthAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Readablity";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(VariableDeclarationChainLength, SyntaxKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(VariableDeclarationChainLength, SyntaxKind.ExpressionStatement);
        }

        private void VariableDeclarationChainLength(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            var syntaxTokens = node.DescendantTokens().Where(t => t.ValueText == "." && !IsInArgumentTree(t) );
            if (syntaxTokens.Count() <= MaxChainLength)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }

        private bool IsInArgumentTree(SyntaxToken token)
        {
            var node = token.Parent;
            while (true)
            {
                if (node.GetType() == typeof(ArgumentSyntax))
                {
                    return true;
                }
                else if (node.Parent == null)
                {
                    return false;
                }
                else
                {
                    node = node.Parent;
                }
            }
        }
    }
}
