import {useQuery, useQueryClient} from "react-query";
import {getProject} from "../services/projectService";

export function useProject(id: number) {
    const queryClient = useQueryClient()

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