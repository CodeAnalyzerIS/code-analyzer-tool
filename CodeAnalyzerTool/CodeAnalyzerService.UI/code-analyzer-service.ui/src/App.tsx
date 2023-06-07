import React from 'react';
import './App.css';
import {QueryClient, QueryClientProvider} from "react-query";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import Home from "./pages/Home";
import {createTheme, ThemeProvider} from '@mui/material/styles';
import ProjectDetails from "./pages/ProjectDetails";
import RuleViolationDetails from "./pages/RuleViolationDetails";
import BreadcrumbContextProvider from "./context/BreadcrumbContextProvider";
import NotFound from "./pages/NotFound";
import Header from './components/Header';

export let BACKEND_URL = ''

if (!process.env.NODE_ENV || process.env.NODE_ENV === 'development') {
    BACKEND_URL = 'http://localhost:5082'
}

const queryClient = new QueryClient()
const theme = createTheme({
    palette: {
        primary: {
            main: '#12B1B4',
            dark: '#12B1B4',
            light: '#12B1B4'
        },
        secondary: {
            main: '#E27E65',
            light: '#e89b88',
            dark: '#DB6143',
        },
    },
    typography: {
        h3: {
            fontSize: '2.125rem'
        },
        h4: {
            fontSize: '1.875rem'
        },
        subtitle2: {
            fontSize: '1.375rem',
            fontWeight: 300
        }
    }
});

function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <ThemeProvider theme={theme}>
                <BreadcrumbContextProvider>
                    <BrowserRouter>
                        <Header/>
                        <Routes>
                            <Route path="/" element={<Home/>}/>
                            <Route path="/project/:id" element={<ProjectDetails/>}/>
                            <Route path="/ruleViolation/:id" element={<RuleViolationDetails/>}/>
                            <Route path="*" element={<NotFound/>}/>
                        </Routes>
                    </BrowserRouter>
                </BreadcrumbContextProvider>
            </ThemeProvider>
        </QueryClientProvider>
    );
}

export default App;
