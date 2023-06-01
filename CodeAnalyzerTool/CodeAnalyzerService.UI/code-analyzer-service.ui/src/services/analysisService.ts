import {BACKEND_URL} from "../App";
import {Analysis} from "../model/Analysis";

export async function getAnalysis(id: number) {
    const response = await fetch(`${BACKEND_URL}/api/Analysis/${id}`)
    const analysis = await response.json()
    return mapAnalysis(analysis);
}

function mapAnalysis(analysis: any): Analysis {
    return {
        createdOn: new Date(analysis.createdOn),
        id: analysis.id,
        ruleViolations: analysis.ruleViolations
    }
}