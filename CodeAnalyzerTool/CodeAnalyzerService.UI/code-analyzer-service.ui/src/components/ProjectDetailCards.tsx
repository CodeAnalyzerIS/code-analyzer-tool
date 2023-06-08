import {Box, Card, Container, Grid} from "@mui/material";
import React, {ReactNode} from "react";
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

        <Grid container justifyContent="space-evenly">
            <GridItemCard children={<AnalysesCard analysisAmount={analysisAmount} lastAnalysisDate={lastAnalysisDate}/>}/>
            <GridItemCard children={<MainCard projectName={projectName} repoUrl={repoUrl}/>}/>
            <GridItemCard children={<RuleViolationsCard ruleViolationCount={ruleViolationCount}
                                                        ruleViolationHistory={ruleViolationHistory}
                                                        ruleViolationDifference={ruleViolationDifference}/>}/>
        </Grid>
    )


    function GridItemCard({children}: {children: ReactNode}) {
        return <Grid item xs={12} md={5} lg={3}>
            <Container maxWidth="lg" sx={{height: '175px', textAlign: 'center', display:'flex', justifyContent: 'center', mt: 1}}>
                <Card sx={{width: '100%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', maxWidth: '550px'}}>
                    {children}
                </Card>
            </Container>
        </Grid>
    }
}