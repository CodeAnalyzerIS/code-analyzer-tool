import {ReactElement} from "react";
import BreadcrumbContext, {Breadcrumb} from "./BreadcrumbContext";
import useLocalStorage from "../hooks/useLocalStorage";

interface IWithChildren {
    children: ReactElement | ReactElement[];
}

export default function BreadcrumbContextProvider({ children }: IWithChildren) {
    const [breadcrumbData, setBreadcrumbData] = useLocalStorage<Breadcrumb[]>(
        "BreadcrumbData",
        []
    );

    return (
        <BreadcrumbContext.Provider value={{breadcrumbData: breadcrumbData ?? [], setBreadcrumbData}}>
            {children}
        </BreadcrumbContext.Provider>
    )
}