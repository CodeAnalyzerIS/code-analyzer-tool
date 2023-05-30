import {RuleViolation} from "./RuleViolation";

export type Analysis = {
    id: number;
    createdOn: Date;
    ruleViolations: RuleViolation[];
}

export type AnalysisWithViolationCount = {
    id: number;
    createdOn: Date;
    ruleViolationCount: number;
}