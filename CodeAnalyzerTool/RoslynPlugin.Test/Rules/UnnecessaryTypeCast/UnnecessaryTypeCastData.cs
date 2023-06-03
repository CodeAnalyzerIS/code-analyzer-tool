namespace RoslynPlugin.Test.Rules.UnnecessaryTypeCast;

public class UnnecessaryTypeCastData
{
    public static TheoryData<IEnumerable<string>> CastToDerivedTypeToAccessAlreadyAccessibleMember => new()
    {
        new List<string>
        {
            @"
class C
{
    void Main()
    {
        var c = new C();

        var i = ((DerivedType)c).I;

    }

    public int I { get; set; }
}

class DerivedType : C
{
}",
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
}"
        }
    };
    
    public static TheoryData<IEnumerable<string>> ProtectedMemberOtherwiseNotAccessible => new()
    {
        new List<string>
        {
            @"
class C
{
    protected void Protected() { }
}

class DerivedType : C
{
    void M(C c)
    {
        ((DerivedType)c).Protected();
    }
}
",
            @"
class C
{
    private protected void PrivateProtected() { }
}

class DerivedType : C
{
    void M(C c)
    {
        ((DerivedType)c).PrivateProtected();
    }
}
",
            @"
class C
{
    private protected int PrivateProtectedNumber { get; set; }
}

class DerivedType : C
{
    void M(C c)
    {
        var test = ((DerivedType)c).PrivateProtectedNumber;
    }
}",
            @"
class C
{
    protected int ProtectedNumber { get; set; }
}

class DerivedType : C
{
    void M(C c)
    {
        var test = ((DerivedType)c).ProtectedNumber;
    }
}"
        }
    };
}