import React, {useCallback, useContext, useEffect, useState} from "react";
import Loading from "../components/Loading";
import {Alert, Box, Card, CardContent, CardHeader, Stack, Typography, useTheme} from "@mui/material";
import {useProjectOverview} from "../hooks/useProjectOverview";
import TroubleshootIcon from '@mui/icons-material/Troubleshoot';
import {ProjectOverview} from "../model/ProjectOverview";
import ReportIcon from '@mui/icons-material/Report';
import {Searchbar} from "../components/Searchbar";
import {useNavigate} from "react-router-dom";
import {getProjectIdFromName} from "../services/projectService";
import BreadcrumbContext, {Breadcrumb, IBreadcrumbContext} from "../context/BreadcrumbContext";
import WelcomePlaceholder from "../components/placeholders/WelcomePlaceholder";

export type SearchString = {
    searchValue: string;
}

export default function Home() {
    const [searchString, setSearchString] = useState("")
    const {isLoading, isError, projectOverviews} = useProjectOverview()
    const {setBreadcrumbData} = useContext<IBreadcrumbContext>(BreadcrumbContext)
    const navigate = useNavigate()
    const palette = useTheme().palette

    //use useCallback to update the state and to be able to provide the dependency array with the set state
    //without triggering infinite re-renders, because the callback function will only be called when the setter changes
    const updateBreadcrumbData = useCallback(() => {
        const newBreadcrumbData: Breadcrumb[] = [
            {label: 'ᓚᘏᗢ', path:'/'},
        ];
        setBreadcrumbData(newBreadcrumbData);
    }, [setBreadcrumbData]);

    useEffect(() => {
        updateBreadcrumbData();
    }, [updateBreadcrumbData]);

    if (isLoading) return <Loading/>

    if (isError || !projectOverviews) return <Alert severity="error">Error loading the projects</Alert>


    if (searchString !== ""){
        getProjectIdFromName(searchString)
            .then((id) => navigate(`/project/${id}`))
            .catch(() => {
                return <Alert severity="error">Error getting project with name: {searchString}</Alert>
            })
    }

    if (projectOverviews.length < 1) return <WelcomePlaceholder/>;

    return(
        <Box sx={{display: 'flex', flexDirection: 'column', alignItems: 'center', mt: 5}}>
            <Searchbar setSearchString={setSearchString}/>
            {projectOverviews.map((po:ProjectOverview)  => (
                <Card sx={{width: "50%", border: 'solid', borderColor: palette.grey["300"], borderWidth: 'thin', mt: 3, cursor: 'pointer'}}
                      key={po.id} onClick={() => {navigate(`/project/${po.id}`)}}>
                    <CardHeader
                        style={{ textAlign: 'center', color: palette.primary.main, borderBottom: 'solid', borderColor: palette.grey["300"],
                            borderWidth: 'thin' }}
                        title={po.projectName}
                    />
                    <CardContent>
                        <Box sx={{display: 'flex', flexDirection: 'row', justifyContent: 'space-evenly',
                            alignItems: 'center', color: palette.secondary.main}}>
                            <Stack direction='row' spacing={1}>
                                <TroubleshootIcon/>
                                <Typography>Last Analysis: {po.lastAnalysisDate.toLocaleString('nl-BE')}</Typography>
                            </Stack>

                            <Stack direction='row' spacing={1}>
                                <ReportIcon/>
                                <Typography>Rule Violations: {po.ruleViolationCount}</Typography>
                            </Stack>
                        </Box>
                    </CardContent>
                </Card>
            ))}
        </Box>
    )
}