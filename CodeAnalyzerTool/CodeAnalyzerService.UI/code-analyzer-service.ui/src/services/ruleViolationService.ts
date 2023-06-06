import {BACKEND_URL} from "../App";
import {RuleViolation} from "../model/RuleViolation";

export async function getRuleViolation(id: string): Promise<RuleViolation> {
    const response = await fetch(`${BACKEND_URL}/api/RuleViolation/${id}`);
    const ruleViolation = await response.json() as RuleViolation;
    return {...ruleViolation, analysisDate: new Date(ruleViolation.analysisDate)};
}