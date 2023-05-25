export async function getProject(id: number) {
    const response = await fetch(`http://localhost:5082/api/Project/${id}`)
    return await response.json()
}