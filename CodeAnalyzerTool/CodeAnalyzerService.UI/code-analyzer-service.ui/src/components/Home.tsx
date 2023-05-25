import React from "react";
import {useProject} from "../hooks/useProject";
import Loading from "./Loading";
import {Alert, Card, CardContent, CardHeader, Stack, Typography} from "@mui/material";

export default function Home() {
    const {isLoading, isError, project} = useProject(1)

    if (isLoading) {
        return <Loading/>
    }
    if (isError){
        return <Alert severity="error">Error loading the project</Alert>
    }
    console.log(project)

    return(
        <Card sx={{width: "50%"}}>
            <CardHeader title={project.projectName}/>
            <CardContent>
                <Stack direction="row" spacing={'space-between'}>
                    <Typography>{project.analyses[project.analyses.length - 1].createdOn.toLocaleString("en-US")}</Typography>
                    <Typography>{project.analyses[project.analyses.length - 1].ruleViolations.length}</Typography>
                </Stack>
            </CardContent>
        </Card>
    )
}