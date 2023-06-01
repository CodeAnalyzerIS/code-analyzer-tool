import {useQuery} from "react-query";
import {getAnalysis} from "../services/analysisService";

export function useAnalysis(id: number) {

    const {
        isLoading,
        isError,
        data: analysis,
    } = useQuery(['analysis', id], () => getAnalysis(id))

    return {
        isLoading,
        isError,
        analysis
    }
}