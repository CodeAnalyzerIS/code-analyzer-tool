import {BACKEND_URL} from "../App";

export async function getAnalysis(id: number) {
    const response = await fetch(`${BACKEND_URL}/api/Analysis/${id}`)
    return await response.json()
}