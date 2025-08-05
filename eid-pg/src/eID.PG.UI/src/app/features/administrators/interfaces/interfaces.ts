import { IEidAdministratorOffice } from '../../eid-management/interfaces/eid-management.interface';
import { Actions } from '../enums/enums';

export interface ICoordinatesEventEmitter {
    office: IEidAdministratorOffice;
    latitude: number;
    longitude: number;
    action: Actions;
}

export interface IHideOfficesEventEmitter {
    showPreview: boolean;
    refreshTable: boolean;
}

export interface ICity {
    city: string;
    lng: string;
    lat: string;
}
