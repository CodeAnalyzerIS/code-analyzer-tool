import {Accordion, AccordionDetails, AccordionSummary, Stack, Typography} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import React from "react";
import {RuleViolation} from "../model/RuleViolation";
import RuleViolationAlert from "./RuleViolationAlert";

interface FileViolationsProps {
    path: string;
    violations: RuleViolation[];
    id: string;
}

export default function FileViolations({path, violations, id}: FileViolationsProps) {
    return (
        <Accordion id={id} defaultExpanded={true} key={path}>
            <AccordionSummary expandIcon={<ExpandMoreIcon/>}>
                <Typography fontWeight={300}>{path}</Typography>
            </AccordionSummary>
            <AccordionDetails sx={{padding: 'unset'}}>
                <Stack>
                    {violations.map((violation, index) => (
                        <RuleViolationAlert key={index} violation={violation}/>
                    ))}
                </Stack>
            </AccordionDetails>
        </Accordion>
    )
}