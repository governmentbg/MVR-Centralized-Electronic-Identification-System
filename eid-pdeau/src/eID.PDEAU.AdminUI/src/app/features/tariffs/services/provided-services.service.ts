import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TariffsService } from '@app/features/tariffs/tariffs.service';
import { ProvidedServiceType } from '@app/features/tariffs/tariffs.dto';
import { TranslocoService } from '@ngneat/transloco';

@Injectable({
    providedIn: 'root',
})
export class ProvidedServicesService {
    constructor(private tariffsService: TariffsService, private translateService: TranslocoService) {}

    private _providedServices: ProvidedServiceType[] = [];

    get providedServices() {
        return this._providedServices;
    }

    set providedServices(data: ProvidedServiceType[]) {
        this._providedServices = data;
    }

    getProvidedServiceById(id: string) {
        return this._providedServices.find(service => service.id === id);
    }

    getProvidedServiceTranslation(id: string) {
        const foundService = this.getProvidedServiceById(id);
        if (foundService) {
            return this.translateService.getActiveLang() === 'bg' ? foundService.name : foundService.nameLatin;
        } else {
            return null;
        }
    }

    loadProvidedServices(): Observable<ProvidedServiceType[]> {
        return new Observable(subscriber => {
            this.tariffsService.fetchProvidedServices().subscribe({
                next: response => {
                    this.providedServices = response;
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
