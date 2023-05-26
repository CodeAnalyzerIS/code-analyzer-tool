import {Analysis} from "./Analysis";

export type Project = {
    id: number;
    projectName: string;
    analyses: Analysis[];
}