import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { generalInformationAndOfficesSchema, GeneralInformationAndOfficesType } from './general-information.dto';

@Injectable({
    providedIn: 'root',
})
export class GeneralInformationService {
    private apiUrl = '/admin/api/v1/Providers/current/information';

    constructor(private http: HttpClient) {}

    getGeneralInformationAndOffices = () => {
        return this.http.get(this.apiUrl).pipe(map(data => generalInformationAndOfficesSchema.parse(data)));
    };

    mutateGeneralInformationAndOffices = (payload: GeneralInformationAndOfficesType) => {
        return this.http.put(this.apiUrl, payload);
    };
}
