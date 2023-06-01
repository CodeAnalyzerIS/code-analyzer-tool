import {createContext} from "react";

export interface Breadcrumb {
    label: string;
    path: string
}

export interface IBreadcrumbContext {
    breadcrumbData: Breadcrumb[];
    setBreadcrumbData: (breadcrumbData: Breadcrumb[]) => void;
}

export default createContext<IBreadcrumbContext>({
    breadcrumbData: [],
    setBreadcrumbData: () => {}
})