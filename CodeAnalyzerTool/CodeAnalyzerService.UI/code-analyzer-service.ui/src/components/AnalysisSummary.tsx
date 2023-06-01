import {
    Alert,
    FormControl, Grid, MenuItem, Select, SelectChangeEvent,
    Stack,
    Typography
} from "@mui/material";
import {groupRuleViolationsByPath} from "../util/HelperFunctions";
import React, {useState} from "react";
import {useAnalysis} from "../hooks/useAnalysis";
import Loading from "./Loading";
import {AnalysisHistory} from "../model/Analysis";
import FileViolations from "./FileViolations";

interface AnalysisSummaryProps {
    initialAnalysisId: number;
    analysisHistory: AnalysisHistory[];
}

export default function AnalysisSummary({initialAnalysisId, analysisHistory}: AnalysisSummaryProps) {
    const [analysisId, setAnalysisId] = useState(initialAnalysisId)
    const {isLoading, isError, analysis} = useAnalysis(analysisId)

    if (isLoading) {
        return <Loading/>
    }
    if (isError || !analysis){
        return <Alert severity="error">Error loading the analysis</Alert>
    }
    const groupedAnalysis = groupRuleViolationsByPath(analysis)
    const handleChange = (event: SelectChangeEvent) => {
        const newAnalysisId = Number(event.target.value)
        setAnalysisId(newAnalysisId);
    };

    return (
        <Stack sx={{width: '60%', mt: 3}} spacing={2}>
            <Grid container alignItems='center'>
                <Grid item xs={2}>
                    <FormControl fullWidth>
                        <Select
                            labelId="select-analysis-label"
                            id="select-analysis"
                            value={analysis.id.toString()}
                            onChange={handleChange}
                        >
                            {analysisHistory.map((analysisItem, index) => (
                                <MenuItem key={index} value={analysisItem.id}>{analysisItem.createdOn.toLocaleString('nl-BE')}</MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </Grid>
                <Grid item xs={8}>
                    <Typography sx={{textAlign: 'center'}}>Analysis Results</Typography>
                </Grid>
                <Grid item xs={2}>
                    <Typography sx={{textAlign: 'end'}}><strong>{analysis.ruleViolations.length}</strong> Rule Violations</Typography>
                </Grid>
            </Grid>
            {Object.entries(groupedAnalysis).map(([path, violations]) => (
                <FileViolations path={path} violations={violations}/>
            ))}
        </Stack>
    )
}