import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

const baseApiUrl = '/raeicei/api/v1';
@Injectable({
    providedIn: 'root',
})
export class DocumentTypesService {
    private apiUrl = baseApiUrl;

    constructor(private http: HttpClient) {}

    fetchDocumentTypes = () => {
        return this.http.get(`${this.apiUrl}/document-type/list`).pipe(map((data: any) => data));
    };

    createDocumentType = (payload: any) => {
        return this.http.post(`${this.apiUrl}/document-type/create`, payload);
    };

    updateDocumentType = (payload: any) => {
        return this.http.put(`${this.apiUrl}/document-type/update`, payload);
    };

    deleteDocumentType = (id: string) => {
        return this.http.delete(`${this.apiUrl}/document-type/delete/${id}`);
    };
}
