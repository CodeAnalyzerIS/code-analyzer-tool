import {useParams} from "react-router-dom";
import {useRule} from "../hooks/useRule";
import Loading from "../components/Loading";
import {Alert, Box, Container, Divider, Grid, Paper, Stack, Typography, useTheme} from "@mui/material";
import React, {ComponentType, ReactElement} from "react";
import CategoryIcon from '@mui/icons-material/Category';
import RuleNameIcon from '@mui/icons-material/Fingerprint';
import ErrorIcon from '@mui/icons-material/Error';
import WarningIcon from '@mui/icons-material/Warning';
import InfoIcon from '@mui/icons-material/Info';
import {SEVERITY} from "../constants";
import {Rule} from "../model/Rule";

export default function RuleDetails(props: any) {
    const {id, severity} = useParams<{ id: string, severity: string }>();
    const {isLoading, isError, rule} = useRule(id!);
    const palette = useTheme().palette;

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

        return <Grid container my={1}>
            <RuleInfo label="Name:" text={rule.ruleName} Icon={<RuleNameIcon htmlColor={palette.secondary.main}/>}/>
            <RuleInfo label="Category:" text={rule.category} Icon={<CategoryIcon htmlColor={palette.secondary.main}/>}/>
            <RuleInfo label="Severity:" text={severity ?? rule.defaultSeverity}
                      Icon={<SeverityIcon severity={severity ?? rule.defaultSeverity}/>}/>
        </Grid>;
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

    function CodeExample({title, code} : {title: string, code: string}) {
        return <Box mt={3}>
            <Typography variant="h4">{title}</Typography>
            <Paper sx={{p: 2, mt: 2, overflowX: "auto"}} elevation={0} variant="outlined">
                <code><pre style={{margin: 'unset'}}>{code}</pre></code>
            </Paper>
        </Box>;
    }

    if (isLoading) return <Loading/>;
    if (isError || !rule)return <Alert severity="error">Error loading the rule</Alert>;

    return(
        <Container maxWidth="lg" sx={{my: 2}}>
            <Typography variant="h3" color={palette.primary.dark}>{rule.title}</Typography>
            <RuleInfoBar rule={rule}/>

            <Divider sx={{my: 2}}/>
            <Typography>{rule.description}</Typography>

            {rule.codeExample && <CodeExample title={"Non-compliant code example"} code={rule.codeExample}/>}
            {rule.codeExampleFix && <CodeExample title={"Compliant solution example"} code={rule.codeExampleFix}/>}
        </Container>
    )
}