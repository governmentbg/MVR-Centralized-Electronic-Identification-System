import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IConfiguration, IConfigurationsPaginatedData, IGetConfigurationsParams } from '../interfaces/iconfigurations';

@Injectable({
    providedIn: 'root',
})
export class ConfigurationsClientService {
    private apiUrl = '/api/v1/smtpconfigurations';

    constructor(private http: HttpClient) {}

    getConfigurations(params: IGetConfigurationsParams): Observable<IConfigurationsPaginatedData> {
        const httpParams = new HttpParams().set('pageIndex', params.pageIndex).set('pageSize', params.pageSize);

        return this.http.get<IConfigurationsPaginatedData>(`${this.apiUrl}`, {
            params: httpParams,
        });
    }

    getConfigurationById(id: string): Observable<IConfiguration> {
        return this.http.get<IConfiguration>(`${this.apiUrl}/${id}`);
    }

    createConfiguration(configuration: IConfiguration): Observable<IConfiguration> {
        return this.http.post<IConfiguration>(`${this.apiUrl}`, configuration);
    }

    updateConfiguration(id: string, configuration: IConfiguration): Observable<IConfiguration> {
        return this.http.put<IConfiguration>(`${this.apiUrl}/${id}`, configuration);
    }

    deleteConfiguration(id: string): Observable<IConfiguration> {
        return this.http.delete<IConfiguration>(`${this.apiUrl}/${id}`);
    }
}
