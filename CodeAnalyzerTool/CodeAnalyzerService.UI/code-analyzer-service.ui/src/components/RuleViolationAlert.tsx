import {getMUISeverity} from "../util/HelperFunctions";
import {Alert, AlertTitle} from "@mui/material";
import React from "react";
import {RuleViolation} from "../model/RuleViolation";
import {useNavigate} from "react-router-dom";

interface RuleViolationAlertProps {
    violation: RuleViolation;
}

export default function RuleViolationAlert({violation}: RuleViolationAlertProps) {
    const navigate = useNavigate();

    return (
        <Alert severity={getMUISeverity(violation.severity)} sx={{paddingY: 'unset', borderRadius: 'unset', cursor: 'pointer'}}
               onClick={() => navigate(`/ruleViolation/${violation.id}`)}>
            <AlertTitle sx={{fontWeight: 400}}>{violation.message}</AlertTitle>
            <small>Line {violation.location.startLine}</small>
        </Alert>
    )
}