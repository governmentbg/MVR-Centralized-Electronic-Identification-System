import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProviderService } from '@app/features/providers/provider.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { ToastService } from '@app/shared/services/toast.service';
import { providerStatus } from '@app/features/providers/provider.dto';

@Component({
    selector: 'app-provider-preview',
    templateUrl: './provider-preview.component.html',
    styleUrls: ['./provider-preview.component.scss'],
})
export class ProviderPreviewComponent implements OnInit, OnDestroy {
    constructor(
        private translateService: TranslocoService,
        private router: Router,
        private route: ActivatedRoute,
        private providerService: ProviderService,
        private toastService: ToastService
    ) {}
    providerIdInRoute = this.route.snapshot.paramMap.get('providerId');
    @Output() closePreview = new EventEmitter();
    @Input() providerId: string | null = null;
    @Input() standalone = true;

    ngOnInit(): void {
        const providerId = this.providerId || this.providerIdInRoute;
        if (!providerId) {
            this.router.navigate(['/providers']);
            return;
        } else {
            this.providerQuery.execute(providerId);
        }

        const langChanges$ = this.translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.providers.applications.txtTitle'),
                    onClick: () => this.onClosePreview(),
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

    providerQuery = new RequestHandler({
        requestFunction: this.providerService.fetchProviderById,
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('global.txtSomethingWentWrong')
            );
            this.router.navigate(['/providers']);
        },
    });

    refreshQuery = () => {
        if (!this.providerId) return;
        this.providerQuery.execute(this.providerId);
    };

    subscriptions = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    ProviderStatus = providerStatus;

    onClosePreview() {
        if (this.standalone) {
            this.router.navigate(['/providers']);
        } else {
            this.closePreview.emit();
        }
    }

    get allowUsersModification() {
        return this.providerQuery.data?.status === providerStatus.enum.Active;
    }
}
