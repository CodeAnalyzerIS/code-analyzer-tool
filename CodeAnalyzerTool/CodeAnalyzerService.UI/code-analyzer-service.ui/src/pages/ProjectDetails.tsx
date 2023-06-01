import {useProject} from "../hooks/useProject";
import {useParams} from "react-router-dom";
import Loading from "../components/Loading";
import {
    Alert,
    Box,
} from "@mui/material";
import React, {useContext, useEffect} from "react";
import ProjectDetailCards from "../components/ProjectDetailCards";
import AnalysisSummary from "../components/AnalysisSummary";
import BreadcrumbContext, {Breadcrumb, IBreadcrumbContext} from "../context/BreadcrumbContext";

export default function ProjectDetails() {
    const {id} = useParams<{ id: string }>()
    const {isLoading, isError, project} = useProject(id!)
    const {breadcrumbData, setBreadcrumbData} = useContext<IBreadcrumbContext>(BreadcrumbContext)

    useEffect(() => {
        const newBreadcrumbData: Breadcrumb[] = [
            {label: 'ᓚᘏᗢ', path:'/'},
            {label: project ? project.projectName : '', path:`/project/${id}`}
        ];
        setBreadcrumbData(newBreadcrumbData);
    }, [breadcrumbData, id, project])

    if (isLoading) {
        return <Loading/>
    }
    if (isError || !project){
        return <Alert severity="error">Error loading the project</Alert>
    }


    return(
        <Box sx={{display: 'flex', flexDirection: 'column', alignItems: 'center', mt: 3, mb: 20}}>
            <ProjectDetailCards projectName={project.projectName}
                                lastAnalysisDate={project.analysisHistory[project.analysisHistory.length - 1].createdOn}
                                analysisHistory={project.analysisHistory}
                                ruleViolationCount={project.analysisHistory[project.analysisHistory.length - 1].ruleViolationCount}/>
            <AnalysisSummary initialAnalysisId={project.lastAnalysisId} analysisHistory={project.analysisHistory}/>
        </Box>
    )
}