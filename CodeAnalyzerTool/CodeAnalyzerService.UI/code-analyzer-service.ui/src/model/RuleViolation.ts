import {Rule} from "./Rule";
import {Location} from "./Location";

export type RuleViolation = {
    id: number;
    rule: Rule;
    message: string;
    location: Location;
    severity: string;
}