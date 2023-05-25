using Microsoft.CodeAnalysis;
using RoslynPlugin.rules;

namespace RoslynPlugin.Test.Rules;

public class RequireBracesInBlockTest
{
    [Fact]
    public async void RequiredBracesInBlockRule_ShouldReportRuleViolation_WhenIfStatementWithoutBracesExist()
    {
        var code = @"
class C {
    private void M() {
        if (true) Console.WriteLine(""true is indeed true"") 
    }
}
";
        var rule = new RequireBracesInBlockRule();
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().Contain(rv => rv.Rule.RuleName == RuleNames.REQUIRE_BRACES_IN_BLOCK_RULE);
    }
    
    [Fact]
    public async void RequiredBracesInBlockRule_ShouldNotReportRuleViolation_WhenIfStatementWithoutBracesDoesNotExist()
    {
        var code = @"
class C {
    private void M() {
        if (true) {
            Console.WriteLine(""true is indeed true"")
        }
    }
}
";
        var rule = new RequireBracesInBlockRule();
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().BeEmpty();
    }
}