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

interface Node {
    name: string;
    children: Node[];
}

// export interface Folder {
//     [folderName: string]: Folder | string;
// }
//
// export function createFolderHierarchy(paths: string[]): Folder {
//     const root: Folder = {};
//
//     for (const path of paths) {
//         const folders = path.split("\\");
//         let currentFolder = root;
//
//         for (let i = 0; i < folders.length - 1; i++) {
//             const folderName = folders[i];
//
//             if (!currentFolder[folderName]) {
//                 currentFolder[folderName] = {};
//             }
//
//             currentFolder = currentFolder[folderName] as Folder;
//         }
//
//         const fileName = folders[folders.length - 1];
//         currentFolder[fileName] = fileName;
//     }
//
//     return root;
// }

export interface Folder {
    name: string;
    children?: Folder[];
}

export function createFolderHierarchy(paths: string[]): Folder {
    const root: Folder = { name: "root" };

    for (const path of paths) {
        const folders = path.split("\\");
        let currentFolder = root;

        for (const folderName of folders) {
            let folder = currentFolder.children?.find((child) => child.name === folderName);

            if (!folder) {
                folder = { name: folderName };
                currentFolder.children = currentFolder.children || [];
                currentFolder.children.push(folder);
            }

            currentFolder = folder;
        }
    }

    return root;
}


// export function splitPathsIntoStringArray(groupedAnalysis: Record<string, RuleViolation[]>) {
//     // const paths = Object.keys(groupedAnalysis)
//     // const splitPaths = paths.map((path) => path.split('\\'));
//     const paths = Object.keys(groupedAnalysis)[1]
//     const splitPaths = paths.split('\\')
//     const root: FileNode = {name: splitPaths[0], children: []}
//     console.log(splitPaths)
//     console.log(root)
//     for (let i = 1; i < splitPaths.length; i++) {
//         root.children?.push({name: splitPaths[i], children: []})
//     }
// }

export function splitPathsIntoStringArray(groupedAnalysis: Record<string, RuleViolation[]>) {
    const paths = Object.keys(groupedAnalysis)
    console.log(paths)
    // const splitPaths = paths.map((path) => path.split('\\'));
    function createHierarchy(paths: string[]): Node {
        const root: Node = { name: "", children: [] };

        for (const path of paths) {
            const segments = path.split("\\");
            let currentNode = root;

            for (const segment of segments) {
                let childNode = currentNode.children.find((node) => node.name === segment);

                if (!childNode) {
                    childNode = { name: segment, children: [] };
                    currentNode.children.push(childNode);
                }

                currentNode = childNode;
            }
        }

        return root;
    }
    console.log(createHierarchy(paths))
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