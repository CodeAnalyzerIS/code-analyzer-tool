import {Card, CardContent, Typography, useTheme} from "@mui/material";
import TrendingUpIcon from "@mui/icons-material/TrendingUp";
import TrendingDownIcon from "@mui/icons-material/TrendingDown";
import {Sparklines, SparklinesLine} from "react-sparklines";
import React from "react";

interface RuleViolationsCardProps {
    ruleViolationCount: number;
    ruleViolationHistory: number[];
    ruleViolationDifference: number;
}

export default function RuleViolationsCard({ruleViolationCount, ruleViolationHistory, ruleViolationDifference}: RuleViolationsCardProps) {
    const iconColor = ruleViolationDifference < 0 ? 'error' : 'success';
    const sparkLineColor = ruleViolationDifference < 0 ? 'red' : 'green';
    const palette = useTheme().palette;

    return(
        <Card sx={{width: '20%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
            <CardContent sx={{flex: '1'}}>
                <Typography sx={{color: palette.primary.main, mb: 1, fontSize: '1.2em'}}>
                    <strong>{ruleViolationCount}</strong> Rule violations in last analysis
                </Typography>
                {ruleViolationHistory.length < 2 ? "" :
                    <>
                        <Typography sx={{display: 'flex', alignItems: 'center', justifyContent: 'center', mb: 1}}>
                            {ruleViolationDifference < 0 ? <TrendingUpIcon color={iconColor}/>
                                : <TrendingDownIcon color={iconColor}/>}
                            {`${Math.abs(ruleViolationDifference)}`}
                        </Typography>
                        <Sparklines data={ruleViolationHistory}
                                    height={40}>
                            <SparklinesLine color={sparkLineColor} style={{fill: "none"}}/>
                        </Sparklines>
                    </>}
            </CardContent>
        </Card>
    )
}