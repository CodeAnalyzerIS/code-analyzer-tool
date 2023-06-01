import React, {useContext, useEffect, useState} from "react";
import Loading from "../components/Loading";
import {Alert, Box, Card, CardContent, CardHeader, Stack, Typography} from "@mui/material";
import {useProjectOverview} from "../hooks/useProjectOverview";
import TroubleshootIcon from '@mui/icons-material/Troubleshoot';
import {ProjectOverview} from "../model/ProjectOverview";
import ReportIcon from '@mui/icons-material/Report';
import {Searchbar} from "../components/Searchbar";
import {useNavigate} from "react-router-dom";
import {getProjectIdFromName} from "../services/projectService";
import BreadcrumbContext, {Breadcrumb, IBreadcrumbContext} from "../context/BreadcrumbContext";

export type SearchString = {
    searchValue: string;
}

export default function Home() {
    const [searchString, setSearchString] = useState("")
    const {isLoading, isError, projectOverviews} = useProjectOverview()
    const {setBreadcrumbData} = useContext<IBreadcrumbContext>(BreadcrumbContext)
    const navigate = useNavigate()

    useEffect(() => {
        const newBreadcrumbData: Breadcrumb[] = [
            {label: 'ᓚᘏᗢ', path:'/'},
        ];
        setBreadcrumbData(newBreadcrumbData);
    }, [projectOverviews])

    if (isLoading) {
        return <Loading/>
    }
    if (isError || !projectOverviews){
        return <Alert severity="error">Error loading the projects</Alert>
    }

    if (searchString !== ""){
        getProjectIdFromName(searchString)
            .then((id) => navigate(`/project/${id}`))
            .catch(() => {
                return <Alert severity="error">Error getting project with name: {searchString}</Alert>
            })
    }

    return(
        <Box sx={{display: 'flex', flexDirection: 'column', alignItems: 'center', mt: 5}}>
            <Searchbar setSearchString={setSearchString}/>
            {projectOverviews.map((po:ProjectOverview)  => (
                <Card sx={{width: "50%", border: 'solid', borderColor: '#c4c4c4', borderWidth: 'thin', mt: 3, cursor: 'pointer'}}
                      key={po.id} onClick={() => {navigate(`/project/${po.id}`)}}>
                    <CardHeader
                        style={{ textAlign: 'center', color: '#6574FC', borderBottom: 'solid', borderColor: '#c4c4c4',
                            borderWidth: 'thin' }}
                        title={po.projectName}
                    />
                    <CardContent>
                        <Box sx={{display: 'flex', flexDirection: 'row', justifyContent: 'space-evenly',
                            alignItems: 'center', color: '#15B7B9'}}>
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