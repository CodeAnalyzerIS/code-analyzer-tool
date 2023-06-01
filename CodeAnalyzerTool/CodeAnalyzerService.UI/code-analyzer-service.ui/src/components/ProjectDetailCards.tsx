import {Box} from "@mui/material";
import React from "react";
import AnalysesCard from "./projectCards/AnalysesCard";
import MainCard from "./projectCards/MainCard";
import RuleViolationsCard from "./projectCards/RuleViolationsCard";

interface ProjectDetailCardsProps {
    projectName: string;
    repoUrl: string | null;
    lastAnalysisDate: Date;
    ruleViolationCount: number;
    ruleViolationHistory: number[];
    ruleViolationDifference: number;
    analysisAmount: number;
}

export default function ProjectDetailCards({
                                               projectName,
                                               repoUrl,
                                               lastAnalysisDate,
                                               ruleViolationCount,
                                               ruleViolationHistory,
                                               ruleViolationDifference,
                                               analysisAmount
                                           }: ProjectDetailCardsProps) {

    return (
        <Box sx={{
            display: 'flex',
            flexDirection: 'row',
            justifyContent: 'space-around',
            textAlign: 'center',
            width: '100%',
            height: '175px'
        }}>
            <AnalysesCard analysisAmount={analysisAmount} lastAnalysisDate={lastAnalysisDate}/>
            <MainCard projectName={projectName} repoUrl={repoUrl}/>
            <RuleViolationsCard ruleViolationCount={ruleViolationCount}
                                ruleViolationHistory={ruleViolationHistory}
                                ruleViolationDifference={ruleViolationDifference}/>
        </Box>
    )
}