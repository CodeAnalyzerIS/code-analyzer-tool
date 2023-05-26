import {CircularProgress} from "@mui/material";

export default function Loading({ text = "" }) {
    return (
        <div
            style={{
                width: 400,
                height: 250,
                marginTop: 50,
                marginLeft: "auto",
                marginRight: "auto",
            }}
        >
            <div
                style={{
                    display: "flex",
                    flexDirection: "column",
                    textAlign: "center",
                }}
            >
                <div style={{ padding: 40 }}>
                    <h2>{text}</h2>
                </div>
                <div>
                    <CircularProgress />
                </div>
            </div>
        </div>
    );
}