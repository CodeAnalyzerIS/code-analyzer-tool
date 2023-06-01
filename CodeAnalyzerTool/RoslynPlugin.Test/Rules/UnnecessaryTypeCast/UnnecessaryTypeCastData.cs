namespace RoslynPlugin.Test.Rules.UnnecessaryTypeCast;

public class UnnecessaryTypeCastData
{
    public static TheoryData<IEnumerable<string>> UnnecessaryCastToDerivedTypeData => new()
    {
        new List<string>
        {
            @"
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
",
            @"
class DerivedType : C
{
    public static void M()
    {
        var c = default(C);

        ((DerivedType)c).ProtectedInternal();
    }
}

class C
{
    protected internal void ProtectedInternal() { }
}
"
        }
    };
}