import {InputAdornment, TextField} from "@mui/material";
import SearchIcon from '@mui/icons-material/Search';
import React from "react";

interface SearchbarProps {
    setSearchString: (value: string) => void;
}

export function Searchbar({setSearchString}: SearchbarProps) {
    return <TextField sx={{backgroundColor: 'white'}}
                      variant="outlined"
                      fullWidth
                      InputProps={{endAdornment: <InputAdornment position="start"><SearchIcon/></InputAdornment>}}
                      onChange={(e) => setSearchString(e.target.value)} placeholder="Search for a project"/>;
}