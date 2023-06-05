import React, {useContext} from 'react';
import './App.css';
import {QueryClient, QueryClientProvider} from "react-query";
import {BrowserRouter, Link, Route, Routes} from "react-router-dom";
import Home from "./pages/Home";
import {AppBar, Avatar, Breadcrumbs, Stack, Toolbar, Typography} from "@mui/material";
import ProjectDetails from "./pages/ProjectDetails";
import RuleDetails from "./pages/RuleDetails";
import BreadcrumbContext, {IBreadcrumbContext} from './context/BreadcrumbContext';
import BreadcrumbContextProvider from "./context/BreadcrumbContextProvider";
import NotFound from "./pages/NotFound";

export let BACKEND_URL = ''

if (!process.env.NODE_ENV || process.env.NODE_ENV === 'development') {
    BACKEND_URL = 'http://localhost:5082'
}

const queryClient = new QueryClient()


function Header() {
    const {breadcrumbData} = useContext<IBreadcrumbContext>(BreadcrumbContext)

    return(
        <AppBar position="static" color="transparent">
            <Toolbar variant='dense' sx={{justifyContent: 'center'}}>
                <Stack direction='row' spacing={1} alignItems='center' sx={{position: 'absolute', left: 10}}>
                    <Avatar src={'/logo_transparent.png'}/>
                    <Breadcrumbs aria-label='breadcrumb'>
                        {breadcrumbData.map((breadcrumb, index) => (
                            <Link key={index} style={{color: "#6574FC"}} to={breadcrumb.path}>
                                {breadcrumb.label}
                            </Link>
                        ))}
                    </Breadcrumbs>
                </Stack>
                <Typography variant="h6" sx={{color: '#15B7B9'}}>Code Analyzer Tool</Typography>
            </Toolbar>
        </AppBar>
    )
}

function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <BreadcrumbContextProvider>
                <BrowserRouter>
                    <Header/>
                    <Routes>
                        <Route path="/" element={<Home/>}/>
                        <Route path="/project/:id" element={<ProjectDetails/>}/>
                        <Route path="/rule/:id" element={<RuleDetails/>}/>
                        <Route path="*" element={<NotFound/>}/>
                    </Routes>
                </BrowserRouter>
            </BreadcrumbContextProvider>
        </QueryClientProvider>
    );
}

export default App;
