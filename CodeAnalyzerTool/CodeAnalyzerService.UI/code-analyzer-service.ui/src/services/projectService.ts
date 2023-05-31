import {BACKEND_URL} from "../App";

export async function getProject(id: string) {
    const response = await fetch(`${BACKEND_URL}/api/Project/${id}`)
    return await response.json()
}

export async function getProjectOverviews() {
    const response = await fetch(`${BACKEND_URL}/api/Project/Overview`)
    return await response.json()
}

export async function getProjectIdFromName(projectName: string) {
    const response = await fetch(`${BACKEND_URL}/api/Project/GetFromName/${projectName}`)
    return await response.json()
}