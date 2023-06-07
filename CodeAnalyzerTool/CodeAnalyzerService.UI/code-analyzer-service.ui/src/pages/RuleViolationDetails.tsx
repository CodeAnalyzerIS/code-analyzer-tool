import {useParams} from "react-router-dom";
import {useRuleViolation} from "../hooks/useRuleViolation";
import Loading from "../components/Loading";
import {Alert, Box, Container, Divider, Grid, Paper, Stack, Typography, useTheme} from "@mui/material";
import React, {ReactElement} from "react";
import CategoryIcon from '@mui/icons-material/Category';
import RuleNameIcon from '@mui/icons-material/Fingerprint';
import ErrorIcon from '@mui/icons-material/Error';
import WarningIcon from '@mui/icons-material/Warning';
import InfoIcon from '@mui/icons-material/Info';
import PluginIcon from '@mui/icons-material/Extension';
import LanguageIcon from '@mui/icons-material/Terminal';
import {SEVERITY} from "../model/Severity";
import {RuleViolation} from "../model/RuleViolation";

export default function RuleViolationDetails() {
    const {id} = useParams<{ id: string}>();
    const {isLoading, isError, violation} = useRuleViolation(id!);
    const palette = useTheme().palette;

    if (isLoading) return <Loading/>;
    if (isError || !violation)return <Alert severity="error">Error loading the rule</Alert>;
    const rule = violation.rule;

    return(
        <Container maxWidth="lg" sx={{my: 2}}>
            <Typography variant="h3">{violation.message}</Typography>
            <Typography variant="subtitle2" mt={1} color={palette.primary.dark}>{rule.title}</Typography>
            <RuleViolationInfoBar violation={violation}/>
            <Typography>{rule.description}</Typography>
            {rule.codeExample && <CodeExample title={"Non-compliant code example"} code={rule.codeExample}/>}
            {rule.codeExampleFix && <CodeExample title={"Compliant solution example"} code={rule.codeExampleFix}/>}
        </Container>
    )

    // -------------------- helper functions------------------------

    function CodeExample({title, code} : {title: string, code: string}) {
        return <Box mt={3} component="section">
            <Typography variant="h4">{title}</Typography>
            <Paper sx={{p: 2, mt: 2, overflowX: "auto"}} elevation={0} variant="outlined">
                <code><pre style={{margin: 'unset'}}>{code}</pre></code>
            </Paper>
        </Box>;
    }
}


function RuleViolationInfoBar({violation} : {violation: RuleViolation}) {
    const palette = useTheme().palette;
    const rule = violation.rule;

    const ICON_COLOR = palette.secondary.main;

    return <>
        <Grid container my={1}>
            <RuleInfo label="Severity:" text={violation.severity ?? rule.defaultSeverity}
                      Icon={<SeverityIcon severity={violation.severity ?? rule.defaultSeverity}/>}/>
            <RuleInfo label="Name:" text={rule.ruleName} Icon={<RuleNameIcon htmlColor={ICON_COLOR}/>}/>
            <RuleInfo label="Category:" text={rule.category} Icon={<CategoryIcon htmlColor={ICON_COLOR}/>}/>
            <RuleInfo label="Plugin:" text={violation.rule.pluginName} Icon={<PluginIcon htmlColor={ICON_COLOR}/>}/>
            <RuleInfo label="Target language:" text={violation.rule.targetLanguage} Icon={<LanguageIcon htmlColor={ICON_COLOR}/>}/>
        </Grid>
        <Grid container mt={2}>
            <Grid item xs={12} lg={10}>
                <Typography variant="body2" fontWeight={300} sx={{overflow: "auto", whiteSpace: "nowrap"}}>{violation.location.path}</Typography>
            </Grid>
            <Grid item xs={12} lg={2}>
                <Typography variant="body2" fontWeight={300} textAlign="end" minWidth="100%">{violation.analysisDate.toLocaleString()}</Typography>
            </Grid>
        </Grid>
        <Divider sx={{mb: 2}}/>
    </>;


    // -------------------- helper functions------------------------

    function RuleInfo({label, text, Icon} : {label: string, text: string, Icon: ReactElement}) {
        return <Grid item xs={12} sm={6} md={4}>
            <Stack direction="row" spacing={1}>
                <Stack direction="row" spacing='4px'>
                    {Icon}
                    <Typography fontWeight={500}>{label}</Typography>
                </Stack>
                <Typography>{text}</Typography>
            </Stack>
        </Grid>;
    }

    function SeverityIcon({severity} : {severity: string}) {
        const palette = useTheme().palette;
        switch (severity) {
            case SEVERITY.INFO: return <InfoIcon htmlColor={palette.info.main}/>;
            case SEVERITY.WARNING: return <WarningIcon htmlColor={palette.warning.main}/>;
            case SEVERITY.ERROR: return <ErrorIcon  htmlColor={palette.error.main}/>;
            default: return <ErrorIcon/>;
        }
    }
}