import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import {
    IEmpowermentApproveRequestData,
    IEmpowermentCheckRequestData,
    IEmpowermentDenyRequestData,
    ILegalEntityInfoOriginalResponse,
    ILegalEntityInfoResponse,
} from '@app/features/empowerment-check/interfaces/empowerment-check.interfaces';
import { IdentifierType } from '@app/shared/enums';

@Injectable({
    providedIn: 'root',
})
export class EmpowermentCheckClientService {
    private apiUrl = '/admin/api/v1';
    private ROapiUrl = 'ro/api/v1';

    constructor(private http: HttpClient) {}

    empowermentCheck(data: IEmpowermentCheckRequestData): Observable<any> {
        return this.http.post<any>(`${this.ROapiUrl}/deau/empowerments`, data, { observe: 'response' });
    }

    denyEmpowerment(data: IEmpowermentDenyRequestData): Observable<any> {
        return this.http.post<any>(`${this.ROapiUrl}/deau/deny-empowerment`, data);
    }

    approveEmpowerment(data: IEmpowermentApproveRequestData): Observable<any> {
        return this.http.post<any>(`${this.ROapiUrl}/deau/approve-empowerment`, data);
    }

    getLegalEntityInfo(data: { uic: string }): Observable<ILegalEntityInfoResponse> {
        const params = new HttpParams({ fromObject: data });
        return this.http
            .get<ILegalEntityInfoOriginalResponse>(`${this.apiUrl}/legalEntity/info`, { params: params })
            .pipe(
                map((response: ILegalEntityInfoOriginalResponse) => {
                    // Information about the legal entity retrieved from TR (isFoundInRegix = true => ActualStateResponseV3)
                    // or Bulstat (isFoundInRegix = false => StateOfPlayResponseType)
                    if ('actualStateResponseV3' in response.originalResponse.response) {
                        const regixResponse = response.originalResponse.response.actualStateResponseV3;
                        const responseObj: ILegalEntityInfoResponse = {
                            legalEntityName: regixResponse.deed.companyName,
                            uic: regixResponse.deed.uic,
                            legalRepresentatives: regixResponse.deed.subdeeds.subdeed[0].records.record.reduce(
                                (filtered: any[], record: any) => {
                                    if (record.recordData.representative) {
                                        const someNewValue = {
                                            name: record.recordData.representative.subject.name,
                                            uid: record.recordData.representative.subject.indent,
                                            uidType: record.recordData.representative.subject.indentType,
                                        };
                                        filtered.push(someNewValue);
                                    }
                                    return filtered;
                                },
                                []
                            ),
                            wayOfRepresentation:
                                regixResponse.deed.subdeeds.subdeed[0].records.record.find((record: any) => {
                                    return !!record.recordData.wayOfRepresentation;
                                })?.recordData.wayOfRepresentation?.value || '',
                            state: '',
                        };
                        return responseObj;
                    } else {
                        const bulstatResponse = response.originalResponse.response.StateOfPlayResponseType;
                        const responseObj: ILegalEntityInfoResponse = {
                            legalEntityName: bulstatResponse.subject?.legalEntitySubject.cyrillicFullName || '',
                            uic: bulstatResponse.subject?.uic.uiC1 || '',
                            legalRepresentatives:
                                bulstatResponse.managers?.reduce((filtered: any[], record: any) => {
                                    if (record.relatedSubject.naturalPersonSubject) {
                                        const someNewValue = {
                                            name: record.relatedSubject.naturalPersonSubject.cyrillicName,
                                            uid:
                                                record.relatedSubject.naturalPersonSubject.egn ||
                                                record.relatedSubject.naturalPersonSubject.lnc,
                                            uidType: record.relatedSubject.naturalPersonSubject.egn
                                                ? IdentifierType.EGN
                                                : IdentifierType.LNCh,
                                        };
                                        filtered.push(someNewValue);
                                    }
                                    return filtered;
                                }, []) || [],
                            wayOfRepresentation: '',
                            state: bulstatResponse.state?.state.code || '',
                        };
                        return responseObj;
                    }
                })
            );
    }
}
