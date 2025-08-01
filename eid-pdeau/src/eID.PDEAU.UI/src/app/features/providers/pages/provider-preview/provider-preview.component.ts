import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProviderService } from '@app/features/providers/provider.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { ConfirmationService } from 'primeng/api';
import { ToastService } from '@app/shared/services/toast.service';
import { providerStatus } from '../../provider.dto';
import { UserService } from '@app/core/services/user.service';
import { RoleType } from '@app/core/enums/auth.enum';

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
        private confirmationService: ConfirmationService,
        private toastService: ToastService,
        public userService: UserService
    ) {}
    providerId = this.route.snapshot.paramMap.get('providerId');
    mode: 'view' | 'edit' = 'view';
    providerStatus = providerStatus;
    RoleType = RoleType;
    @Output() closePreview = new EventEmitter();

    ngOnInit(): void {
        // const providerId = this.route.snapshot.paramMap.get('providerId');
        if (!this.providerId) {
            this.router.navigate(['/providers']);
            return;
        }
        this.providerQuery.execute(this.providerId);

        const langChanges$ = this.translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.providers.applications.title'),
                    onClick: () => this.router.navigate(['/providers']),
                },
                {
                    label: this.translateService.translate('modules.providers.provider-preview.txtTitle'),
                },
            ];
        });
        this.subscriptions.add(langChanges$);
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    changeMode(mode: 'view' | 'edit') {
        this.mode = mode;
        window.scrollTo({
            top: 0,
            behavior: 'smooth',
        });
    }
    closeEdit(refetch: boolean) {
        this.changeMode('view');
        if (refetch) {
            this.providerQuery.execute(this.providerId!);
        }
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

    subscriptions = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
}
