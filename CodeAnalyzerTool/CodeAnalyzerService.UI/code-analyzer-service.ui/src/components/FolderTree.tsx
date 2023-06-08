import React from 'react';
import TreeView from '@mui/lab/TreeView';
import TreeItem from '@mui/lab/TreeItem';
import {Folder} from "../services/ruleViolationService";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";

export function FolderTree({ folders }: { folders: Folder[] }) {
    const renderTree = (folder: Folder) => {
        const { name, id, children } = folder;

        return (
            <TreeItem key={name} nodeId={id} label={name}>
                {children && children.map((child) => renderTree(child))}
            </TreeItem>
        );
    };

    return (
        <TreeView aria-label="file system navigator"
                  defaultCollapseIcon={<ExpandMoreIcon />}
                  defaultExpandIcon={<ChevronRightIcon />}
                  sx={{ height: 240, flexGrow: 1, maxWidth: 400, overflowY: 'auto' }}>
            {folders.map((folder) => renderTree(folder))}
        </TreeView>
    );
}