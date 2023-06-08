import React from 'react';
import TreeView from '@mui/lab/TreeView';
import TreeItem, {TreeItemProps} from '@mui/lab/TreeItem';
import {Folder} from "../services/ruleViolationService";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import {Box, styled, SvgIconProps, Typography} from "@mui/material";
import {treeItemClasses} from "@mui/lab";
import FolderIcon from '@mui/icons-material/Folder';
import DescriptionIcon from '@mui/icons-material/Description';

type StyledTreeItemProps = TreeItemProps & {
    bgColor?: string;
    color?: string;
    labelIcon: React.ElementType<SvgIconProps>;
    labelInfo?: string;
    labelText: string;
};

export function FolderTree({folders}: { folders: Folder[] }) {
    const renderTree = (folder: Folder) => {

        return (
            <div key={folder.id}>
                {folder.children && folder.children?.length > 0 ?
                    <StyledTreeItem nodeId={folder.id} labelText={folder.name} labelIcon={FolderIcon} defaultChecked>
                        {folder.children && folder.children.map((child) => renderTree(child))}
                    </StyledTreeItem> :
                    <a href={`#${folder.pathLink}`} style={{all: 'unset'}}>
                        <StyledTreeItem nodeId={folder.id} labelText={folder.name} labelIcon={DescriptionIcon}/>
                    </a>}
            </div>
        );
    };

    return (
        <TreeView aria-label="file system navigator"
                  defaultCollapseIcon={<ExpandMoreIcon/>}
                  defaultExpandIcon={<ChevronRightIcon/>}
                  defaultExpanded={['0']}>
            {folders.map((folder) => renderTree(folder))}
        </TreeView>
    );
}

const StyledTreeItemRoot = styled(TreeItem)(({theme}) => ({
    color: theme.palette.text.secondary,
    [`& .${treeItemClasses.content}`]: {
        color: theme.palette.text.secondary,
        borderTopRightRadius: theme.spacing(2),
        borderBottomRightRadius: theme.spacing(2),
        paddingRight: theme.spacing(1),
        fontWeight: theme.typography.fontWeightMedium,
        '&.Mui-expanded': {
            fontWeight: theme.typography.fontWeightRegular,
        },
        '&:hover': {
            backgroundColor: theme.palette.action.hover,
        },
        '&.Mui-focused, &.Mui-selected, &.Mui-selected.Mui-focused': {
            backgroundColor: `var(--tree-view-bg-color, ${theme.palette.action.selected})`,
            color: 'var(--tree-view-color)',
        },
        [`& .${treeItemClasses.label}`]: {
            fontWeight: 'inherit',
            color: 'inherit',
        },
    },
    [`& .${treeItemClasses.group}`]: {
        [`& .${treeItemClasses.content}`]: {
            paddingLeft: theme.spacing(0.1),
        },
    },
}));

function StyledTreeItem(props: StyledTreeItemProps) {
    const {
        bgColor,
        color,
        labelIcon: LabelIcon,
        labelInfo,
        labelText,
        ...other
    } = props;

    return (
        <StyledTreeItemRoot
            label={
                <Box sx={{display: 'flex', alignItems: 'center', p: 0.5, pr: 0}}>
                    <Box component={LabelIcon} color="inherit" sx={{mr: 1}}/>
                    <Typography variant="body2" sx={{fontWeight: 'inherit', flexGrow: 1}}>
                        {labelText}
                    </Typography>
                    <Typography variant="caption" color="inherit">
                        {labelInfo}
                    </Typography>
                </Box>
            }
            {...other}
        />
    );
}