import { Component, Input } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { providerServiceStatusEnum } from '@app/features/providers/services.dto';

export const statuses: Record<
    providerServiceStatusEnum,
    {
        icon: string;
        translationKey: string;
        class?: string;
    }
> = {
    Denied: {
        icon: '&#xe5c9;',
        translationKey: 'Denied',
        class: 'text-danger',
    },
    Approved: {
        icon: '&#xe877;',
        translationKey: 'Approved',
        class: 'text-success',
    },
    InReview: {
        icon: '&#xe8b5;',
        translationKey: 'InReview',
        class: 'text-secondary',
    },
    None: {
        icon: '',
        translationKey: 'None',
        class: '',
    },
};

@Component({
    selector: 'app-service-status',
    templateUrl: './service-status.component.html',
    styleUrls: ['./service-status.component.scss'],
})
export class ServiceStatusComponent {
    @Input() status?: providerServiceStatusEnum | null = null;
    constructor(private translateService: TranslocoService) {}

    get currentStatus() {
        if (!this.status) return null;
        return statuses[this.status];
    }

    get label() {
        return this.translateService.translate(`modules.services.enums.${this.currentStatus?.translationKey}`);
    }
}
