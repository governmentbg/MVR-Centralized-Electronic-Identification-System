import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { IDataset, IDatasetRequestPayload, IDatasets } from '../interfaces/IDatasets';

@Injectable({
    providedIn: 'root',
})
export class DatasetsService {
    private apiUrl = 'api/v1/datasets';

    constructor(private http: HttpClient) {}

    getAllDatasets(): Observable<IDatasets> {
        return this.http.get<any>(`${this.apiUrl}`);
    }

    createDataset(dataset: IDatasetRequestPayload): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}`, dataset);
    }

    deleteDataset(datasetId: string): Observable<any> {
        return this.http.delete<any>(`${this.apiUrl}/${datasetId}`);
    }

    editDataset(dataset: IDataset): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/${dataset.id}`, dataset);
    }

    uploadDataset(datasetId: string): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/${datasetId}/upload`, { id: datasetId });
    }
}
