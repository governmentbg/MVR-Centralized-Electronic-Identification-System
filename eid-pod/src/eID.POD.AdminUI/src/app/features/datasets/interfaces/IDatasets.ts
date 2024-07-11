export interface IDatasetRequestPayload {
    id?: string;
    datasetName: string;
    cronPeriod: string;
    dataSource: string;
    isActive: boolean;
}

export interface IDataset {
    id: string;
    datasetName: string;
    cronPeriod: string;
    dataSource: string;
    datasetUri?: string;
    isActive: boolean;
    isDeleted?: true;
    lastRun?: string;
    createdBy?: string;
}

export interface IDatasets {
    data: Array<IDataset>;
}
