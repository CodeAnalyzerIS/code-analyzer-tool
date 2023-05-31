import {AnalysisWithViolationCount} from "../model/Analysis";
import {Box, Card, CardContent, Typography} from "@mui/material";
import React from "react";
import {Sparklines, SparklinesLine} from 'react-sparklines';
import TrendingDownIcon from '@mui/icons-material/TrendingDown';
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import TroubleshootIcon from '@mui/icons-material/Troubleshoot';

interface ProjectDetailCardsProps {
    projectName: string;
    lastAnalysisDate: Date;
    ruleViolationCount: number;
    analysisHistory: AnalysisWithViolationCount[];
}

export default function ProjectDetailCards({
                                               projectName,
                                               lastAnalysisDate,
                                               ruleViolationCount,
                                               analysisHistory
                                           }: ProjectDetailCardsProps) {
    const ruleViolationDifference = analysisHistory.length > 1
        ?
        Math.abs(Math.round((1 - (ruleViolationCount / analysisHistory[analysisHistory.length - 2].ruleViolationCount))*100))
        :
        0;

    const iconColor = ruleViolationDifference < 0 ? 'error' : 'success';
    const sparkLineColor = ruleViolationDifference < 0 ? 'red' : 'green';

    return (
        <Box sx={{display: 'flex', flexDirection: 'row', justifyContent: 'space-around', textAlign: 'center', width: '100%'}}>
            <Card sx={{width: '25%', height: '15vh', display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
                <CardContent sx={{color: "#15B7B9", fontSize: '2em'}}>
                    {projectName}
                </CardContent>
            </Card>
            <Card sx={{width: '20%', height: '15vh'}}>
                <CardContent>
                    <Typography sx={{color: '#b0b0b0', mb: 1, fontSize: '1.2em'}}>{ruleViolationCount} Rule violations in last analysis</Typography>
                    {analysisHistory.length < 2 ? "" :
                        <>
                            <Typography sx={{display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
                                {ruleViolationDifference < 0 ? <TrendingUpIcon color={iconColor}/>
                                    : <TrendingDownIcon color={iconColor}/>}
                                {`${ruleViolationDifference}%`}
                            </Typography>
                            <Sparklines data={analysisHistory.map((analysis) => analysis.ruleViolationCount)} height={30}>
                                <SparklinesLine color={sparkLineColor} style={{fill: "none"}}/>
                            </Sparklines>
                        </>}
                </CardContent>
            </Card>
            <Card sx={{width: '25%', height: '15vh', display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
                <CardContent>
                    <Box sx={{color: "#15B7B9"}}>
                        <TroubleshootIcon fontSize={'large'}/>
                    </Box>
                    <Typography sx={{color: '#b0b0b0', mt: 1, fontSize: '1.2em'}}>{analysisHistory.length} Analyses</Typography>
                    <Typography sx={{color: '#b0b0b0', mt: 1, fontSize: '1.2em'}}>Last Analysis: {lastAnalysisDate.toString()}</Typography>
                </CardContent>
            </Card>
        </Box>
    )
}