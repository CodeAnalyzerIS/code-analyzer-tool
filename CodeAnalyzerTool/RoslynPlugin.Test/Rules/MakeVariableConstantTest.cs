namespace RoslynPlugin.Test.Rules;

public class MakeVariableConstantTest
{

    [Fact]
    public async Task MakeVariableConstant_ShouldReportRuleViolation_WhenConstantVariableNotMarkedAsConst()
    {
        var code = @"
class C
{
    void M()
    {
        string s = ""This string stays constant"";
    }
}";
    }

    [Fact]
    public async Task MakeVariableConstant_ShouldReportRuleViolation_WhenConstantVariableDeclaredWithVarKeywordNotMarkedAsConst()
    {
        var code = @"
class C
{
    void M()
    {
        var s = ""This string stays constant"";
    }
}";
        
    }

    [Fact]
    public async Task MakeVariableConstant_ShouldReportRuleViolation_WhenInterpolatedStringContainsNoVariables()
    {
        var code = @"
class C
{
    void M()
    {
        string s = $""This is a string with interpolation but without variables"";
    }
}";
    }
    
    [Fact]
    public async Task MakeVariableConstant_ShouldReportRuleViolation_WhenInterpolatedStringContainsOnlyConstants()
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
    }
    
    [Fact]
    public async Task MakeVariableConstant_ShouldNotReportRuleViolation_WhenInterpolatedStringContainsNonConstants()
    {
        var code = @"
class C
{
    void M()
    {
        const string constantString = ""This is a constant string"";
        string nonConstantString = ""This isn't a constant string"";
        string s = $""This is an interpolated string with non-constant variables: {nonConstantString} {constantString}"";
    }
}";
    }

    [Fact]
    public async Task MakeVariableConstant_ShouldNotReportRuleViolation_WhenVariablePassedAsRefParameter()
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
    }

    [Fact]
    public async Task MakeVariableConstant_ShouldNotReportRuleViolation_WhenVariablePassedAsRefParameterThroughExtensionMethod()
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
    }
}