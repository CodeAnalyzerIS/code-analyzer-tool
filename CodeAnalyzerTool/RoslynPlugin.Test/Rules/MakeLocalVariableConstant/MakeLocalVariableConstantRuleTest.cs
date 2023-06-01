using RoslynPlugin.rules;

namespace RoslynPlugin.Test.Rules.MakeLocalVariableConstant;

public class MakeLocalVariableConstantRuleTest
{
    [Theory]
    [MemberData(nameof(MakeLocalVariableConstantRuleData.ConstantVariableNotMarkedAsConstData), MemberType = typeof(MakeLocalVariableConstantRuleData))]
    public async Task ShouldReport_WhenConstantVariableNotMarkedAsConst(IEnumerable<string> codeScenarios)
    {
        var rule = new MakeLocalVariableConstantRule();
        foreach (var code in codeScenarios)
        {
            await RuleTestRunner.ShouldReport(code, rule);
        }
    }

    [Fact]
    public async Task ShouldReport_WhenConstantVariableDeclaredWithVarKeywordNotMarkedAsConst()
    {
        var code = @"
class C
{
    void M()
    {
        var s = ""This string stays constant"";
    }
}";
        await RuleTestRunner.ShouldReport(code, new MakeLocalVariableConstantRule());
    }

    [Theory]
    [MemberData(nameof(MakeLocalVariableConstantRuleData.LocalVariableAssignedNewValueData), MemberType = typeof(MakeLocalVariableConstantRuleData))]
    public async Task ShouldNotReport_WhenLocalVariableAssignedNewValue(IEnumerable<string> codeScenarios)
    {
        var rule = new MakeLocalVariableConstantRule();
        foreach (var code in codeScenarios)
        {
            await RuleTestRunner.ShouldNotReport(code, rule);
        }
    }

    [Fact]
    public async Task ShouldNotReport_WhenVariablePassedAsRefParameter()
    {
        var code = @"
class C
{
    void M(int p)
    {
        int x = 0;
        Add(ref x, p);
    }

    void Add(ref int p1, int p2)
    {
        p1 += p2;
    }
}";
        await RuleTestRunner.ShouldNotReport(code, new MakeLocalVariableConstantRule());
    }

    [Fact]
    public async Task ShouldNotReport_WhenVariablePassedAsRefParameterThroughExtensionMethod()
    {
        var code = @"
public static class C
{
    static void M(int p)
    {
        int x = 0;
        x.Foo(p);
    }
    public static int Foo(this ref int p1, int p2)
    {
        return p1 + p2;
    }
}";
        await RuleTestRunner.ShouldNotReport(code, new MakeLocalVariableConstantRule());
    }
}