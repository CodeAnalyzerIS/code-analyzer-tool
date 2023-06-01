using RoslynPlugin.rules;

namespace RoslynPlugin.Test.Rules;

public class UnnecessaryTypeCastTest
{
    private async Task ShouldReport(string code)
    {
        var rule = new UnnecessaryTypeCastRule();
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().Contain(rv => rv.Rule.RuleName == RuleNames.UNNECESSARY_TYPE_CAST_RULE);
    }

    private async Task ShouldNotReport(string code)
    {
        var rule = new UnnecessaryTypeCastRule();
        var results = await RuleTestRunner.CompileStringWithRule(code, rule);
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReport_WhenCastToDerivedType()
    {
        var code = @"
class C
{
    void M()
    {
        var c = new C();

        var s = ((DerivesFromC)c).I;

    }

    public int I { get; set; }
}

class DerivesFromC : C
{
}
";
        await ShouldReport(code);
    }

    [Fact]
    public async Task ShouldReport_WhenCastToDerivedTypeWithConditionalAccess()
    {
        var code = @"
class C
{
    void M()
    {
        var c = new C();

        var s = ((D)c)?.I;

    }

    public int I { get; set; }
}

class D : C
{
}
";
        await ShouldReport(code);
    }

    [Fact]
    public async Task ShouldReport_WhenCastToImplementedInterface()
    {
        var code = @"
using System.Collections.Generic;

class C
{
    void M()
    {
        List<string> list = new List<string>{""hello world""};

        string s = ((IList<string>)list)[0];
    }
}
";
        await ShouldReport(code);
    }

    [Fact]
    public async Task
        ShouldReport_WhenCastToImplementedInterfaceWithConditionalAccess()
    {
        var code = @"
using System.Collections.Generic;

class C
{
    void M()
    {
        List<string> list = new List<string>{""hello world""};

        string s = ((IList<string>)list)?[0];
    }
}
";
        await ShouldReport(code);
    }


    [Fact]
    public async Task ShouldReport_WhenTODO()
    {
        var code = @"
class B
{
    private void M(B b)
    {
        ((C)b).Protected();

        ((C)b).PrivateProtected();

        ((C)b).ProtectedInternal();
    }

    private class C : B
    {
        private void M2(B b)
        {
            ((C)b).Protected();

            ((C)b).PrivateProtected();

            ((C)b).ProtectedInternal();
        }
    }

    protected void Protected() { }

    private protected void PrivateProtected() { }

    protected internal void ProtectedInternal() { }
}
";
        await ShouldReport(code);
    }

    [Fact]
    public async Task ShouldReport_WhenTODO2()
    {
        var code = @"
class C : B
{
    public static void M()
    {
        var b = default(B);

        ((C)b).ProtectedInternal();
    }
}

class B
{
    protected internal void ProtectedInternal() { }
}
";
        await ShouldReport(code);
    }

    [Fact]
    public async Task ShouldNotReport_WhenPrivateProtectedOtherwiseNotAccesible()
    {
        var code = @"
class B
{
    private protected void PrivateProtected() { }
}

class C : B
{
    void M(B b)
    {
        ((C)b).PrivateProtected();
    }
}
";
        await ShouldNotReport(code);
    }

    [Fact]
    public async Task ShouldNotReport_WhenCastToDerivedType()
    {
        var code = @"
class C
{
    void M()
    {
        var c = new C();

        var s = ((B)c).P;

    }
}

class B : C
{
    public string P { get; set; }
}
";
        await ShouldNotReport(code);
    }
}