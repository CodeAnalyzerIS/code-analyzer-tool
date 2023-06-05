import {Card, CardContent, CardHeader, Stack, useTheme} from "@mui/material";
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

    return(
        <Card sx={{
            width: '30%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center',
            flexDirection: 'column'
        }}>
            <CardHeader sx={{color: palette.primary.main}} title={projectName}/>
            <CardContent>
                {repoUrl !== null ?
                    <Stack direction="row" alignItems='center' spacing={1}>
                        {getRepoIcon(repoUrl)}
                        <Link style={{color: "#6574FC"}} to={repoUrl}>
                            {repoUrl}
                        </Link>
                    </Stack> :
                    "No repo url provided"
                }
            </CardContent>
        </Card>
    )
}