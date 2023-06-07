export type Rule = {
    id: number;
    ruleName: string;
    title: string;
    description: string;
    category: string;
    pluginName: string;
    targetLanguage: string;
    isEnabledByDefault: string;
    defaultSeverity: string;
    codeExample: string | null;
    codeExampleFix: string | null;
}