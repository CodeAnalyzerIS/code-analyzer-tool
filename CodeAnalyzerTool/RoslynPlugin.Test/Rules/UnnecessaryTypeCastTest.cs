
using RoslynPlugin.rules;

namespace RoslynPlugin.Test.Rules;

public class UnnecessaryTypeCastTest
{
    [Fact]
    public async Task ShouldReport_WhenUnnecessaryCastToDerivedType()
    {
        var code = @"
class C
{
    void Main()
    {
        var c = new C();

        var i = ((Derivative)c).I;

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
        await RuleTestRunner.ShouldReport(code, new UnnecessaryTypeCastRule());
    }

    [Fact]
    public async Task ShouldNotReport_WhenPrivateProtectedOtherwiseNotAccesible()
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
        Console.Writeline(""Hello"");
    }

}
";
        await RuleTestRunner.ShouldNotReport(code, new UnnecessaryTypeCastRule());
    }
    
    
    
    // ============================================================================================
    
//     [Fact]
//     public async Task Test_CastToIDisposable()
//     {
//         var code = @"
// using System;
// using System.Collections.Generic;
//
// class C
// {
//     void M()
//     {
//         ((IDisposable)new Disposable()).Dispose();
//     }
// }
//
// class Disposable : IDisposable
// {
//     public void Dispose()
//     {
//         throw new NotImplementedException();
//     }
// }
// ";
//         
//         await RuleTestRunner.ShouldNotReport(code, new UnnecessaryTypeCastRule());
//     }
    
    
    
    [Fact]
    public async Task TestNoDiagnostic_ExplicitImplementation()
    {
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/explicit-interface-implementation
        var code = @"
using System;
using System.Collections;
using System.Collections.Generic;

class C
{
    static void M()
    {
        var e1 = ((IEnumerable<string>)new EnumerableOfString()).GetEnumerator();
    }
}

class EnumerableOfString : IEnumerable<string>
{
    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator<string> IEnumerable<string>.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}

class DerivedEnumerableOfString : EnumerableOfString
{
}

class ExplicitDisposable : IDisposable
{
    void IDisposable.Dispose()
    {
        throw new NotImplementedException();
    }
}
";
        
        await RuleTestRunner.ShouldNotReport(code, new UnnecessaryTypeCastRule());
    }

    [Fact]
    public async Task TestNoDiagnostic_ExplicitImplementationOfGenericMethod()
    {
        var code = @"
interface IC
{
    void M<T>(T t);
}

class C : IC
{
    void M<T>(T t)
    {
        var c = new C();

        ((IC)c).M(c);
    }
}
";
        await RuleTestRunner.ShouldNotReport(code, new UnnecessaryTypeCastRule());
    }
    
    
    [Fact]
    internal async Task TestNoDiagnostic_DefaultInterfaceImplementation()
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