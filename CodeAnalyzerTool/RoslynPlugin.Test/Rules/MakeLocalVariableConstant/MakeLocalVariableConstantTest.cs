using RoslynPlugin.rules;

namespace RoslynPlugin.Test.Rules.MakeLocalVariableConstant;

public class MakeLocalVariableConstantTest
{
    [Theory]
    [MemberData(nameof(MakeLocalVariableConstantData.ConstantVariableNotMarkedAsConstData), MemberType = typeof(MakeLocalVariableConstantData))]
    public async Task MakeLocalVariableConstantRule_ShouldReportRuleViolation_WhenConstantVariableNotMarkedAsConst(IEnumerable<string> codeScenarios)
    {
        var rule = new MakeLocalVariableConstantRule();
        foreach (var code in codeScenarios)
        {
            var results = await RuleTestRunner.CompileStringWithRule(code, rule);
            results.Should().Contain(rv => rv.Rule.RuleName == RuleNames.MAKE_LOCAL_VARIABLE_CONSTANT_RULE);
        }
    }

    [Fact]
    public async Task MakeLocalVariableConstantRule_ShouldReportRuleViolation_WhenConstantVariableDeclaredWithVarKeywordNotMarkedAsConst()
    {
        var code = @"
class C
{
    void M()
    {
        var s = ""This string stays constant"";
    }
}";
        var rule = new MakeLocalVariableConstantRule();
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().Contain(rv => rv.Rule.RuleName == RuleNames.MAKE_LOCAL_VARIABLE_CONSTANT_RULE);
    }

    [Theory]
    [MemberData(nameof(MakeLocalVariableConstantData.LocalVariableAssignedNewValueData), MemberType = typeof(MakeLocalVariableConstantData))]
    public async Task MakeLocalVariableConstantRule_ShouldNotReportRuleViolation_WhenLocalVariableAssignedNewValue(IEnumerable<string> codeScenarios)
    {
        foreach (var code in codeScenarios)
        {
            var rule = new MakeLocalVariableConstantRule();
            var results = await RuleTestRunner.CompileStringWithRule(code, rule);
            results.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task MakeLocalVariableConstantRule_ShouldNotReportRuleViolation_WhenVariablePassedAsRefParameter()
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
        var rule = new MakeLocalVariableConstantRule();
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task MakeLocalVariableConstantRule_ShouldNotReportRuleViolation_WhenVariablePassedAsRefParameterThroughExtensionMethod()
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
        var rule = new MakeLocalVariableConstantRule();
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().BeEmpty();
    }
}