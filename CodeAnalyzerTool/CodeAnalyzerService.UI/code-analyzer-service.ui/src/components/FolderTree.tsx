import React from 'react';
import TreeView from '@mui/lab/TreeView';
import TreeItem from '@mui/lab/TreeItem';
import {Folder} from "../services/ruleViolationService";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";

export function FolderTree({folders}: { folders: Folder[] }) {
    const renderTree = (folder: Folder) => {

        return (
            <div key={folder.id}>
                {folder.children && folder.children?.length > 0 ?
                    <TreeItem nodeId={folder.id} label={folder.name}>
                        {folder.children && folder.children.map((child) => renderTree(child))}
                    </TreeItem> :
                <a href={`#${folder.pathLink}`} style={{all: 'unset'}}>
                    <TreeItem nodeId={folder.id} label={folder.name}/>
                </a>}
            </div>
        );
    };

    return (
        <TreeView aria-label="file system navigator"
                  defaultCollapseIcon={<ExpandMoreIcon/>}
                  defaultExpandIcon={<ChevronRightIcon/>}>
            {folders.map((folder) => renderTree(folder))}
        </TreeView>
    );
}