import React from 'react';
import './App.css';
import {QueryClient, QueryClientProvider} from "react-query";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import Home from "./components/Home";
import {AppBar, Avatar, Toolbar, Typography} from "@mui/material";

export const BACKEND_URL = 'http://localhost:5082';

const queryClient = new QueryClient()

const Header = () => (
    <AppBar position="static" color="transparent">
        <Toolbar>
            <Avatar src={'logo_transparent.png'}/>
            <Typography variant="h6" sx={{color: '#15B7B9', justifySelf: "center"}}>Code Analyzer Tool</Typography>
        </Toolbar>
    </AppBar>
);

function App() {
  return (
      <QueryClientProvider client={queryClient}>
            <BrowserRouter>
                <Header/>
                <Routes>
                    <Route path="/" element={<Home/>}></Route>
                </Routes>
            </BrowserRouter>
      </QueryClientProvider>
  );
}

export default App;
