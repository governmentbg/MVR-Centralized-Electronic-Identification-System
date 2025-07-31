import { Injectable } from '@angular/core';
import { SystemType } from '@app/core/enums/auth.enum';
import { CentersService } from '@app/features/centers/centers.service';
import { UserService } from '@app/core/services/user.service';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import { ManagerStatus } from '@app/features/administrators/enums/administrators.enum';
import { AlertService } from '@app/shared/services/alert.service';
import { TranslocoService } from '@ngneat/transloco';

@Injectable({
    providedIn: 'root',
})
export class ManagerStatusAlertService {
    constructor(
        private administratorService: AdministratorsService,
        private centersService: CentersService,
        private userService: UserService,
        private translocoService: TranslocoService,
        private alertService: AlertService
    ) {}

    checkForPendingAttachments() {
        if (this.userService.getSystemType() === SystemType.CEI) {
            this.centersService.fetchCenter().subscribe({
                next: response => {
                    this.handleStatus(response.managerStatus);
                },
            });
        }
        if (this.userService.getSystemType() === SystemType.AEI) {
            this.administratorService.fetchAdministrator().subscribe({
                next: response => {
                    this.handleStatus(response.managerStatus);
                },
            });
        }
    }

    handleStatus(managerStatus: ManagerStatus) {
        if (managerStatus === ManagerStatus.PENDING_ATTACHMENTS) {
            this.alertService.addAlert(
                this.translocoService.translate('modules.administrators.documents.txtPendingAttachmentsTitle'),
                this.translocoService.translate('modules.administrators.documents.txtPendingAttachmentsMessage'),
                'warn',
                ManagerStatus.PENDING_ATTACHMENTS
            );
        } else {
            this.alertService.removeAlert(ManagerStatus.PENDING_ATTACHMENTS);
        }
    }
}
