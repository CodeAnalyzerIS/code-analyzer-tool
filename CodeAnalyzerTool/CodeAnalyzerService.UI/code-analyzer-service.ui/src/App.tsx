import React from 'react';
import './App.css';
import {QueryClient, QueryClientProvider} from "react-query";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import Home from "./pages/Home";
import {createTheme, ThemeProvider} from '@mui/material/styles';
import ProjectDetails from "./pages/ProjectDetails";
import RuleDetails from "./pages/RuleDetails";
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
            main: '#15B7B9',
            dark: '#0e8586',
            light: '#15B7B9'
        },
    },
    typography: {
        h3: {
            fontSize: '2.5rem'
        },
        h4: {
            fontSize: '1.875rem'
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
                            <Route path="/rule/:id" >
                                <Route path=":severity" element={<RuleDetails/>} />
                                <Route path="" element={<RuleDetails/>} />
                            </Route>
                            <Route path="*" element={<NotFound/>}/>
                        </Routes>
                    </BrowserRouter>
                </BreadcrumbContextProvider>
            </ThemeProvider>
        </QueryClientProvider>
    );
}

export default App;
