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

    // Sort the paths alphabetically
    const sortedPaths = Object.keys(groupedRuleViolations).sort();

    // Create a new object with sorted paths
    const sortedGroupedRuleViolations: Record<string, RuleViolation[]> = {};
    sortedPaths.forEach((path) => {
        sortedGroupedRuleViolations[path] = groupedRuleViolations[path];
    });


    return sortedGroupedRuleViolations;
}

interface FileNode {
    name: string;
    children?: FileNode[];
}

export function splitPathsIntoStringArray(groupedAnalysis: Record<string, RuleViolation[]>) {
    // const paths = Object.keys(groupedAnalysis)
    // const splitPaths = paths.map((path) => path.split('\\'));
    const paths = Object.keys(groupedAnalysis)[1]
    const splitPaths = paths.split('\\')
    const root: FileNode = {name: splitPaths[0], children: []}
    console.log(splitPaths)
    console.log(root)
    for (let i = 1; i < splitPaths.length; i++) {
        root.children?.push({name: splitPaths[i], children: []})
    }
}

// function createFileNode(splitPaths: string[]): FileNode {
//     for (let i = 1; i < splitPaths.length; i++) {
//         if (i < splitPaths.length) {
//
//         }
//     }
// }

export const getMUISeverity = (severity: string) => {
    switch (severity) {
        case 'Info':
            return 'info';
        case 'Warning':
            return 'warning';
        case 'Error':
            return 'error';
    }
}