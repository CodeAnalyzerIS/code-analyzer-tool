using RoslynPlugin.rules;
using Microsoft.CodeAnalysis.Text;

namespace RoslynPlugin.Test;

public class NamesSpaceContainsTest
{
//     [Fact]
//     public async void NamespaceDoesntContainGivenString()
//     {
//         var code = @"
// namespace Animals.Mammals.Felines;
// class Cat { }
// ";
//         var rule = new NamespaceContainsRule();
//         rule.Options = new Dictionary<string, string>
//             { [NamespaceContainsRule.NAMESPACE_OPTION_KEY] = "Animals.Mammals" };
//         
//         RuleViolationVerifier verifier = new RuleViolationVerifier();
//         var results = await verifier.CompileStringWithRule(code, rule);
//     }

    [Fact]
    public async void NamespaceDoesntContainGivenString()
    {
        var code = SourceText.From(@"
namespace Animals.Mammals.Felines;
class Cat {
    void DoNothing() { 
        if(true) Console.WriteLine(""Testing"")
    }
}
");

        var actual = await code.GetDiagnostics<RequireBracesInBlockRule>();
        
        actual.ShouldContainDiagnosticWithId("RequireBracesInBlockz");
        
        // var rule = new NamespaceContainsRule();
        // rule.Options = new Dictionary<string, string>
        //     { [NamespaceContainsRule.NAMESPACE_OPTION_KEY] = "Animals.Mammals" };
        //
        // RuleViolationVerifier verifier = new RuleViolationVerifier();
        // var results = await verifier.CompileStringWithRule(code, rule);
    }

    [Fact]
    public void NamespaceContainsGivenString()
    {
    }

    [Fact]
    public void NoGivenString()
    {
    }
}