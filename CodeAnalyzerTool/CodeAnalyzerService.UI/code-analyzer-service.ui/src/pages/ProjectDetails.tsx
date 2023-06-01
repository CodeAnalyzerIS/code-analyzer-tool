import {useProject} from "../hooks/useProject";
import {useParams} from "react-router-dom";
import Loading from "../components/Loading";
import {
    Alert,
    Box,
} from "@mui/material";
import React, {useCallback, useContext, useEffect} from "react";
import ProjectDetailCards from "../components/ProjectDetailCards";
import AnalysisSummary from "../components/AnalysisSummary";
import BreadcrumbContext, {Breadcrumb, IBreadcrumbContext} from "../context/BreadcrumbContext";

export default function ProjectDetails() {
    const {id} = useParams<{ id: string }>()
    const {isLoading, isError, project} = useProject(id!)
    const {setBreadcrumbData} = useContext<IBreadcrumbContext>(BreadcrumbContext)

    //use useCallback to update the state and to be able to provide the dependency array with the set state
    //without triggering infinite re-renders, because the callback function will only be called when the setter changes
    const updateBreadcrumbData = useCallback(() => {
        const newBreadcrumbData: Breadcrumb[] = [
            {label: 'ᓚᘏᗢ', path:'/'},
            {label: project ? project.projectName : '', path:`/project/${id}`}
        ];
        setBreadcrumbData(newBreadcrumbData);
    }, [id, project, setBreadcrumbData]);

    useEffect(() => {
        updateBreadcrumbData();
    }, [updateBreadcrumbData]);

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
                                analysisAmount={project.analysisHistory.length}
                                ruleViolationCount={project.ruleViolationCount}
                                ruleViolationHistory={project.ruleViolationHistory}
                                ruleViolationDifference={project.ruleViolationDifference}/>
            <AnalysisSummary initialAnalysisId={project.lastAnalysisId} analysisHistory={project.analysisHistory}/>
        </Box>
    )
}