import { Component, EventEmitter, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { first, Subscription } from 'rxjs';
import {
    IEidAdministrator,
    IEidAdministratorOffice,
} from 'src/app/features/eid-management/interfaces/eid-management.interface';
import { RaeiceiClientService } from 'src/app/features/eid-management/services/raeicei-client.service';
import { IBreadCrumbItems } from 'src/app/shared/interfaces/IBreadCrumbItems';
import { ToastService } from 'src/app/shared/services/toast.service';
import { cities } from '../../cities-data/cities';
import { Actions, Providers } from '../../enums/enums';
import { ICity, ICoordinatesEventEmitter } from '../../interfaces/interfaces';
import { ActivatedRoute, Router } from '@angular/router';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';

@Component({
    selector: 'app-provider-data',
    templateUrl: './provider-data.component.html',
    styleUrls: ['./provider-data.component.scss'],
})
export class ProviderDataComponent implements OnInit, OnDestroy, OnChanges {
    breadcrumbItems: IBreadCrumbItems[] = [];
    subscriptions: Subscription[] = [];
    providerFrontOfficesList!: IEidAdministratorOffice[];
    cities!: ICity[];
    chosenCity!: string;
    officesList!: IEidAdministratorOffice[];
    language!: string;
    providerTypes = Providers;
    providerid!: string;
    type!: string;
    provider!: IEidAdministrator;
    coordinates: EventEmitter<ICoordinatesEventEmitter> = new EventEmitter();

    constructor(
        public translateService: TranslocoService,
        private toastService: ToastService,
        private raeiceiService: RaeiceiClientService,
        private route: ActivatedRoute,
        private router: Router
    ) {
        const languageChangeSubscription = translateService.langChanges$.subscribe(language => {
            this.providerid = this.route.snapshot.paramMap.get('id') || '';
            this.type = this.route.snapshot.url[0].path;
            this.language = language;
            this.cities = cities;
        });
        this.subscriptions.push(languageChangeSubscription);
    }

    ngOnInit(): void {
        if (this.providerid && this.type === Providers.ADMINISTRATORS) {
            this.getAdministrator();
        } else {
            this.getCenter();
        }
    }

    getAdministrator() {
        const administrator = this.raeiceiService
            .getAdministratorById(this.providerid)
            .pipe(first())
            .subscribe({
                next: (provider: IEidAdministrator) => {
                    this.provider = provider;
                    this.updateBreadcrumbItems();
                    this.raeiceiService
                        .fetchOffices(this.providerid)
                        .subscribe((offices: IEidAdministratorOffice[]) => {
                            this.providerFrontOfficesList = offices;
                            this.officesList = offices;
                        });
                },
                error: err => {
                    let showDefaultError = true;
                    err.errors?.forEach((el: string) => {
                        if (errorDataStorage.has(el)) {
                            this.toastService.showErrorToast(
                                this.translateService.translate('global.txtErrorTitle'),
                                this.translateService.translate('errors.' + formatError(el))
                            );
                            showDefaultError = false;
                        }
                    });
                    if (showDefaultError) {
                        this.toastService.showErrorToast(
                            this.translateService.translate('global.txtErrorTitle'),
                            this.translateService.translate('global.txtUnexpectedError')
                        );
                    }
                },
            });
        this.subscriptions.push(administrator);
    }

    getCenter() {
        const center = this.raeiceiService
            .getEidCenterById(this.providerid)
            .pipe(first())
            .subscribe({
                next: (provider: IEidAdministrator) => {
                    this.provider = provider;
                    this.updateBreadcrumbItems();
                    this.raeiceiService
                        .fetchOffices(this.providerid)
                        .subscribe((offices: IEidAdministratorOffice[]) => {
                            this.providerFrontOfficesList = offices;
                            this.officesList = offices;
                        });
                },
                error: err => {
                    let showDefaultError = true;
                    err.errors?.forEach((el: string) => {
                        if (errorDataStorage.has(el)) {
                            this.toastService.showErrorToast(
                                this.translateService.translate('global.txtErrorTitle'),
                                this.translateService.translate('errors.' + formatError(el))
                            );
                            showDefaultError = false;
                        }
                    });
                    if (showDefaultError) {
                        this.toastService.showErrorToast(
                            this.translateService.translate('global.txtErrorTitle'),
                            this.translateService.translate('global.txtUnexpectedError')
                        );
                    }
                },
            });
        this.subscriptions.push(center);
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['administrator']) {
            this.updateBreadcrumbItems();
        }
    }

    ngOnDestroy() {
        this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    getLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(this.showPosition.bind(this));
        } else {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('errors.txtUnsupportedBrowserFeature')
            );
        }
    }

    showPosition(position: GeolocationPosition) {
        const userLatitude = position.coords.latitude;
        const userLongitude = position.coords.longitude;
        if (!this.providerFrontOfficesList.length) {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.administrators.providersPages.txtNoRegisteredOffices')
            );
            return;
        }
        const nearestOffice = this.findNearestOffice(userLatitude, userLongitude);

        this.coordinates.emit({
            office: nearestOffice,
            latitude: nearestOffice.latitude,
            longitude: nearestOffice.longitude,
            action: Actions.FOCUS_OFFICE,
        });
    }

    focusCity(event: any) {
        if (event.city === 'Всички') {
            this.providerFrontOfficesList = this.officesList;
            this.coordinates.emit({
                office: event,
                latitude: event.lat,
                longitude: event.lng,
                action: Actions.FOCUS_OUT,
            });
            return;
        }
        this.providerFrontOfficesList = this.officesList.filter(el => el.location === event.cityEN);
        this.coordinates.emit({ office: event, latitude: event.lat, longitude: event.lng, action: Actions.FOCUS_CITY });
    }

    showOfficeOnMap(office: IEidAdministratorOffice, longitude: number, latitude: number) {
        this.coordinates.emit({
            office: office,
            latitude: latitude,
            longitude: longitude,
            action: Actions.FOCUS_OFFICE,
        });
    }

    findNearestOffice(userLat: number, userLon: number): IEidAdministratorOffice {
        let nearestOffice = this.providerFrontOfficesList[0];
        let minDistance = this.haversineDistance(userLat, userLon, nearestOffice.latitude, nearestOffice.longitude);

        for (const office of this.providerFrontOfficesList) {
            const distance = this.haversineDistance(userLat, userLon, office.latitude, office.longitude);
            if (distance < minDistance) {
                nearestOffice = office;
                minDistance = distance;
            }
        }
        return nearestOffice;
    }

    haversineDistance(lat1: number, lon1: number, lat2: number, lon2: number): number {
        const toRad = (x: number) => (x * Math.PI) / 180;
        const R = 6371;
        const dLat = toRad(lat2 - lat1);
        const dLon = toRad(lon2 - lon1);
        const a =
            Math.sin(dLat / 2) * Math.sin(dLat / 2) +
            Math.cos(toRad(lat1)) * Math.cos(toRad(lat2)) * Math.sin(dLon / 2) * Math.sin(dLon / 2);
        const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        return R * c;
    }

    updateBreadcrumbItems() {
        const isMobile = this.router.url.includes('/mobile');

        if (this.provider && this.provider.name) {
            const basePath = isMobile ? '/mobile/home' : '/home';
            const adminPath = `${basePath}/administrators`;
            const centerPath = `${basePath}/centers`;

            this.breadcrumbItems = [
                {
                    label:
                        this.type === Providers.ADMINISTRATORS
                            ? this.translateService.translate('modules.administrators.mainPage.txtTitleAdministrator')
                            : this.translateService.translate('modules.administrators.mainPage.txtTitleCenter'),
                    routerLink: this.type === Providers.ADMINISTRATORS ? adminPath : centerPath,
                },
                { label: this.provider.name },
            ];
        }
    }

    isLangBG() {
        return this.language === 'bg';
    }
}
