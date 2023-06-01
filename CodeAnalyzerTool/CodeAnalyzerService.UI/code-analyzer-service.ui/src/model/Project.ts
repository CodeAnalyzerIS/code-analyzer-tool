import {AnalysisHistory} from "./Analysis";

export type Project = {
    id: number;
    projectName: string;
    lastAnalysisId: number;
    ruleViolationCount: number;
    analysisHistory: AnalysisHistory[];
    ruleViolationHistory: number[];
    ruleViolationDifference: number;
}