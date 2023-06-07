import {Stack} from "@mui/material";
import React, {ReactElement} from "react";

export function IconAndText({icon, text}: {icon: ReactElement, text: ReactElement}) {
    return <Stack direction='row' spacing={1}>
        {icon}
        {text}
    </Stack>;
}