import { Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { DetailedProviderType, providerStatus } from '@app/features/providers/provider.dto';
import { ProviderService } from '@app/features/providers/provider.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-provider-details',
    templateUrl: './provider-details.component.html',
    styleUrls: ['./provider-details.component.scss'],
})
export class ProviderDetailsComponent implements OnInit, OnDestroy, OnChanges {
    constructor(private translateService: TranslocoService, private providerService: ProviderService) {}

    @Input() application: DetailedProviderType | null = null;
    @Input() isLoading = false;

    ngOnInit(): void {
        const langChanges$ = this.translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.providers.applications.txtTitle'),
                },
                {
                    label: this.translateService.translate('modules.providers.application-preview.txtTitle'),
                },
            ];
        });
        this.subscriptions.add(langChanges$);
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    ngOnChanges(changes: SimpleChanges): void {
        const application: DetailedProviderType | undefined = changes['application']?.currentValue;

        if (!application || !application.id) return;

        if (application.status === providerStatus.Values.ReturnedForCorrection) {
            this.historyQuery.execute(application.id);
        }
    }

    subscriptions = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    get administrator() {
        return this.application?.users.find(user => user.isAdministrator);
    }

    get isCreatedByAdmin() {
        if (!this.application) return false;
        const { createdByAdministratorId, createdByAdministratorName } = this.application;
        return createdByAdministratorId && createdByAdministratorName;
    }

    correctionComment = '';

    historyQuery = new RequestHandler({
        requestFunction: this.providerService.fetchProviderStatusHistory,
        onSuccess: data => {
            if (data[0].comment) {
                this.correctionComment = data[0].comment;
            }
        },
    });
}
