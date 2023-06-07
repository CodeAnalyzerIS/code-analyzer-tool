import {Container, InputBase, Paper, Stack} from "@mui/material";
import SearchIcon from '@mui/icons-material/Search';
import React from "react";

interface SearchbarProps {
    setSearchString: (value: string) => void;
}

export function Searchbar({setSearchString}: SearchbarProps) {
    return (
        <Container maxWidth="lg">
            <Paper sx={{p: 2}}>
                <Stack display="flex" direction="row" justifyContent="space-between" alignItems="center" width="100%">
                    <InputBase onChange={(e) => setSearchString(e.target.value)} placeholder="Search for a project"/>
                    <SearchIcon/>
                </Stack>
            </Paper>
        </Container>
    );
}