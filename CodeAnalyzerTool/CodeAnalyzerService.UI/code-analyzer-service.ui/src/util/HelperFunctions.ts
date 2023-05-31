import {Analysis} from "../model/Analysis";
import {RuleViolation} from "../model/RuleViolation";

export function groupRuleViolationsByPath(analysis: Analysis): Record<string, RuleViolation[]> {
    const groupedRuleViolations: Record<string, RuleViolation[]> = {};

    analysis.ruleViolations.forEach((violation) => {
        const { path } = violation.location;

        if (groupedRuleViolations[path]) {
            groupedRuleViolations[path].push(violation);
        } else {
            groupedRuleViolations[path] = [violation];
        }
    });

    return groupedRuleViolations;
}

export const getMUISeverity = (severity: string) => {
    switch (severity) {
        case 'Info':
            return 'info';
        case 'Warning':
            return 'warning';
        case 'Error':
            return 'error'
    }
}