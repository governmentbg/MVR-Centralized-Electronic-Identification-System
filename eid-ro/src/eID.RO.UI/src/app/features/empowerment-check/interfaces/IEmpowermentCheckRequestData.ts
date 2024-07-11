import { OnBehalfOf } from '../enums/empowerment-check.enum';

export interface IEmpowermentCheckRequestData {
    onBehalfOf?: OnBehalfOf;
    volumeOfRepresentation?: string[];
    statusOn?: Date;
    serviceId: number;
    empoweredUid: string;
    authorizerUid: string;
    pageIndex: number;
    pageSize: number;
}
