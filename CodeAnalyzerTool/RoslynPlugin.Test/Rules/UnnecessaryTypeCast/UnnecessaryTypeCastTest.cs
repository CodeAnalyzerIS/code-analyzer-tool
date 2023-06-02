
using RoslynPlugin.rules;

namespace RoslynPlugin.Test.Rules.UnnecessaryTypeCast;

public class UnnecessaryTypeCastTest
{
    // ------------------------------------ Should report --------------------------------------------------------------
    [Theory]
    [MemberData(nameof(UnnecessaryTypeCastData.CastToDerivedTypeToAccessAlreadyAccessibleMethod), MemberType = typeof(UnnecessaryTypeCastData))]
    public async Task ShouldReport_WhenCastToDerivedTypeToAccessAlreadyAccessibleMethod(IEnumerable<string> codeScenarios)
    {
        var rule = new UnnecessaryTypeCastRule();
        foreach (var code in codeScenarios)
            await RuleTestRunner.ShouldReport(code, rule);
        
    }

    [Fact]
    public async Task ShouldReport_WhenCastToDerivedTypeWithConditionalAccess()
    {
        const string code = @"
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
    public async Task ShouldReport_WhenCastToImplementedInterface()
    {
        const string code = @"
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
    public async Task ShouldReport_WhenCastToImplementedInterfaceWithConditionalAccess()
    {
        const string code = @"
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

    
    [Fact]
    public async Task ShouldReport_WhenCastToNestedDerivedTypeToAccessAlreadyAccessibleMethod()
    {
        const string code = @"
class C
{
    private void Main(C c)
    {
        ((NestedDerivedType)c).Protected(); // unnecessary #1

        ((NestedDerivedType)c).PrivateProtected(); // unnecessary #2

        ((NestedDerivedType)c).ProtectedInternal(); // unnecessary #3
    }

    private class NestedDerivedType : C
    {
        private void Main2(C c)
        {
            ((NestedDerivedType)c).Protected(); // unnecessary #4

            ((NestedDerivedType)c).PrivateProtected(); // unnecessary #5

            ((NestedDerivedType)c).ProtectedInternal(); // unnecessary #6
        }
    }

    protected void Protected() { }

    private protected void PrivateProtected() { }

    protected internal void ProtectedInternal() { }
}
";
        await RuleTestRunner.ShouldReport(code, new UnnecessaryTypeCastRule(), 6);
    }

    // ------------------------------------ Should not report ----------------------------------------------------------

    [Theory]
    [MemberData(nameof(UnnecessaryTypeCastData.AccessingProtectedMethod), MemberType = typeof(UnnecessaryTypeCastData))]
    public async Task ShouldNotReport_WhenProtectedMethodOtherwiseNotAccessible(IEnumerable<string> codeScenarios)
    {
        // The (private) protected access modifier allows access to the member within the class and its derived classes
        // (that are defined in the same assembly). However, it does not allow direct access to the member through an
        // instance of the class outside of the class, even if it's inside a derived Type.
        // => casting to Derivative is required to be able to access the method '(Private)Protected'
        var rule = new UnnecessaryTypeCastRule();
        foreach (var code in codeScenarios)
            await RuleTestRunner.ShouldNotReport(code, rule);
    }

    [Fact]
    public async Task ShouldNotReport_WhenCastToDerivedTypeToAccessProperty()
    {
        const string code = @"
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
        const string code = @"
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
    public async Task ShouldNotReport_WhenExplicitImplementationOfMethod()
    {
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/explicit-interface-implementation
        const string code = @"
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
    public async Task ShouldNotReport_WhenExplicitImplementationOfProperty()
    {
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/explicit-interface-implementation
        const string code = @"
using System;

namespace ExampleNamespace;

class C
{
    static void M()
    {
        var ex = new ExplicitImplementationExample();
        var test = ((IExample)ex).PropExample;
    }
}

interface IExample {
    int PropExample { get; set; }
}

class ExplicitImplementationExample : IExample
{
    int IExample.PropExample { get; set; }
}
";
        
        await RuleTestRunner.ShouldNotReport(code, new UnnecessaryTypeCastRule());
    }

    [Fact]
    public async Task ShouldNotReport_WhenExplicitImplementationOfGenericMethod()
    {
        const string code = @"
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
        const string code = @"
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