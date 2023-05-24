using Microsoft.CodeAnalysis;
using RoslynPlugin.rules;

namespace RoslynPlugin.Test;

public class BracesTest
{
    [Fact]
    public async void NamespaceDoesntContainGivenString()
    {
        var code = @"
namespace TestProject;
class Cat {
    private void MethodWithBraces() {
        if (true) {
            Console.WriteLine(""true is indeed true"") 
        }
    }
}
";
        var rule = new RequireBracesInBlockRule();
        rule.Options = new Dictionary<string, string>();
        rule.Severity = DiagnosticSeverity.Warning;
        
        RuleViolationVerifier verifier = new RuleViolationVerifier();
        var results = await verifier.CompileStringWithRule(code, rule);
    }
//
//     [Fact]
//     public void NamespaceContainsGivenString()
//     {
//     }
//
//     [Fact]
//     public void NoGivenString()
//     {
//     }
}