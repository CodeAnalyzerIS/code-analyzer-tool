using Microsoft.CodeAnalysis;
using RoslynPlugin.rules;

namespace RoslynPlugin.Test.Rules;

public class MakeLocalVariableConstantTest
{

    public static TheoryData<IEnumerable<string>> ConstantVariableNotMarkedAsConstData => new()
    {
        new List<string>
        {
            @"
class C
{
    void M()
    {
        string s = ""This string stays constant"";
    }
}",
            @"
class C
{
    void M()
    {
        string s = ""This string "" + ""stays constant"";
    }
}"
        }
    };

    [Theory]
    [MemberData(nameof(ConstantVariableNotMarkedAsConstData))]
    public async Task MakeLocalVariableConstantRule_ShouldReportRuleViolation_WhenConstantVariableNotMarkedAsConst(IEnumerable<string> codeStrings)
    {
        var rule = new MakeLocalVariableConstantRule();
        foreach (var code in codeStrings)
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
    
    public static TheoryData<IEnumerable<string>> LocalVariableAssignedNewValueData => new()
    {
        new List<string>
        {
            @"
class C
{
    void M()
    {
        string nonConstantString = ""This isn't a constant string"";
        nonConstantString = ""new value"";
    }
}",
            @"
class C
{
    void M()
    {
        string nonConstantString = ""This isn't a constant string"";
        nonConstantString += ""new value"";
    }
}",
            @"
class C
{
    void M()
    {
        int nonConstantInt = 0;
        nonConstantInt += 8;
    }
}",
            @"
class C
{
    void M()
    {
        var nonConstantInt = 2;
        nonConstantInt *= 8;
    }
}",
            @"
class C
{
    void M()
    {
        int nonConstantInt = 2;
        ++nonConstantInt;
    }
}",
            @"
class C
{
    void M()
    {
        int nonConstantInt = 2;
        nonConstantInt++;
    }
}",
            @"
class C
{
    void M()
    {
        int nonConstantInt = 2;
        --nonConstantInt;
    }
}",
            @"
class C
{
    void M()
    {
        int nonConstantInt = 2;
        nonConstantInt--;
    }
}"
        }
    };
    
    [Theory]
    [MemberData(nameof(LocalVariableAssignedNewValueData))]
    public async Task MakeLocalVariableConstantRule_ShouldNotReportRuleViolation_WhenLocalVariableAssignedNewValue(IEnumerable<string> codeStrings)
    {
        foreach (var code in codeStrings)
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