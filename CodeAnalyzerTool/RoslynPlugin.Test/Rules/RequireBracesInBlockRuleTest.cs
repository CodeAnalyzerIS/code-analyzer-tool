using RoslynPlugin.rules;

namespace RoslynPlugin.Test.Rules;

public class RequireBracesInBlockRuleTest
{
    [Fact]
    public async void ShouldReport_WhenIfStatementWithoutBracesExist()
    {
        const string code = @"
class C {
    private void M() {
        if (true) Console.WriteLine(""true is indeed true"") 
    }
}
";
        await RuleTestRunner.ShouldReport(code, new RequireBracesInBlockRule());
    }
    
    [Fact]
    public async void ShouldNotReport_WhenIfStatementWithoutBracesDoesNotExist()
    {
        const string code = @"
class C {
    private void M() {
        if (true) {
            Console.WriteLine(""true is indeed true"")
        }
    }
}
";
        await RuleTestRunner.ShouldNotReport(code, new RequireBracesInBlockRule());
    }
}