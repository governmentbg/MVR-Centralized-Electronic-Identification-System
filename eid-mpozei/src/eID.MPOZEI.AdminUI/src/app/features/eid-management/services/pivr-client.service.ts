import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import {
    ICheckUidRestrictionsResponseData,
    IGetForeignIdentityV2ResponseData,
    IGetPersonalIdentityV2ResponseData,
    IPersonalIdentity,
    IRelationsSearchResponseData,
} from '../interfaces/eid-management.interfaces';
import { PersonalIdTypes } from '../enums/eid-management.enum';
import { DateParserService } from '../../../shared/services/date-parser.service';

@Injectable({
    providedIn: 'root',
})
export class PivrClientService {
    private apiUrl = '/pivr/api/v1';

    constructor(private http: HttpClient, private dateParserService: DateParserService) {}

    getForeignIdentityV2(data: {
        Identifier?: string;
        IdentifierType?: string;
    }): Observable<IGetForeignIdentityV2ResponseData> {
        const params = new HttpParams({ fromObject: data });
        return this.http.get<any>(`${this.apiUrl}/Registries/mvr/getforeignidentityv2`, { params: params }).pipe(
            map((response: any) => {
                const identityDocumentObject = response.response.ForeignIdentityInfoResponse.identityDocument;
                for (const key in identityDocumentObject) {
                    response.response.ForeignIdentityInfoResponse[key] = identityDocumentObject[key];
                }
                // The birthdate of foreign citizens has multiple formats that we need to parse
                response.response.ForeignIdentityInfoResponse.birthDate = this.dateParserService.parseDateString(
                    response.response.ForeignIdentityInfoResponse.birthDate
                );
                return response;
            })
        );
    }

    getPersonalIdentityV2(data: {
        EGN: string;
        IdentityDocumentNumber: string;
    }): Observable<IGetPersonalIdentityV2ResponseData> {
        const params = new HttpParams({ fromObject: data });
        return this.http.get<any>(`${this.apiUrl}/Registries/mvr/getpersonalidentityv2`, { params: params }).pipe(
            map((response: any) => {
                // Certain foreign citizens can have EGN and therefore their personal information is located in another object
                if (response.response.PersonalIdentityInfoResponse.dataForeignCitizen) {
                    const {
                        names = {},
                        birthDate,
                        birthDateSpecified,
                        nationalityList = [],
                    } = response.response.PersonalIdentityInfoResponse.dataForeignCitizen;
                    const personNames: IPersonalIdentity['personNames'] =
                        response.response.PersonalIdentityInfoResponse.personNames;

                    if (Object.keys(names).length > 0) {
                        if (names.firstName) personNames.firstName = names.firstName;
                        if (names.surname) personNames.surname = names.surname;
                        if (names.familyName) personNames.familyName = names.familyName;
                        if (names.firstNameLatin) personNames.firstNameLatin = names.firstNameLatin;
                        if (names.surnameLatin) personNames.surnameLatin = names.surnameLatin;
                        if (names.familyNameLatin) personNames.lastNameLatin = names.familyNameLatin;
                        if (names.lastNameLatin) personNames.lastNameLatin = names.lastNameLatin;
                    }

                    if (birthDate) {
                        response.response.PersonalIdentityInfoResponse.birthDate = birthDate;
                    }

                    if (birthDateSpecified) {
                        response.response.PersonalIdentityInfoResponse.birthDateSpecified = birthDateSpecified;
                    }

                    if (nationalityList.length > 0) {
                        response.response.PersonalIdentityInfoResponse.nationalityList = nationalityList;
                    }
                }
                return response;
            })
        );
    }

    getPersonRelatives(identifier: string): Observable<IRelationsSearchResponseData> {
        const params = new HttpParams().set('identifier', identifier);
        return this.http.get<any>(`${this.apiUrl}/Registries/grao/relationssearch`, { params: params });
    }

    checkUidRestrictions(data: {
        uid: string;
        uidType: PersonalIdTypes;
    }): Observable<ICheckUidRestrictionsResponseData> {
        return this.http.get<any>(`${this.apiUrl}/Registries/otherservices/checkuidrestrictions`, { params: data });
    }
}
