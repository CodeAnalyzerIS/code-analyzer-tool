import {BACKEND_URL} from "../App";
import {RuleViolation} from "../model/RuleViolation";
import {Analysis} from "../model/Analysis";

export async function getRuleViolation(id: string): Promise<RuleViolation> {
    const response = await fetch(`${BACKEND_URL}/api/RuleViolation/${id}`);
    const ruleViolation = await response.json() as RuleViolation;
    return {...ruleViolation, analysisDate: new Date(ruleViolation.analysisDate)};
}

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

export interface Folder {
    name: string;
    id: string;
    children?: Folder[];
}

export function createFolderHierarchy(paths: string[]): Folder[] {
    const root: Folder = { name: "root", id: "root" };
    let index = 0

    for (const path of paths) {
        const folders = path.split("\\");
        let currentFolder = root;

        for (const folderName of folders) {
            let folder = currentFolder.children?.find((child) => child.name === folderName);

            if (!folder) {
                folder = { name: folderName, id: (index++).toString() };
                currentFolder.children = currentFolder.children || [];
                currentFolder.children.push(folder);
            }

            currentFolder = folder;
        }
    }
    if (root.children)
        return root.children

    return [];
}