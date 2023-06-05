import {useQuery} from "react-query";
import {getRule} from "../services/ruleService";

export function useRule(id: string) {

    const {
        isLoading,
        isError,
        data: rule,
    } = useQuery(['rule', id], () => getRule(id))

    return {
        isLoading,
        isError,
        rule
    }
}