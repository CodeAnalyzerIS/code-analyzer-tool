import React, {useState} from 'react';
import './App.css';
import {QueryClient, QueryClientProvider} from "react-query";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import Home from "./components/Home";
import {AppBar, Avatar, Breadcrumbs, Link, Stack, Toolbar, Typography} from "@mui/material";

export const BACKEND_URL = 'http://localhost:5082';

const queryClient = new QueryClient()
export interface Breadcrumb {
    label: string;
    path: string
}

interface HeaderProps {
    breadcrumbData: Breadcrumb[]
}

const Header = ({breadcrumbData}: HeaderProps) => (
    <AppBar position="static" color="transparent">
        <Toolbar variant='dense' sx={{justifyContent: 'center'}}>
            <Stack direction='row' spacing={1} alignItems='center' sx={{position: 'absolute', left: 10}}>
                <Avatar src={'logo_transparent.png'}/>
                <Breadcrumbs aria-label='breadcrumb'>
                    {breadcrumbData.map((breadcrumb, index) => (
                        <Link key={index} underline='hover' color="#6574FC" href={breadcrumb.path}>
                            {breadcrumb.label}
                        </Link>
                    ))}
                </Breadcrumbs>
            </Stack>
            <Typography variant="h6" sx={{color: '#15B7B9'}}>Code Analyzer Tool</Typography>
        </Toolbar>
    </AppBar>
);

function App() {
    const [breadcrumbData, setBreadcrumbData] =
        useState<Breadcrumb[]>([{label: 'ᓚᘏᗢ', path:'/'}])
    return (
        <QueryClientProvider client={queryClient}>
            <BrowserRouter>
                <Header breadcrumbData={breadcrumbData}/>
                <Routes>
                    <Route path="/" element={<Home/>}/>
                </Routes>
            </BrowserRouter>
        </QueryClientProvider>
    );
}

export default App;
