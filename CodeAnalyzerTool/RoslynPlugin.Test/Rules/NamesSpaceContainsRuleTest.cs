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
        await RuleTestRunner.ShouldReport(code, rule);
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
        await RuleTestRunner.ShouldNotReport(code, rule);
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
        await RuleTestRunner.ShouldNotReport(code, rule);
    }
}