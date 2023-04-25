using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace PluginTest1
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TestMethodWithoutAssertionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NoAssertion";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.NoAssertionTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.NoAssertionMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.NoAssertionDescription), Resources.ResourceManager,
                typeof(Resources));

        private const string Category = "Syntax";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
            Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (!IsTestMethod(methodDeclaration))
            {
                return;
            }

            if (ContainsAssertion(methodDeclaration)) return;
            var diagnostic = Diagnostic.Create(Rule, methodDeclaration.GetFirstToken().GetLocation(),
                methodDeclaration.Identifier.ValueText);
            context.ReportDiagnostic(diagnostic);
        }
        
        private bool IsTestMethod(MethodDeclarationSyntax methodDeclaration)
        {
            var attributes = methodDeclaration.AttributeLists.SelectMany(al => al.Attributes);
            return attributes.Any(a => a.Name.ToString().StartsWith("Test"));
        }

        private bool ContainsAssertion(MethodDeclarationSyntax methodDeclaration)
        {
            var assertionExpressions = methodDeclaration.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Select(mae => mae.Expression.ToString());

            return assertionExpressions.Any(ae => ae.StartsWith("Assert."));
        }
    }
}