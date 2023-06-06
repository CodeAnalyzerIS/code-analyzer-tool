import {useParams} from "react-router-dom";
import {useRule} from "../hooks/useRule";
import Loading from "../components/Loading";
import {Alert, Box, Container, Paper, Stack, Typography, useTheme} from "@mui/material";
import React, {ComponentType, ReactElement} from "react";
import CategoryIcon from '@mui/icons-material/Category';
import RuleNameIcon from '@mui/icons-material/Fingerprint';
import ErrorIcon from '@mui/icons-material/Error';
import WarningIcon from '@mui/icons-material/Warning';
import InfoIcon from '@mui/icons-material/Info';
import {SEVERITY} from "../constants";
import {Rule} from "../model/Rule";

export default function RuleDetails() {
    const {id, severity} = useParams<{ id: string, severity: string }>();
    const {isLoading, isError, rule} = useRule(id!);

    // const updateBreadcrumbData = useCallback(() => {
    //     const newBreadcrumbData: Breadcrumb[] = [
    //         {label: 'ᓚᘏᗢ', path:'/'},
    //         {label: project ? project.projectName : '', path:`/project/${id}`}
    //     ];
    //     setBreadcrumbData(newBreadcrumbData);
    // }, [id, project, setBreadcrumbData]);
    //
    // useEffect(() => {
    //     updateBreadcrumbData();
    // }, [updateBreadcrumbData]);


    function RuleInfoBar({rule} : {rule: Rule}) {
        const palette = useTheme().palette;

        function RuleInfo({label, text, Icon} : {label: string, text: string, Icon: ReactElement}) {
            return <Stack direction="row" spacing={1}>
                <Stack direction="row" spacing='4px'>
                    {Icon}
                    <Typography fontWeight={500}>{label}</Typography>
                </Stack>
                <Typography>{text}</Typography>
            </Stack>
        }

        return <Stack direction="row" spacing={5}>
            <RuleInfo label="Name:" text={rule.ruleName} Icon={<RuleNameIcon htmlColor={palette.secondary.main}/>}/>
            <RuleInfo label="Category:" text={rule.category} Icon={<CategoryIcon htmlColor={palette.secondary.main}/>}/>
            <RuleInfo label="Severity:" text={severity ?? rule.defaultSeverity}
                      Icon={<SeverityIcon severity={severity ?? rule.defaultSeverity}/>}/>
        </Stack>;
    }

    function SeverityIcon({severity} : {severity: string}) {
        const palette = useTheme().palette;
        switch (severity) {
            case SEVERITY.Info: return <InfoIcon htmlColor={palette.info.main}/>;
            case SEVERITY.Warning: return <WarningIcon htmlColor={palette.warning.main}/>;
            case SEVERITY.Error: return <ErrorIcon  htmlColor={palette.error.main}/>;
            default: return <ErrorIcon/>;
        }
    }

    if (isLoading) return <Loading/>;
    if (isError || !rule)return <Alert severity="error">Error loading the rule</Alert>;

    return(
        <Container maxWidth="lg">
            <Typography variant="h2">{rule.title}</Typography>
            <RuleInfoBar rule={rule}/>
            <Typography>{rule.description}</Typography>

            {rule.codeExample && <Paper>
                <Typography variant="h5">Noncompliant code example</Typography>
                <code><pre>{rule.codeExample}</pre></code>
            </Paper>}
            {rule.codeExampleFix &&<Paper>
                <Typography variant="h5">Compliant solution example</Typography>
                <code><pre>{rule.codeExampleFix}</pre></code>
            </Paper>}
        </Container>
    )
}