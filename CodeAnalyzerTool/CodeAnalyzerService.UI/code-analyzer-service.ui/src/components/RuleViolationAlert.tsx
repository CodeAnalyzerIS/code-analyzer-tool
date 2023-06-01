import {getMUISeverity} from "../util/HelperFunctions";
import {Alert, AlertTitle} from "@mui/material";
import React from "react";
import {RuleViolation} from "../model/RuleViolation";

interface RuleViolationAlertProps {
    violation: RuleViolation;
}

export default function RuleViolationAlert({violation}: RuleViolationAlertProps) {
    return (
        <Alert severity={getMUISeverity(violation.severity)} sx={{paddingY: 'unset', borderRadius: 'unset'}}>
            <AlertTitle>{violation.message}</AlertTitle>
            <small>Line {violation.location.startLine}</small>
        </Alert>
    )
}