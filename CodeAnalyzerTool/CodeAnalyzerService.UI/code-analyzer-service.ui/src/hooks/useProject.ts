import {useQuery} from "react-query";
import {getProject} from "../services/projectService";

export function useProject(id: string) {

    const {
        isLoading,
        isError,
        data: project,
    } = useQuery(['project', id], () => getProject(id))

    return {
        isLoading,
        isError,
        project
    }
}