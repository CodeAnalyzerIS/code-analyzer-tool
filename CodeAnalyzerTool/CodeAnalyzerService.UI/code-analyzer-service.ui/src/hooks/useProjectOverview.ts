import {useQuery} from "react-query";
import {getProjectOverviews} from "../services/projectService";

export function useProjectOverview(){
    const {
        isLoading,
        isError,
        data: projectOverviews
    } = useQuery(['projectOverview'], () => getProjectOverviews())

    return {
        isLoading,
        isError,
        projectOverviews
    }
}