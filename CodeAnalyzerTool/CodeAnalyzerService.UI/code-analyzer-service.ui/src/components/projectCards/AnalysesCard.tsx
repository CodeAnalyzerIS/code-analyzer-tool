import {Box, Card, CardContent, Typography} from "@mui/material";
import TroubleshootIcon from "@mui/icons-material/Troubleshoot";
import React from "react";

interface AnalysesCardProps {
    analysisAmount: number;
    lastAnalysisDate: Date;
}

export default function AnalysesCard({analysisAmount, lastAnalysisDate}: AnalysesCardProps) {
    return (
        <Card sx={{width: '20%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
            <CardContent>
                <Box sx={{color: "#15B7B9"}}>
                    <TroubleshootIcon fontSize={'large'}/>
                </Box>
                <Typography
                    sx={{color: '#15B7B9', mt: 1, fontSize: '1.2em'}}><strong>{analysisAmount}</strong> Analyses</Typography>
                <Typography sx={{color: '#15B7B9', mt: 1, fontSize: '1.2em'}}>
                    Last Analysis: {lastAnalysisDate.toLocaleString('nl-BE')}
                </Typography>
            </CardContent>
        </Card>
    )
}