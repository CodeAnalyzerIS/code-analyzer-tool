import React from 'react';
import './App.css';
import {QueryClient, QueryClientProvider} from "react-query";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import Home from "./components/Home";
import {AppBar, Avatar, Button, IconButton, Toolbar, Typography} from "@mui/material";

const queryClient = new QueryClient()

const Header = () => (
    <AppBar position="static" color="transparent">
        <Toolbar sx={{justifyContent: "space-between"}}>
            <div className="headerBar">
                <Avatar src={'logo_transparent.png'}/>
                <Typography variant="h6" sx={{justifySelf: "center"}}>Code Analyzer Tool</Typography>
            </div>
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
