using Microsoft.CodeAnalysis;
using RoslynPlugin.rules;

namespace RoslynPlugin.Test.Rules;

public class NamesSpaceContainsRuleTest
{
    [Fact]
    public async void ShouldReport_WhenNamespaceThatDoesNotContainStringExists()
    {
        var code = @"
namespace Animals.Mammals.Felines {
    class Cat {
        void M() { 
            Console.WriteLine(""void"");
        }
    }
}";
        var rule = new NamespaceContainsRule();
        rule.Options = new Dictionary<string, string> { [NamespaceContainsRule.NAMESPACE_OPTION_KEY] = "InfoSupport" };
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().Contain(rv => rv.Rule.RuleName == RuleNames.NAMESPACE_CONTAINS_RULE);
    }

    [Fact]
    public async void ShouldNotReport_WhenEveryNamespaceContainsString()
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
        rule.Options = new Dictionary<string, string> { [NamespaceContainsRule.NAMESPACE_OPTION_KEY] = "InfoSupport" };
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().BeEmpty();
    }

    [Fact]
    public async void ShouldNotReport_WhenNoStringOptionGiven()
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
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().BeEmpty();
    }
}