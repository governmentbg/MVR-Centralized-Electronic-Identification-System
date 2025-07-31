import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
    IGetCertificateInformationRequestData,
    IGetCertificateInformationResponseData,
    IPINRequestData,
    ISaveCertificateRequestData,
} from '../interfaces/eid-management.interfaces';

@Injectable({
    providedIn: 'root',
})
export class SmartCardCommunicationClientService {
    private apiUrl = 'https://localhost:5024/api/v1';

    constructor(private http: HttpClient) {}

    getReaders(): Observable<string[]> {
        return this.http.get<any>(`${this.apiUrl}/readers`);
    }

    saveCertificate(data: ISaveCertificateRequestData): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/card/certificate`, data);
    }

    getCertificateInformation(
        data: IGetCertificateInformationRequestData
    ): Observable<IGetCertificateInformationResponseData> {
        const params = new HttpParams({ fromObject: { ...data } });
        return this.http.get<any>(`${this.apiUrl}/card/information`, { params: params });
    }

    changePIN(data: IPINRequestData): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/card/pin/change`, data);
    }
}
