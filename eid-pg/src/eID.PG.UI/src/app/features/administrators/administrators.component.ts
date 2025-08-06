import { Component, OnDestroy, OnInit } from '@angular/core';
import { IEidAdministrator } from '../eid-management/interfaces/eid-management.interface';
import { RaeiceiClientService } from '../eid-management/services/raeicei-client.service';
import { Subscription } from 'rxjs';
import { IHidePreviewEventEmitter } from '../authorization-register/interfaces/authorization-register.interfaces';
import { ActivatedRoute } from '@angular/router';
import { Providers } from './enums/enums';

@Component({
    selector: 'app-administrators',
    templateUrl: './administrators.component.html',
    styleUrls: ['./administrators.component.scss'],
})
export class AdministratorsComponent implements OnInit, OnDestroy {
    administrators!: IEidAdministrator[];
    subscription!: Subscription;
    showPreview!: boolean;
    selectedAdministrator!: IEidAdministrator;
    paramType!: string;
    providerTypes = Providers;
    constructor(private raeiceiService: RaeiceiClientService, private route: ActivatedRoute) {}

    ngOnInit(): void {
        this.paramType = this.route.snapshot.url[0].path;
        if (this.paramType === Providers.ADMINISTRATORS) {
            this.subscription = this.raeiceiService
                .fetchAdministrators()
                .subscribe((administrators: IEidAdministrator[]) => {
                    this.administrators = administrators;
                });
        } else if (this.paramType === Providers.CENTERS) {
            this.subscription = this.raeiceiService.fetchEidCenters().subscribe((centers: IEidAdministrator[]) => {
                this.administrators = centers;
            });
        }
    }

    showDetails(administrator: IEidAdministrator) {
        this.selectedAdministrator = administrator;
        this.showPreview = true;
    }

    onHideDetails(eventData: IHidePreviewEventEmitter) {
        this.showPreview = eventData.showPreview;
    }

    ngOnDestroy(): void {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }
}
