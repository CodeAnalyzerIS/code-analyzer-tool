import {CardContent, Stack, Typography, useTheme} from "@mui/material";
import {Link} from "react-router-dom";
import React from "react";
import RepoIcon from "./RepoIcon";

interface MainCardProps {
    projectName: string;
    repoUrl: string | null;
}

const getRepoIcon = (repoUrl: string) => {
    if (repoUrl.includes("github.com")) {
        return <RepoIcon iconSource={'/github-icon.png'}/>
    }
    if (repoUrl.includes("gitlab.com")) {
        return <RepoIcon iconSource={'/gitlab-icon.png'}/>
    }
    if (repoUrl.includes("dev.azure.com")) {
        return <RepoIcon iconSource={'/azure-devops-logo.png'}/>
    }
}

export default function MainCard({projectName, repoUrl}: MainCardProps) {
    const palette = useTheme().palette;

    return (<>
            <CardContent sx={{maxWidth: '80%', overflow: "hidden"}}>
                <Typography variant="h4" color={palette.primary.main} sx={{mb: 2}}>{projectName}</Typography>
                {repoUrl !== null ?
                    <Stack direction="row" alignItems='center' spacing={1} sx={{overflowWrap: 'anywhere'}}>
                        {getRepoIcon(repoUrl)}
                        <Link style={{color: palette.secondary.dark}} to={repoUrl}>
                            {repoUrl}
                        </Link>
                    </Stack> :
                    "No repo url provided"
                }
            </CardContent>
        </>
    )
}