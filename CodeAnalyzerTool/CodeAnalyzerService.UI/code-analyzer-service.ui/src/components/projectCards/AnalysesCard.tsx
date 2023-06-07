import {Box, Card, CardContent, Typography, useTheme} from "@mui/material";
import TroubleshootIcon from "@mui/icons-material/Troubleshoot";
import React from "react";

interface AnalysesCardProps {
    analysisAmount: number;
    lastAnalysisDate: Date;
}

export default function AnalysesCard({analysisAmount, lastAnalysisDate}: AnalysesCardProps) {
    const palette = useTheme().palette;

    return (
        <CardContent>
            <TroubleshootIcon fontSize={'large'} htmlColor={palette.primary.main}/>
            <Typography
                sx={{color: palette.primary.main, mt: 1, fontSize: '1.2em'}}><strong>{analysisAmount}</strong> Analyses</Typography>
            <Typography sx={{color: palette.primary.main, mt: 1, fontSize: '1.2em'}}>
                Last Analysis: {lastAnalysisDate.toLocaleString('nl-BE')}
            </Typography>
        </CardContent>
    )
}