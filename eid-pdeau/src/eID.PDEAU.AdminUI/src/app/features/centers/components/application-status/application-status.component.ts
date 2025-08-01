import { Component, Input } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ApplicationStatus } from '@app/features/centers/enums/centers.enum';

export const statuses: Record<
    ApplicationStatus,
    {
        icon: string;
        translationKey: string;
        class?: string;
    }
> = {
    [ApplicationStatus.DENIED]: {
        icon: '&#xe5c9;',
        translationKey: 'denied',
        class: 'text-danger',
    },
    [ApplicationStatus.ACCEPTED]: {
        icon: '&#xe876;',
        translationKey: 'approved',
        class: 'text-success',
    },
    [ApplicationStatus.IN_REVIEW]: {
        icon: '&#xe8b5;',
        translationKey: 'inReview',
        class: 'text-secondary',
    },
    [ApplicationStatus.ACTIVE]: {
        icon: '&#xe877;',
        translationKey: 'active',
        class: 'text-success',
    },
    [ApplicationStatus.RETURNED_FOR_CORRECTION]: {
        icon: '&#xe645;',
        translationKey: 'returnedForCorrection',
        class: 'text-warning',
    },
};

@Component({
    selector: 'app-application-status',
    templateUrl: './application-status.component.html',
    styleUrls: ['./application-status.component.scss'],
})
export class ApplicationStatusComponent {
    @Input() status?: ApplicationStatus | null = null;
    constructor(private translateService: TranslocoService) {}

    get currentStatus() {
        if (!this.status) return null;
        return statuses[this.status];
    }

    get label() {
        return this.translateService.translate(
            `modules.centers.applications.statuses.${this.currentStatus?.translationKey}`
        );
    }
}
