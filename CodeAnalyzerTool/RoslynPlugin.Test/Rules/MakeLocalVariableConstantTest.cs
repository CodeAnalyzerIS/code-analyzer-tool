using Microsoft.CodeAnalysis;
using RoslynPlugin.rules;

namespace RoslynPlugin.Test.Rules;

public class MakeLocalVariableConstantTest
{

    [Fact]
    public async Task MakeLocalVariableConstantRule_ShouldReportRuleViolation_WhenConstantVariableNotMarkedAsConst()
    {
        var code = @"
class C
{
    void M()
    {
        string s = ""This string stays constant"";
    }
}";
        
        var rule = new MakeLocalVariableConstantRule();
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().Contain(rv => rv.Rule.RuleName == RuleNames.MAKE_LOCAL_VARIABLE_CONSTANT_RULE);
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

    [Fact]
    public async Task MakeLocalVariableConstantRule_ShouldReportRuleViolation_WhenInterpolatedStringContainsNoVariables()
    {
        var code = @"
class C
{
    void M()
    {
        string s = $""This is a string with interpolation but without variables"";
    }
}";
        var rule = new MakeLocalVariableConstantRule();
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().Contain(rv => rv.Rule.RuleName == RuleNames.MAKE_LOCAL_VARIABLE_CONSTANT_RULE);
    }
    
    [Fact]
    public async Task MakeLocalVariableConstantRule_ShouldReportRuleViolation_WhenInterpolatedStringContainsOnlyConstants()
    {
        var code = @"
class C
{
    void M()
    {
        const string constantString = ""This is a constant string"";
        string s = $""This is a string with interpolation with solely constants: {constantString}"";
    }
}";
        var rule = new MakeLocalVariableConstantRule();
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().Contain(rv => rv.Rule.RuleName == RuleNames.MAKE_LOCAL_VARIABLE_CONSTANT_RULE);
    }
    
    [Fact]
    public async Task MakeLocalVariableConstantRule_ShouldNotReportRuleViolation_WhenLocalVariableAssignedNewValue()
    {
        var code = @"
class C
{
    void M()
    {
        string nonConstantString = ""This isn't a constant string"";
        nonConstantString = ""new value"";

        string nonConstantString2 = ""This isn't a constant string"";
        nonConstantString2 += ""new value"";
    }
}";
        var rule = new MakeLocalVariableConstantRule();
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().BeEmpty();
    }
    
    [Fact]
    public async Task MakeLocalVariableConstantRule_ShouldNotReportRuleViolation_WhenInterpolatedStringContainsNonConstants()
    {
        var code = @"
class C
{
    void M()
    {
        const string constantString = ""This is a constant string"";
        string nonConstantString = ""This isn't a constant string"";
        nonConstantString += ""."";
        string s = $""This is an interpolated string with non-constant variables: {nonConstantString} {constantString}"";
    }
}";
        var rule = new MakeLocalVariableConstantRule();
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().BeEmpty();
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
        x.Add(p);
    }
    public static void Add(this ref int p1, int p2)
    {
        p1 += p2;
    }
}";
        var rule = new MakeLocalVariableConstantRule();
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().BeEmpty();
    }
}