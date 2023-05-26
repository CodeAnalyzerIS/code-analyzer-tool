import React from "react";
import Loading from "./Loading";
import {Alert, Box, Card, CardContent, CardHeader, Stack, Typography} from "@mui/material";
import {useProjectOverview} from "../hooks/useProjectOverview";
import TroubleshootIcon from '@mui/icons-material/Troubleshoot';
import {ProjectOverview} from "../model/ProjectOverview";
import ReportIcon from '@mui/icons-material/Report';

export default function Home() {
    const {isLoading, isError, projectOverviews} = useProjectOverview()

    if (isLoading) {
        return <Loading/>
    }
    if (isError){
        return <Alert severity="error">Error loading the project</Alert>
    }

    return(
        <Box sx={{display: 'flex', justifyContent: 'center', mt: 5}}>
            {projectOverviews.map((po:ProjectOverview)  => (
                <Card sx={{width: "50%", border: 'solid', borderColor: '#c4c4c4', borderWidth: 'thin'}} key={po.id}>
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
                                <Typography>Last Analysis: {po.lastAnalysisDate.toString()}</Typography>
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