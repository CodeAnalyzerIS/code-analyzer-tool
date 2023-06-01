import {ReactElement, useState} from "react";
import BreadcrumbContext, {Breadcrumb} from "./BreadcrumbContext";

interface IWithChildren {
    children: ReactElement | ReactElement[];
}

export default function BreadcrumbContextProvider({ children }: IWithChildren) {
    const [breadcrumbData, setBreadcrumbData] = useState<Breadcrumb[]>([]);

    return (
        <BreadcrumbContext.Provider value={{breadcrumbData: breadcrumbData ?? [], setBreadcrumbData}}>
            {children}
        </BreadcrumbContext.Provider>
    )
}