export interface IAppConfig {
    journals: IJournalsConfig;
    integrity: IIntegrityConfig;
}

export interface IJournalsConfig {
    fileExtensionFilter?: string;
    folders?: Array<string>;
}

export interface IIntegrityConfig {
    systemId?: string;
    poolingRequestInterval?: number;
    poolingTimeout?: number;
    downloadBytesFileSize: number;
}
