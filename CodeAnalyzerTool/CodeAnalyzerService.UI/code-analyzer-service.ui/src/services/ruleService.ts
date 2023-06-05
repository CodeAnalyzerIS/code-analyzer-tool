import {BACKEND_URL} from "../App";
import {Rule} from "../model/Rule";

export async function getRule(id: string) {
    const response = await fetch(`${BACKEND_URL}/api/Rule/${id}`);
    return await response.json() as Rule;
}