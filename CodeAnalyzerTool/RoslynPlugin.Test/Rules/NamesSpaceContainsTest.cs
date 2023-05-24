using Microsoft.CodeAnalysis;
using RoslynPlugin.rules;
using Microsoft.CodeAnalysis.Text;

namespace RoslynPlugin.Test;

public class NamesSpaceContainsTest
{
    [Fact]
    public async void NamespaceContainsRule_ShouldReportRuleViolation_WhenNamespaceThatDoesNotContainStringExists()
    {
        var code = @"
namespace Animals.Mammals.Felines {
    class Cat {
        void M() { 
            Console.WriteLine(""void"");
        }
    }
}
";
        var rule = new NamespaceContainsRule();
        rule.Severity = DiagnosticSeverity.Warning;
        rule.Options = new Dictionary<string, string> { [NamespaceContainsRule.NAMESPACE_OPTION_KEY] = "InfoSupport" };
        
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().Contain(rv => rv.Rule.RuleName == RuleNames.NAMESPACE_CONTAINS_RULE);
    }

    [Fact]
    public async void NamespaceContainsRule_ShouldNotReportRuleViolation_WhenEveryNamespaceContainsString()
    {
        var code = @"
namespace InfoSupport.Animals.Mammals.Felines {
    class Cat {
        void M() { 
            Console.WriteLine(""void"");
        }
    }
}
";
        var rule = new NamespaceContainsRule();
        rule.Severity = DiagnosticSeverity.Warning;
        rule.Options = new Dictionary<string, string> { [NamespaceContainsRule.NAMESPACE_OPTION_KEY] = "InfoSupport" };
        
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().BeEmpty();
    }

    [Fact]
    public async void NamespaceContainsRule_ShouldNotReportRuleViolation_WhenNoStringOptionGiven()
    {
        var code = @"
namespace InfoSupport.Animals.Mammals.Felines {
    class Cat {
        void M() { 
            Console.WriteLine(""void"");
        }
    }
}
";
        var rule = new NamespaceContainsRule();
        rule.Severity = DiagnosticSeverity.Warning;
        rule.Options = new Dictionary<string, string>();
        
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().BeEmpty();
    }
}