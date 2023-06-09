import {Drawer, Fab, styled} from "@mui/material";
import {FolderTree} from "./FolderTree";
import React from "react";
import {Folder} from "../services/ruleViolationService";
import CloseIcon from '@mui/icons-material/Close';
import ForestIcon from '@mui/icons-material/Forest';

interface ProjectDetailsDrawerProps {
    isOpen: boolean;
    onClose: () => void;
    handleDrawerOpen: () => void;
    folderHierarchy: Folder[]
}

export default function ProjectDetailsDrawer({isOpen, onClose, handleDrawerOpen, folderHierarchy}: ProjectDetailsDrawerProps) {
    
    return (
        <div className={"project-details-drawer-container"}>
            <Drawer PaperProps={{sx: {minWidth: 300, overflow: 'hidden', paddingTop: 2}}}
                    BackdropProps={{sx: {backgroundColor: 'transparent'}}}
                    open={isOpen}
                    onClose={onClose}>
                <FolderTree folders={folderHierarchy}/>
                {isOpen && (
                    <div style={{position: 'absolute', bottom: 25, right: 16}}>
                        <Fab color="secondary" onClick={handleDrawerOpen}>
                            <CloseIcon/>
                        </Fab>
                    </div>
                )}
            </Drawer>
            {!isOpen && (
                <div style={{position: 'fixed', bottom: 10, left: 10}}>
                    <Fab color="secondary" onClick={handleDrawerOpen}>
                        <ForestIcon sx={{transform: 'rotate(90deg)'}}/>
                    </Fab>
                </div>
            )}
        </div>
    )
}