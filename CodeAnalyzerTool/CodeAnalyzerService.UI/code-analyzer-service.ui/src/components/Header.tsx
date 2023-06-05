import React, {useContext} from "react";
import BreadcrumbContext, {IBreadcrumbContext} from "../context/BreadcrumbContext";
import {AppBar, Avatar, Breadcrumbs, Stack, Toolbar, Typography, useTheme} from "@mui/material";
import {Link} from "react-router-dom";

export default function Header() {
    const {breadcrumbData} = useContext<IBreadcrumbContext>(BreadcrumbContext)
    const palette = useTheme().palette;

    return(
        <AppBar position="static" color="transparent">
            <Toolbar variant='dense' sx={{justifyContent: 'center'}}>
                <Stack direction='row' spacing={1} alignItems='center' sx={{position: 'absolute', left: 10}}>
                    <Avatar src={'/logo_transparent.png'}/>
                    <Breadcrumbs aria-label='breadcrumb'>
                        {breadcrumbData.map((breadcrumb, index) => (
                            // todo color
                            <Link key={index} style={{color: "#6574FC"}} to={breadcrumb.path}>
                                {breadcrumb.label}
                            </Link>
                        ))}
                    </Breadcrumbs>
                </Stack>
                <Typography variant="h6" sx={{color: palette.primary.main}}>Code Analyzer Tool</Typography>
            </Toolbar>
        </AppBar>
    )
}