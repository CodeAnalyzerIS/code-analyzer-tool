import {useProject} from "../hooks/useProject";
import {useParams} from "react-router-dom";
import Loading from "../components/Loading";
import {
    Alert,
    Box,
} from "@mui/material";
import React, {useEffect} from "react";
import ProjectDetailCards from "../components/ProjectDetailCards";
import {Breadcrumb} from "../App";
import AnalysisSummary from "../components/AnalysisSummary";

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
    }, [id, project])

    if (isLoading) {
        return <Loading/>
    }
    if (isError){
        return <Alert severity="error">Error loading the project</Alert>
    }


    return(
        <Box sx={{display: 'flex', flexDirection: 'column', alignItems: 'center', mt: 3}}>
            <ProjectDetailCards projectName={project.projectName}
                                lastAnalysisDate={project.analysisHistory[project.analysisHistory.length - 1].createdOn}
                                analysisHistory={project.analysisHistory}
                                ruleViolationCount={project.analysisHistory[project.analysisHistory.length - 1].ruleViolationCount}/>
            <AnalysisSummary initialAnalysisId={project.lastAnalysisId} analysisHistory={project.analysisHistory}/>
        </Box>
    )
}