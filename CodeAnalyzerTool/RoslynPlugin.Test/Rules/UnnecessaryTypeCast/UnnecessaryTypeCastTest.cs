
using RoslynPlugin.rules;

namespace RoslynPlugin.Test.Rules.UnnecessaryTypeCast;

public class UnnecessaryTypeCastTest
{
    [Theory]
    [MemberData(nameof(UnnecessaryTypeCastData.UnnecessaryCastToDerivedTypeData), MemberType = typeof(UnnecessaryTypeCastData))]
    public async Task ShouldReport_WhenUnnecessaryCastToDerivedType(IEnumerable<string> codeScenarios)
    {
        var rule = new UnnecessaryTypeCastRule();
        foreach (var code in codeScenarios)
        {
            await RuleTestRunner.ShouldReport(code, rule);
        }
    }

    [Fact]
    public async Task ShouldReport_WhenUnnecessaryCastToDerivedTypeWithConditionalAccess()
    {
        var code = @"
class C
{
    void Main()
    {
        var c = new C();

        var i = ((Derivative)c)?.I;

    }

    public int I { get; set; }
}

class Derivative : C
{
}
";
        await RuleTestRunner.ShouldReport(code, new UnnecessaryTypeCastRule());
    }

    [Fact]
    public async Task ShouldReport_WhenUnnecessaryCastToImplementedInterface()
    {
        var code = @"
using System.Collections.Generic;

class C
{
    void Main()
    {
        List<string> list = new List<string>{""hello world""};

        string s = ((IList<string>)list)[0];
    }
}
";
        await RuleTestRunner.ShouldReport(code, new UnnecessaryTypeCastRule());
    }

    [Fact]
    public async Task ShouldReport_WhenUnnecessaryCastToImplementedInterfaceWithConditionalAccess()
    {
        var code = @"
using System.Collections.Generic;

class C
{
    void Main()
    {
        List<string> list = new List<string>{""hello world""};

        string s = ((IList<string>)list)?[0];
    }
}
";
        await RuleTestRunner.ShouldReport(code, new UnnecessaryTypeCastRule());
    }


    // todo
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
        await RuleTestRunner.ShouldReport(code, new UnnecessaryTypeCastRule());
    }

    [Fact]
    public async Task ShouldNotReport_WhenPrivateProtectedOtherwiseNotAccessible()
    {
        // The private protected access modifier allows access to the member within the class and its derived classes
        // that are defined in the same assembly. However, it does not allow direct access to the member through an
        // instance of the class outside of the class hierarchy.
        // => casting to Derivative is required to be able to access the method 'PrivateProtected'
        var code = @"
class C
{
    private protected void PrivateProtected() { }
}

class Derivative : C
{
    void M(C c)
    {
        ((Derivative)c).PrivateProtected();
    }
}
";
        await RuleTestRunner.ShouldNotReport(code, new UnnecessaryTypeCastRule());
    }

    [Fact]
    public async Task ShouldNotReport_WhenCastToDerivedTypeToAccessProperty()
    {
        var code = @"
class C
{
    void Main()
    {
        var c = new C();

        var d = ((Derivative)c).I;

    }
}

class Derivative : C
{
    public int I { get; set; }
}
";
        await RuleTestRunner.ShouldNotReport(code, new UnnecessaryTypeCastRule());
    }
    
    [Fact]
    public async Task ShouldNotReport_WhenCastToDerivedTypeToAccessMethod()
    {
        var code = @"
class C
{
    void Main()
    {
        var c = new C();

        var d = ((Derivative)c).SayHello();

    }
}

class Derivative : C
{
    public void SayHello() {
        Console.WriteLine(""Hello"");
    }

}
";
        await RuleTestRunner.ShouldNotReport(code, new UnnecessaryTypeCastRule());
    }


    [Fact]
    public async Task ShouldNotReport_WhenExplicitImplementation()
    {
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/explicit-interface-implementation
        var code = @"
using System;

namespace ExampleNamespace;

class C
{
    static void M()
    {
        var ex = new ExplicitImplementationExample();
        ((IExample) ex).Method();
    }
}

interface IExample {
    void Method();
}

class ExplicitImplementationExample : IExample
{
    void IExample.Method()
    {
        Console.WriteLine(""nothing"");
    }
}
";
        
        await RuleTestRunner.ShouldNotReport(code, new UnnecessaryTypeCastRule());
    }

    [Fact]
    public async Task ShouldNotReport_WhenExplicitImplementationOfGenericMethod()
    {
        var code = @"
using System;

namespace GenericExampleNamespace;

class C
{
    static void M()
    {
        var ex = new ExplicitImplementationGenericExample();
        ((IGenericExample) ex).Method();
    }
}

interface IGenericExample {
    void Method();
}

class ExplicitImplementationGenericExample : IGenericExample
{
    void IGenericExample.Method()
    {
        Console.WriteLine(""nothing"");
    }
}
";
        await RuleTestRunner.ShouldNotReport(code, new UnnecessaryTypeCastRule());
    }
    
    
    [Fact]
    internal async Task ShouldNotReport_WhenDefaultInterfaceImplementation()
    {
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/explicit-interface-implementation
        var code = @"
public interface IPaintable
{
    void Paint() => Console.WriteLine(""Default Paint method"");
}
public class SampleClass : IPaintable
{
    // Paint() is inherited from IPaintable.

    void Main()
    {
        var sample = new SampleClass();

        ((IPaintable)sample).Paint();
    }
}
";
        await RuleTestRunner.ShouldNotReport(code, new UnnecessaryTypeCastRule());
    }
}