import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
    BoricaSignRequestData,
    DocumentSignResponseData,
    BoricaStatusRequestData,
    EvrotrustSignRequestData,
    EvrotrustStatusRequestData,
    SignatureStatusResponseData,
    SignedDocumentDownloadResponseData,
    EvrotrustUserCheckResponseData,
    KEPSignRequestData,
    KEPSignResponseData,
    KEPDigestDataResponseData,
    KEPDigestRequestData,
    BoricaUserCheckResponseData,
    BoricaStatusResponseData,
    BoricaDocumentSignResponseData,
} from '../interfaces/document-sign.interfaces';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class SignClientService {
    private apiUrlBorica = 'signing/api/v1/Borica';
    private apiUrlEvrotrust = 'signing/api/v1/Evrotrust';
    private apiUrlLocalSignature = 'signing/api/v1/KEP';

    constructor(private http: HttpClient) {}

    public getNexuJSVersion() {
        return fetch(`http://localhost:9795/nexu-info`);
    }

    public digestData(data: KEPDigestRequestData): Observable<KEPDigestDataResponseData> {
        const method = `${this.apiUrlLocalSignature}/digest/data`;

        return this.http.post<any>(method, data);
    }

    public signData(data: KEPSignRequestData): Observable<KEPSignResponseData> {
        const method = `${this.apiUrlLocalSignature}/digest/sign`;

        return this.http.post<any>(method, data);
    }

    public checkUserBorica(uid: string): Observable<BoricaUserCheckResponseData> {
        return this.http.post<any>(`${this.apiUrlBorica}/user/check`, { uid });
    }

    public checkUserEvrotrust(uid: string): Observable<EvrotrustUserCheckResponseData> {
        return this.http.post<any>(`${this.apiUrlEvrotrust}/user/check`, { uid });
    }

    public signDataBorica(data: BoricaSignRequestData): Observable<BoricaDocumentSignResponseData> {
        return this.http.post<any>(`${this.apiUrlBorica}/sign`, data);
    }

    public signDataEvrotrust(data: EvrotrustSignRequestData): Observable<DocumentSignResponseData> {
        return this.http.post<any>(`${this.apiUrlEvrotrust}/sign`, data);
    }

    public checkSignedDataBorica(data: BoricaStatusRequestData): Observable<BoricaStatusResponseData> {
        return this.http.get<any>(`${this.apiUrlBorica}/status`, {
            params: {
                transactionId: data.transactionId,
            },
        });
    }

    public checkSignedDataEvrotrust(data: EvrotrustStatusRequestData): Observable<SignatureStatusResponseData> {
        return this.http.get<any>(`${this.apiUrlEvrotrust}/status`, {
            params: {
                transactionId: data.transactionId,
                groupSigning: data.groupSigning,
            },
        });
    }

    public getSignedDataBorica(data: BoricaStatusRequestData): Observable<SignedDocumentDownloadResponseData> {
        return this.http.get<any>(`${this.apiUrlBorica}/download`, {
            params: {
                transactionId: data.transactionId,
            },
        });
    }

    public getSignedDataEvrotrust(data: EvrotrustStatusRequestData): Observable<SignedDocumentDownloadResponseData[]> {
        return this.http.get<any>(`${this.apiUrlEvrotrust}/download`, {
            params: {
                transactionId: data.transactionId,
                groupSigning: data.groupSigning,
            },
        });
    }
}
