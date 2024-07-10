export interface ISystem {
    id: string;
    name: string;
    isApproved: boolean;
    isDeleted: boolean;
    events: INotificationEvent[];
    translations: { language: string; name: string }[];
}

export interface IRejectedSystem {
    id: string;
    name: string;
    translations: { language: string; name: string }[];
}

export interface INotificationEvent {
    id: string;
    code: string;
    isMandatory: boolean;
    isDeleted: boolean;
    translations: { language: string; shortDescription: string; description: string }[];
}

export interface ISystemPaginatedData {
    data: ISystem[];
    pageIndex: number;
    totalItems: number;
}

export interface IRejectedSystemPaginatedData {
    data: IRejectedSystem[];
    pageIndex: number;
    totalItems: number;
}

export interface IGetSystemsParams {
    includeDeleted?: boolean;
    pageIndex: number;
    pageSize: number;
}
