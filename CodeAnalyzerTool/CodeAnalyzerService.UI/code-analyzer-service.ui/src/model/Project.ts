import {Analysis, AnalysisWithViolationCount} from "./Analysis";

export type Project = {
    id: number;
    projectName: string;
    lastAnalysis: Analysis;
    analysisHistory: AnalysisWithViolationCount[]
}