import {useQuery} from "react-query";
import {getRuleViolation} from "../services/ruleViolationService";

export function useRuleViolation(id: string) {

    const {
        isLoading,
        isError,
        data: ruleViolation,
    } = useQuery(['rule', id], () => getRuleViolation(id))

    return {
        isLoading,
        isError,
        violation: ruleViolation
    }
}