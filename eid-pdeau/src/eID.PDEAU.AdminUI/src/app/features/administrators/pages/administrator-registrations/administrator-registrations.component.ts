import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import { Table } from 'primeng/table';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { eidManagerStatus } from '@app/features/administrators/enums/administrators.enum';

@Component({
    selector: 'app-administrator-registrations',
    templateUrl: './administrator-registrations.component.html',
    styleUrls: ['./administrator-registrations.component.scss'],
})
export class AdministratorRegistrationsComponent implements OnInit {
    constructor(private translateService: TranslocoService, private administratorService: AdministratorsService) {}

    @ViewChild('applicationsTable') table!: Table;

    applicationsQuery = new RequestHandler({
        requestFunction: this.administratorService.fetchAdministrators,
    });

    circumstanceChangesQuery = new RequestHandler({
        requestFunction: this.administratorService.fetchAdministratorCircumstanceChanges,
    });

    technicalChangesQuery = new RequestHandler({
        requestFunction: this.administratorService.fetchAdministratorCircumstanceChanges,
    });
    downloadFile = new RequestHandler({
        requestFunction: this.administratorService.downloadTechnicalCheckFile,
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

    loadCircumstanceChanges() {
        this.circumstanceChangesQuery.execute(eidManagerStatus.IN_REVIEW);
    }

    loadTechnicalChanges() {
        this.technicalChangesQuery.execute(eidManagerStatus.TECHNICAL_CHECK);
    }

    refreshTable(event: any) {
        this.currentTabIndex = event.index;
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
