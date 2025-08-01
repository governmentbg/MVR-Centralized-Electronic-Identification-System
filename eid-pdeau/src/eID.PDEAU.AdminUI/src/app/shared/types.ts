export type Nullable<T> = {
    [P in keyof T]: T[P] | null;
};

export type GenerateFormFilters<Filters, Picks extends keyof Filters> = Nullable<Partial<Pick<Filters, Picks>>>;

export interface PaginationData {
    pageIndex: number;
    pageSize: number;
}
export interface SortData {
    sortBy?: string;
    sortDirection?: 'asc' | 'desc';
}
