import {BACKEND_URL} from "../App";

export async function getProject(id: number) {
    const response = await fetch(`${BACKEND_URL}/api/Project/${id}`)
    return await response.json()
}

export async function getProjectOverviews() {
    const response = await fetch(`${BACKEND_URL}/api/Project/overview`)
    return await response.json()
}