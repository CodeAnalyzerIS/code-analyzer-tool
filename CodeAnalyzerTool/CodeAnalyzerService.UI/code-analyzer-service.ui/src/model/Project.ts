import {AnalysisHistory} from "./Analysis";

export type Project = {
    id: number;
    projectName: string;
    repoUrl: string | null;
    lastAnalysisId: number;
    ruleViolationCount: number;
    lastAnalysisDate: Date
    analysisHistory: AnalysisHistory[];
    ruleViolationHistory: number[];
    ruleViolationDifference: number;
}