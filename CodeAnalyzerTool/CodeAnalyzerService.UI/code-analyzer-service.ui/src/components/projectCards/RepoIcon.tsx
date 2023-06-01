import {Avatar} from "@mui/material";
import React from "react";

interface RepoIconProps {
    iconSource: string;
}

export default function RepoIcon({iconSource}: RepoIconProps) {
    return (
        <Avatar sx={{width: 24, height: 24}} src={iconSource}/>
    )
}