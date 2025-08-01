import { Injectable } from '@angular/core';
import { RaeiceiClientService } from './raeicei-client.service';
import { Observable } from 'rxjs';
import { IDevice, IDeviceWithTranslations } from '../interfaces/eid-management.interfaces';
import { TranslocoService } from '@ngneat/transloco';

@Injectable({
    providedIn: 'root',
})
export class EidDeviceService {
    constructor(private raeiceiClientService: RaeiceiClientService, private translateService: TranslocoService) {}

    private _devices: IDeviceWithTranslations[] = [];

    get devices() {
        return this._devices;
    }

    set devices(data: IDeviceWithTranslations[]) {
        this._devices = data;
    }

    getDeviceById(deviceId: string) {
        return this._devices.find(device => device.id === deviceId);
    }

    getDeviceTranslation(deviceId: string) {
        const foundDevice = this.getDeviceById(deviceId);
        if (foundDevice) {
            return (
                foundDevice.translations.find(
                    (translation: any) => translation.language === this.translateService.getActiveLang()
                )?.name || null
            );
        } else {
            return null;
        }
    }

    loadDevices(): Observable<IDevice[]> {
        return new Observable(subscriber => {
            this.raeiceiClientService.getAllDevices().subscribe({
                next: response => {
                    // Temporary solution until the format of the list of devices is changed in the BE
                    this.devices = response.map((device: IDevice) => {
                        return {
                            id: device.id,
                            type: device.type,
                            translations: [
                                {
                                    language: 'bg',
                                    name: device.name,
                                },
                                {
                                    language: 'en',
                                    name: device.description,
                                },
                            ],
                        };
                    });
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
