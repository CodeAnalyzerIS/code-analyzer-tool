import React, {useState} from 'react';
import logo from './logo.svg';
import './App.css';
import {QueryClient} from "react-query";

const queryClient = new QueryClient()

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={"logo_transparent.png"} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.tsx</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
    </div>
  );
}

export default App;
