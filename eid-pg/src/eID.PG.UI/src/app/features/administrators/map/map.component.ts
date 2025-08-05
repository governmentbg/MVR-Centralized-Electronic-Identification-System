import { Component, Input, OnInit, OnDestroy, EventEmitter } from '@angular/core';
import * as L from 'leaflet';
import { Subscription } from 'rxjs';
import { ICoordinatesEventEmitter } from '../interfaces/interfaces';
import { IEidAdministratorOffice } from '../../eid-management/interfaces/eid-management.interface';
import { Actions } from '../enums/enums';
@Component({
    selector: 'app-map',
    templateUrl: './map.component.html',
    styleUrls: ['./map.component.scss'],
})
export class MapComponent implements OnInit, OnDestroy {
    @Input() coordinates!: EventEmitter<ICoordinatesEventEmitter>;
    @Input() offices!: IEidAdministratorOffice[];
    private map!: L.Map;
    private centroid: L.LatLngExpression = [42.7339, 25.4858];
    private coordinatesSubscription!: Subscription;

    ngOnInit(): void {
        this.initMap();
        this.coordinatesSubscription = this.coordinates.subscribe(coords => {
            if (this.map) {
                this.map.setView([coords.latitude, coords.longitude], 12);
                if (coords.action === Actions.FOCUS_OFFICE) {
                    const marker = L.marker([coords.office.latitude, coords.office.longitude], {
                        icon: L.icon({
                            iconUrl: '../../../../assets/images/marker-icon.png',
                            shadowUrl: '../../../../assets/images/marker-shadow.png',
                            iconSize: [25, 41],
                            iconAnchor: [12, 41],
                            shadowSize: [41, 41],
                            shadowAnchor: [13, 41],
                        }),
                    });
                    marker
                        .addTo(this.map)
                        .bindPopup(
                            `<div><div class='d-flex flex-row flex-nowrap align-items-center justify-content-center'><i class="material-icons-outlined">&#xe892;</i><p>${
                                coords.office.name
                            }</p></div><div class='d-flex flex-row flex-nowrap align-items-center justify-content-center'><i class="material-icons-outlined">&#xe8b5;</i><p>${coords.office.workingHours
                                .map(hours => `<p>${hours}</p>`)
                                .join('')}</p></div></div>`,
                            { className: 'custom-popup' }
                        )
                        .openPopup();
                }
                if (coords.action === Actions.FOCUS_OUT) {
                    this.map.setView(this.centroid, 5);
                }
            }
        });
    }

    private initMap(): void {
        this.map = L.map('map', {
            center: this.centroid,
            zoom: 5,
        });

        const tiles = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 18,
            minZoom: 7,
        }).addTo(this.map);

        if (this.offices && this.offices.length) {
            this.offices.forEach((office: IEidAdministratorOffice) => {
                const marker = L.marker([office.latitude, office.longitude] as L.LatLngExpression, {
                    icon: L.icon({
                        iconUrl: '../../../../assets/images/marker-icon.png',
                        shadowUrl: '../../../../assets/images/marker-shadow.png',
                        iconSize: [25, 41],
                        iconAnchor: [12, 41],
                        shadowSize: [41, 41],
                        shadowAnchor: [13, 41],
                    }),
                });
                marker
                    .addTo(this.map)
                    .bindPopup(
                        `<div><div class='d-flex flex-row flex-nowrap align-items-center justify-content-center'><i class="material-icons-outlined">&#xe892;</i><p>${
                            office.name
                        }</p></div><div class='d-flex flex-row flex-nowrap align-items-center justify-content-center'><i class="material-icons-outlined">&#xe8b5;</i><p>${office.workingHours
                            .map(hours => `<p>${hours}</p>`)
                            .join('')}</p></div></div>`,
                        { className: 'custom-popup' }
                    );
            });
        }
    }

    ngOnDestroy(): void {
        this.coordinatesSubscription.unsubscribe();
    }
}
