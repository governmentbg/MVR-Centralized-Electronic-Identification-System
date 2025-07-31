import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { CentersService } from '@app/features/centers/centers.service';
import { Table } from 'primeng/table';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ApplicationType } from '@app/features/centers/enums/centers.enum';
import { eidManagerStatus } from '@app/features/administrators/enums/administrators.enum';

@Component({
    selector: 'app-center-registrations',
    templateUrl: './center-registrations.component.html',
    styleUrls: ['./center-registrations.component.scss'],
})
export class CenterRegistrationsComponent implements OnInit {
    constructor(private translateService: TranslocoService, private centersService: CentersService) {}

    @ViewChild('applicationsTable') table!: Table;

    applicationsQuery = new RequestHandler({
        requestFunction: this.centersService.fetchCenters,
    });

    circumstanceChangesQuery = new RequestHandler({
        requestFunction: this.centersService.fetchCenterCircumstanceChanges,
    });

    technicalChangesQuery = new RequestHandler({
        requestFunction: this.centersService.fetchCenterCircumstanceChanges,
    });
    downloadFile = new RequestHandler({
        requestFunction: this.centersService.downloadTechnicalCheckFile,
        onSuccess: attachment => {
            const fileName = 'Technical connectivity protocol';
            const url = `data:application/msword;base64,${attachment.content}`;
            const a = document.createElement('a');
            a.href = url;
            a.download = fileName;
            a.click();
            window.URL.revokeObjectURL(url);
            a.remove();
        },
    });
    filters: any = {};
    currentTabIndex = 0;

    ngOnInit(): void {
        this.loadApplications();
    }

    loadApplications() {
        this.applicationsQuery.execute();
    }

    getApplicationTypeText(type: ApplicationType) {
        return this.translateService.translate(`modules.centers.applications.types.${type}`);
    }

    loadCircumstanceChanges() {
        this.circumstanceChangesQuery.execute(eidManagerStatus.IN_REVIEW);
    }

    loadTechnicalChanges() {
        this.technicalChangesQuery.execute(eidManagerStatus.TECHNICAL_CHECK);
    }

    refreshTable(event: any) {
        switch (event.index) {
            case 0:
                this.loadApplications();
                break;
            case 1:
                this.loadTechnicalChanges();
                break;
            case 2:
                this.loadCircumstanceChanges();
                break;
        }
    }

    download() {
        this.downloadFile.execute();
    }
}
