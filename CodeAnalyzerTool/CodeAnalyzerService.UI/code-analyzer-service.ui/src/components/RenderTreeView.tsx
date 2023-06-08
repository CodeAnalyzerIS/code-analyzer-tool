import {Folder} from "../util/HelperFunctions";
import {TreeItem} from "@mui/lab";
import {useState} from "react";
import TreeView from "@mui/lab/TreeView";

export default function RenderTreeView(folder: Folder) {
    const { name, children } = folder;

    const [expandedNodes, setExpandedNodes] = useState<string[]>([]);

    const handleNodeToggle = (event: React.ChangeEvent<{}>, nodeIds: string[]) => {
        setExpandedNodes(nodeIds);
    };

    return (
        // <TreeItem key={name} nodeId={name} label={name}>
        //     {children && children.map((child) => RenderTreeView(child))}
        // </TreeItem>
        <TreeItem key={name} nodeId={name} label={name}>
            {children && (
                <TreeView
                    defaultExpanded={[]}
                    expanded={expandedNodes}
                    onNodeToggle={handleNodeToggle}
                >
                    {children.map((child) => RenderTreeView(child))}
                </TreeView>
            )}
        </TreeItem>
    )
}