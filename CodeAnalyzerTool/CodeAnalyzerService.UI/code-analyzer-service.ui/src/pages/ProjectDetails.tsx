import {useProject} from "../hooks/useProject";
import {useParams} from "react-router-dom";
import Loading from "../components/Loading";
import {Alert, Box} from "@mui/material";
import React, {useEffect} from "react";
import ProjectDetailCards from "../components/ProjectDetailCards";
import {Breadcrumb} from "../App";

interface ProjectDetailsProps {
    updateBreadCrumbData: (data: Breadcrumb[]) => void;
}

export default function ProjectDetails({updateBreadCrumbData}: ProjectDetailsProps) {
    const {id} = useParams<{ id: string }>()
    const {isLoading, isError, project} = useProject(id!)
    useEffect(() => {
        const newBreadcrumbData: Breadcrumb[] = [
            {label: 'ᓚᘏᗢ', path:'/'},
            {label: project ? project.projectName : '', path:`/project/${id}`}
        ];
        updateBreadCrumbData(newBreadcrumbData);
    }, [updateBreadCrumbData])

    if (isLoading) {
        return <Loading/>
    }
    if (isError){
        return <Alert severity="error">Error loading the project</Alert>
    }

    return(
        <Box sx={{display: 'flex', flexDirection: 'column', justifyContent: 'center', mt: 3}}>
            <ProjectDetailCards projectName={project.projectName} lastAnalysisDate={project.lastAnalysis.createdOn}
            analysisHistory={project.analysisHistory} ruleViolationCount={project.lastAnalysis.ruleViolations.length}/>
        </Box>
    )
}