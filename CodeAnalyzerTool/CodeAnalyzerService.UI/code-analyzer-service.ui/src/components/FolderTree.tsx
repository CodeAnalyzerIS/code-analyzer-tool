import React, {useState} from 'react';
import TreeView from '@mui/lab/TreeView';
import TreeItem from '@mui/lab/TreeItem';
import {Folder} from "../util/HelperFunctions";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";

export function FolderTree({ folder }: { folder: Folder }) {
    const renderTree = (folder: Folder) => {
        const { name, children } = folder;

        return (
            <TreeItem key={name} nodeId={name} label={name}>
                {children && children.map((child) => renderTree(child))}
            </TreeItem>
        );
    };

    return (
        <TreeView aria-label="file system navigator"
                  defaultCollapseIcon={<ExpandMoreIcon />}
                  defaultExpandIcon={<ChevronRightIcon />}
                  sx={{ height: 240, flexGrow: 1, maxWidth: 400, overflowY: 'auto' }}>
            {renderTree(folder)}
        </TreeView>
    );
}