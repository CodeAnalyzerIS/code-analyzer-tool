namespace RoslynPlugin; 

public static class StringResources
{
    public const string PLUGIN_NAME = "Roslyn"; 
    public const string FILE_EXTENSION = ".cs";
    public const string TARGET_LANGUAGE = "C#";
    public const string RULES_FOLDER_NAME = "rules";
    public const string EXTERNAL_RULE_SEARCH_PATTERN = "*.dll";
    public const string SOLUTION_SEARCH_PATTERN = "*.sln";
    public const string NULL_COMPILATION_MSG = "Compilation was null";
    public const string NO_NAME_SPACE_OPTION_MSG = "No namespace option has been provided";
}

public static class RuleCategories
{
    public const string STYLE = "Style";
    public const string NAMING = "Naming";
    public const string MAINTAINABILITY = "Maintainability";
    public const string PERFORMANCE = "Performance";
}

public static class RuleNames
{
    public const string MAKE_LOCAL_VARIABLE_CONSTANT_RULE = "MakeLocalVariableConstant";
    public const string NAMESPACE_CONTAINS_RULE = "NamespaceContains";
    public const string REQUIRE_BRACES_IN_BLOCK_RULE = "RequireBracesInBlock";
    public const string TEST_METHOD_WITHOUT_ASSERTION_RULE = "TestWithoutAssertion";
    public const string UNNECESSARY_TYPE_CAST_RULE = "UnnecessaryTypeCast";
}