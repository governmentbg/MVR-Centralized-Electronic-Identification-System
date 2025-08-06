import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class ContentService {
    private apiUrl = '/mpozei/api/v1';

    constructor(private http: HttpClient) {}

    getHelpPagesList(payload: any): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/help-pages/find`, { params: payload });
    }

    getAllHelpPages(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/help-pages/find-all`);
    }

    getHelpPageById(id: string): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/help-pages/${id}`);
    }

    postContent(payload: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/help-pages`, { ...payload });
    }

    updateContent(payload: any): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/help-pages`, { ...payload });
    }

    deleteContent(id: string): Observable<any> {
        return this.http.delete<any>(`${this.apiUrl}/help-pages/${id}/delete`);
    }
}
