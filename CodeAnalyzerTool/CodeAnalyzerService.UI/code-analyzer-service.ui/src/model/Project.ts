import {AnalysisWithViolationCount} from "./Analysis";

export type Project = {
    id: number;
    projectName: string;
    lastAnalysisId: number;
    analysisHistory: AnalysisWithViolationCount[]
}