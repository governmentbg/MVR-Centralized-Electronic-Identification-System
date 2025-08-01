import { Component, Input } from '@angular/core';
import { ProviderStatusType } from '@app/features/providers/provider.dto';
import { TranslocoService } from '@ngneat/transloco';

export const statuses: Record<
    ProviderStatusType,
    {
        icon: string;
        translationKey: string;
        class?: string;
    }
> = {
    Denied: {
        icon: '&#xe5c9;',
        translationKey: 'denied',
        class: 'text-danger',
    },
    Active: {
        icon: '&#xe877;',
        translationKey: 'active',
        class: 'text-success',
    },
    InReview: {
        icon: '&#xe8b5;',
        translationKey: 'inReview',
        class: 'text-secondary',
    },
    ReturnedForCorrection: {
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
    @Input() status?: ProviderStatusType | null = null;
    constructor(private translateService: TranslocoService) {}

    get currentStatus() {
        if (!this.status) return null;
        return statuses[this.status];
    }

    get label() {
        return this.translateService.translate(
            `modules.providers.applications.statuses.${this.currentStatus?.translationKey}`
        );
    }
}
