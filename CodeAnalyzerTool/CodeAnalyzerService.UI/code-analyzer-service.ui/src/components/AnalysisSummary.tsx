import {
    Alert, Container, Divider,
    FormControl, Grid, MenuItem, Select, SelectChangeEvent,
    Typography
} from "@mui/material";
import {
    createFolderHierarchy,
    groupRuleViolationsByPath,
} from "../services/ruleViolationService";
import React, {useState} from "react";
import {useAnalysis} from "../hooks/useAnalysis";
import Loading from "./Loading";
import {AnalysisHistory} from "../model/Analysis";
import FileViolations from "./FileViolations";
import DonePlaceholder from "./placeholders/DonePlaceholder";
import { FolderTree } from "./FolderTree";

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
    const folderHierarchy = createFolderHierarchy(Object.keys(groupedAnalysis))


    const handleChange = (event: SelectChangeEvent) => {
        const newAnalysisId = Number(event.target.value)
        setAnalysisId(newAnalysisId);
    };

    function SummaryHead({analysisId, ruleViolationCount}: {analysisId: string, ruleViolationCount: number}) {
        return <Grid container alignItems='center'>
            <Grid item xs={12} sm={4} md={3} lg={2}>
                <FormControl fullWidth>
                    <Select
                        labelId="select-analysis-label"
                        id="select-analysis"
                        value={analysisId}
                        onChange={handleChange}
                    >
                        {analysisHistory.map((analysisItem, index) => (
                            <MenuItem key={index} value={analysisItem.id}>
                                <Typography textAlign="center">
                                    {analysisItem.createdOn.toLocaleString('nl-BE')}
                                </Typography>
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>
            </Grid>
            <Grid item xs={6} sm={4} md={6} lg={8} pt={1}>
                <Typography sx={{textAlign: 'center'}}>Analysis Results</Typography>
            </Grid>
            <Grid item xs={6} sm={4} md={3} lg={2} pt={1}>
                <Typography sx={{textAlign: 'center'}}>
                    <strong>{ruleViolationCount}</strong> Rule Violations
                </Typography>
            </Grid>
        </Grid>;
    }

    return (
        <Container sx={{mt: 3}} maxWidth="lg">
            <SummaryHead analysisId={analysis.id.toString()} ruleViolationCount={analysis.ruleViolations.length}/>
            <Divider sx={{mt: 2}}/>
            {analysis.ruleViolations.length < 1 ? <DonePlaceholder/>
                :
                <>
                    {Object.entries(groupedAnalysis).map(([path, violations], index) => (
                        <FileViolations id={`path-${index}`} key={index} path={path} violations={violations}/>
                    ))}
                    {folderHierarchy && folderHierarchy.length > 0 && <FolderTree folders={folderHierarchy}/>}
                </>
            }
        </Container>
    )
}