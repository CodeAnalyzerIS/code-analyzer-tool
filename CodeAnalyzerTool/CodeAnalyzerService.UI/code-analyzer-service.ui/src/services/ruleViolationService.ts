import {BACKEND_URL} from "../App";
import {Rule} from "../model/Rule";
import {RuleViolation} from "../model/RuleViolation";

export async function getRuleViolation(id: string) {
    const response = await fetch(`${BACKEND_URL}/api/RuleViolation/${id}`);
    return await response.json() as RuleViolation;
}