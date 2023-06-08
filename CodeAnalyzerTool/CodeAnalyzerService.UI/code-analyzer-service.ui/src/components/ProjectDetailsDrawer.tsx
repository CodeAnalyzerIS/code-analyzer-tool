import {Drawer} from "@mui/material";
import {FolderTree} from "./FolderTree";
import React from "react";
import {Folder} from "../services/ruleViolationService";

interface ProjectDetailsDrawerProps {
    isOpen: boolean;
    onClose: () => void;
    folderHierarchy: Folder[]
}

export default function ProjectDetailsDrawer({isOpen, onClose, folderHierarchy} : ProjectDetailsDrawerProps) {
    return (
        <Drawer PaperProps={{ sx: {minWidth: 300, overflow: 'hidden', paddingTop: 2}}} open={isOpen} onClose={onClose}>
            <FolderTree folders={folderHierarchy}/>
        </Drawer>
    )
}