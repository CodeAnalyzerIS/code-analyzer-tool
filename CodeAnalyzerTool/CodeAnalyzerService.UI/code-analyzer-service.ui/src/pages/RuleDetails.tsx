import {useParams} from "react-router-dom";
import {useRule} from "../hooks/useRule";
import Loading from "../components/Loading";
import {Alert, Container, Typography} from "@mui/material";
import React from "react";
import ProjectDetailCards from "../components/ProjectDetailCards";
import AnalysisSummary from "../components/AnalysisSummary";

export default function RuleDetails() {
    const {id} = useParams<{ id: string }>()
    const {isLoading, isError, rule} = useRule(id!)

    // const updateBreadcrumbData = useCallback(() => {
    //     const newBreadcrumbData: Breadcrumb[] = [
    //         {label: 'ᓚᘏᗢ', path:'/'},
    //         {label: project ? project.projectName : '', path:`/project/${id}`}
    //     ];
    //     setBreadcrumbData(newBreadcrumbData);
    // }, [id, project, setBreadcrumbData]);
    //
    // useEffect(() => {
    //     updateBreadcrumbData();
    // }, [updateBreadcrumbData]);


    if (isLoading) return <Loading/>;
    if (isError || !rule)return <Alert severity="error">Error loading the rule</Alert>;

    return(
        <Container maxWidth="lg">
            <Typography variant="h2">{rule.title}</Typography>
            <p>{rule.ruleName} {rule.category}</p>
            <pre>{rule?.codeExample}</pre>
        </Container>
    )
}