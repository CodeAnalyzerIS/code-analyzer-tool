import {SearchString} from "./Home";
import {IconButton, InputBase, Paper} from "@mui/material";
import {Controller, useForm} from "react-hook-form";
import SearchIcon from '@mui/icons-material/Search';

interface SearchbarProps {
    setSearchString: (value: string) => void;
}

export function Searchbar({setSearchString}: SearchbarProps) {

    const {
        control,
        handleSubmit,
        reset,
    } = useForm({
        defaultValues: {
            searchValue: "",
        },
    });

    const _onSubmit = (data: SearchString) => {
        setSearchString(data.searchValue)
        reset();
    }

    return (
        <Paper
            sx={{p: '2px 4px', display: 'flex', alignItems: 'center', width: "50vw", margin: "1rem"}}
        >
            <form onSubmit={handleSubmit(_onSubmit)}>
                <div style={{display: "flex", flexDirection: "row", justifyContent: "space-between", width: "50vw"}}>
                    <Controller
                        name="searchValue"
                        control={control}
                        render={({field}) => (
                            <InputBase
                                sx={{width: "95%"}}
                                placeholder="Search for a project"
                                {...field}
                            />
                        )}
                    />
                    <IconButton type="submit" sx={{p: '10px'}} aria-label="search">
                        <SearchIcon/>
                    </IconButton>
                </div>
            </form>
        </Paper>
    );
}