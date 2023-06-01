namespace RoslynPlugin.Test.Rules.MakeLocalVariableConstant;

public class MakeLocalVariableConstantRuleData
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
}