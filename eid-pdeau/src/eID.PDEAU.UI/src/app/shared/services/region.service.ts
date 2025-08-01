import { Injectable } from '@angular/core';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import { Observable } from 'rxjs';
import { Region } from '@app/features/centers/centers.dto';

@Injectable({
    providedIn: 'root',
})
export class RegionService {
    constructor(private administratorsService: AdministratorsService) {}

    private _regions: Region[] = [];

    get regions() {
        return this._regions;
    }

    set regions(data: Region[]) {
        this._regions = data;
    }

    loadRegions(): Observable<Region[]> {
        return new Observable(subscriber => {
            this.administratorsService.fetchRegions().subscribe({
                next: response => {
                    this.regions = response;
                    subscriber.next(response);
                    subscriber.complete();
                },
                error: error => {
                    subscriber.error(error);
                },
            });
        });
    }
}
