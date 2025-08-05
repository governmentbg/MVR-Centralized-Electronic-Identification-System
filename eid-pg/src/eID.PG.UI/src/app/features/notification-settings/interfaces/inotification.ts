export interface INotification {
    id: string;
    name: string;
    isApproved: boolean;
    isDeleted: boolean;
    events: INotificationEvent[];
    translations: { language: string; name: string; description: string }[];
    indeterminate?: null | boolean;
    disabled?: boolean;
}

export interface INotificationEvent {
    id: string;
    code: string;
    isMandatory: boolean;
    isDeleted: boolean;
    translations: { language: string; shortDescription: string; description: string }[];
    activated?: boolean;
}

export interface IChannel {
    id: string;
    systemId: string;
    name: string;
    description: string;
    callbackUrl: string;
    price: 0;
    infoUrl: string;
    translations: { language: string; name: string; description: string }[];
}

export interface IUserNotificationChannelPaginatedData {
    data: IUserNotificationChannel[];
    pageIndex: number;
    totalItems: number;
}

export interface IUserNotificationChannel {
    id: string;
    infoUrl: string;
    name: string;
    price: number;
    translations: { language: string; name: string; description: string }[];
    activated?: boolean;
}

export interface IUserNotificationChannelParams {
    pageSize: number;
    pageIndex: number;
}
