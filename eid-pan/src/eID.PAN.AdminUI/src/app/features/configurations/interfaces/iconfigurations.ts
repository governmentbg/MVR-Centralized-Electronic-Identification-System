export interface IConfigurationsPaginatedData {
    data: IConfiguration[];
    pageIndex: number;
    totalItems: number;
}

export interface IConfiguration {
    id?: string;
    server: string;
    port: number;
    securityProtocol: securityProtocol;
    userName: string;
    password?: string;
}

export interface IGetConfigurationsParams {
    pageIndex: number;
    pageSize: number;
}

export enum securityProtocol {
    'SSL' = 'SSL',
    'TLS' = 'TLS',
}
