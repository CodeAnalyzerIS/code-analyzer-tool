import {RuleViolation} from "./RuleViolation";

export type Analysis = {
    id: number;
    createdOn: Date;
    ruleViolations: RuleViolation[];
}

export type AnalysisHistory = Omit<Analysis, "ruleViolations">