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
import PathIcon from '@mui/icons-material/LocationOn';
import LineNumberIcon from '@mui/icons-material/FormatListNumbered';
import {SEVERITY} from "../model/Severity";
import {RuleViolation} from "../model/RuleViolation";

export default function RuleViolationDetails() {
    const {id} = useParams<{ id: string}>();
    const {isLoading, isError, violation} = useRuleViolation(id!);
    const palette = useTheme().palette;

    function RuleViolationInfoBar({violation} : {violation: RuleViolation}) {
        const palette = useTheme().palette;
        const rule = violation.rule;

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
        const ICON_COLOR = palette.secondary.main;

        return <Grid container my={1}>
            <RuleInfo label="Name:" text={rule.ruleName} Icon={<RuleNameIcon htmlColor={ICON_COLOR}/>}/>
            <RuleInfo label="Category:" text={rule.category} Icon={<CategoryIcon htmlColor={ICON_COLOR}/>}/>
            <RuleInfo label="Severity:" text={violation.severity ?? rule.defaultSeverity}
                      Icon={<SeverityIcon severity={violation.severity ?? rule.defaultSeverity}/>}/>
            <RuleInfo label="Plugin:" text={violation.rule.pluginName} Icon={<PluginIcon htmlColor={ICON_COLOR}/>}/>
            <RuleInfo label="Target language:" text={violation.rule.targetLanguage} Icon={<LanguageIcon htmlColor={ICON_COLOR}/>}/>
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

    function CodeExample({title, code} : {title: string, code: string}) {
        return <Box mt={3} component="section">
            <Typography variant="h4">{title}</Typography>
            <Paper sx={{p: 2, mt: 2, overflowX: "auto"}} elevation={0} variant="outlined">
                <code><pre style={{margin: 'unset'}}>{code}</pre></code>
            </Paper>
        </Box>;
    }

    if (isLoading) return <Loading/>;
    if (isError || !violation)return <Alert severity="error">Error loading the rule</Alert>;
    const rule = violation.rule;

    return(
        <Container maxWidth="lg" sx={{my: 2}}>
            <Typography variant="h3">{violation.message}</Typography>
            <Typography variant="subtitle2" mt={1} color={palette.primary.dark}>{rule.title}</Typography>
            <RuleViolationInfoBar violation={violation}/>
            <Typography variant="body2" textAlign="end">{violation.analysisDate.toLocaleString()}</Typography>
            <Divider sx={{mb: 2}}/>
            <Typography>{rule.description}</Typography>

            <Box mt={2} component="section">
                <Typography variant="h4" mb={1}>Where is the rule violation?</Typography>
                <Stack direction="row" spacing='4px'>
                    <PathIcon htmlColor={palette.primary.main}/>
                    <Typography fontWeight={300}>{violation.location.path}</Typography>
                </Stack>
                <Stack direction="row" spacing='4px'>
                    <LineNumberIcon htmlColor={palette.primary.main}/>
                    <Typography fontWeight={300}>{violation.location.startLine} - {violation.location.endLine}</Typography>
                </Stack>

            </Box>


            {rule.codeExample && <CodeExample title={"Non-compliant code example"} code={rule.codeExample}/>}
            {rule.codeExampleFix && <CodeExample title={"Compliant solution example"} code={rule.codeExampleFix}/>}
        </Container>
    )
}