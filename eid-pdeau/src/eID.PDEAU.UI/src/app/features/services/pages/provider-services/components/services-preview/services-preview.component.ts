import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { providerServiceStatusEnum, ProviderServiceType } from '@app/features/services/services.dto';
import { ProviderServicesService } from '@app/features/services/provider-services.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { ToastService } from '@app/shared/services/toast.service';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { ToggleAction } from '@app/features/services/services.dto';

@Component({
    selector: 'app-services-preview',
    templateUrl: './services-preview.component.html',
    styleUrls: ['./services-preview.component.scss'],
})
export class ServicePreviewComponent implements OnInit {
    constructor(
        private translateService: TranslocoService,
        private providerServicesService: ProviderServicesService,
        private toastService: ToastService,
        private router: Router,
    ) {}

    @Input() service!: ProviderServiceType;
    @Output() closePreview = new EventEmitter();
    ToggleAction = ToggleAction;

    ngOnInit(): void {
        this.translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.services.services-table.title'),
                    onClick: () => this.onClosePreview(),
                },
                {
                    label: this.translateService.translate('modules.services.service-preview.title'),
                },
            ];

            if (!this.service.requiredPersonalInformation) {
                this.personalInformation = this.translateService.translate('modules.services.enums.None');
            } else {
                this.personalInformation = this.service.requiredPersonalInformation?.reduce((acc, data, index, arr) => {
                    acc += this.translateService.translate(`modules.services.enums.${data}`);
                    if (index < arr.length - 1) {
                        acc += ', ';
                    }
                    return acc;
                }, '');
            }
        });

        if (!this.service?.id) return;

        this.serviceScopeQuery.execute(this.service?.id);
    }

    breadcrumbItems: IBreadCrumbItems[] = [];
    personalInformation = '';
    ServiceStatus = providerServiceStatusEnum;

    serviceScopeQuery = new RequestHandler({
        requestFunction: this.providerServicesService.fetchServicesScope,

        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.services.service-preview.toasts.serviceScopeFetchFail')
            );
            this.router.navigate(['/providers/services']);
        },
    });

    onClosePreview() {
        this.closePreview.emit();
    }
}
