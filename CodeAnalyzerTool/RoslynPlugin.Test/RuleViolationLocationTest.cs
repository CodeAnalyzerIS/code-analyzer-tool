using RoslynPlugin.rules;

namespace RoslynPlugin.Test;

public class RuleViolationLocationTest
{
    [Fact]
    public async void LocationShouldBeRelative()
    {
        const string code = @"
namespace Animals.Mammals.Felines {
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
        
        results.Should().Contain(rv => rv.Location.Path == "net7.0\\SourceUnderTest.cs");
    }
}