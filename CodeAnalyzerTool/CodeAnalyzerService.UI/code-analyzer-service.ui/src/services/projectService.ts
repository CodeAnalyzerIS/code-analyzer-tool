import {BACKEND_URL} from "../App";
import {Project} from "../model/Project";
import {ProjectOverview} from "../model/ProjectOverview";
import {AnalysisWithViolationCount} from "../model/Analysis";

export async function getProject(id: string) {
    const response = await fetch(`${BACKEND_URL}/api/Project/${id}`);
    let projectResponse = await response.json() as Project
    return mapProject(projectResponse);
}

export async function getProjectOverviews() {
    const response = await fetch(`${BACKEND_URL}/api/Project/Overview`);
    let projectOverviewResponse = await response.json() as ProjectOverview[];
    return mapProjectOverviewDates(projectOverviewResponse);
}

export async function getProjectIdFromName(projectName: string) : Promise<number> {
    const response = await fetch(`${BACKEND_URL}/api/Project/GetFromName/${projectName}`);
    return await response.json();
}

function mapProjectOverviewDates(projectOverviews: ProjectOverview[]) : ProjectOverview[] {
    return projectOverviews.map((overview: ProjectOverview) => {
        const parsedDate = new Date(overview.lastAnalysisDate)
        return {...overview, lastAnalysisDate: parsedDate}
    });
}

function mapProject(project: Project) : Project {
    return {...project, analysisHistory: mapAnalysisHistoryDates(project.analysisHistory)};
}

function mapAnalysisHistoryDates(analysisHistory: AnalysisWithViolationCount[]) : AnalysisWithViolationCount[] {
    return analysisHistory.map((analysis) => {
        const parsedDate = new Date(analysis.createdOn)
        return {...analysis, createdOn: parsedDate}
    });
}