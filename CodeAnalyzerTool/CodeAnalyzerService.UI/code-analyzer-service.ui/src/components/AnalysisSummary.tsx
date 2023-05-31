import {
    Accordion,
    AccordionDetails,
    AccordionSummary,
    Alert,
    AlertTitle,
    FormControl, Grid, InputLabel, MenuItem, Select, SelectChangeEvent,
    Stack,
    Typography
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import {getMUISeverity, groupRuleViolationsByPath} from "../util/HelperFunctions";
import React, {useState} from "react";
import {useAnalysis} from "../hooks/useAnalysis";
import Loading from "./Loading";
import {AnalysisWithViolationCount} from "../model/Analysis";

interface AnalysisSummaryProps {
    initialAnalysisId: number;
    analysisHistory: AnalysisWithViolationCount[];
}

export default function AnalysisSummary({initialAnalysisId, analysisHistory}: AnalysisSummaryProps) {
    const [analysisId, setAnalysisId] = useState(initialAnalysisId)
    const {isLoading, isError, analysis} = useAnalysis(analysisId)

    if (isLoading) {
        return <Loading/>
    }
    if (isError){
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
                <Grid item xs={3}>
                    <FormControl fullWidth>
                        <InputLabel id="demo-simple-select-label">{analysis.createdOn.toString()}</InputLabel>
                        <Select
                            labelId="select-analysis-label"
                            id="select-analysis"
                            value={analysis.id}
                            label={analysis.createdOn.toString()}
                            onChange={handleChange}
                        >
                            {analysisHistory.map((analysisItem, index) => (
                                <MenuItem key={index} value={analysisItem.id}>{analysisItem.createdOn.toString()}</MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </Grid>
                <Grid item xs={6}>
                    <Typography sx={{textAlign: 'center'}}>Analysis Results</Typography>
                </Grid>
                <Grid item xs={3}>
                    <Typography sx={{textAlign: 'end'}}>{analysis.ruleViolations.length} Rule Violations</Typography>
                </Grid>
            </Grid>
            {Object.entries(groupedAnalysis).map(([path, violations]) => (
                <Accordion defaultExpanded={true} key={path}>
                    <AccordionSummary expandIcon={<ExpandMoreIcon/>}>
                        <Typography>{path}</Typography>
                    </AccordionSummary>
                    <AccordionDetails sx={{padding: 'unset'}}>
                        <Stack>
                            {violations.map((violation, index) => (
                                <Alert key={index} severity={getMUISeverity(violation.severity)} sx={{paddingY: 'unset', borderRadius: 'unset'}}>
                                    <AlertTitle>{violation.message}</AlertTitle>
                                    <small>Line {violation.location.startLine}</small>
                                </Alert>
                            ))}
                        </Stack>
                    </AccordionDetails>
                </Accordion>
            ))}
        </Stack>
    )
}