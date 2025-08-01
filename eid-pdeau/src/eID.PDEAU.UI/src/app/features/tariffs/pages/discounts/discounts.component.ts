import { Component, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService } from 'primeng/api';
import { ToastService } from '@app/shared/services/toast.service';
import { Router } from '@angular/router';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { TariffsService } from '@app/features/tariffs/tariffs.service';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { DiscountType } from '@app/features/tariffs/tariffs.dto';
import { ProvidedServicesService } from '@app/features/tariffs/services/provided-services.service';

@Component({
    selector: 'app-discounts',
    templateUrl: './discounts.component.html',
    styleUrls: ['./discounts.component.scss'],
    providers: [ConfirmationService],
})
export class DiscountsComponent implements OnInit {
    constructor(
        public translateService: TranslocoService,
        private confirmationService: ConfirmationService,
        private toastService: ToastService,
        private router: Router,
        private translocoLocaleService: TranslocoLocaleService,
        private tariffsService: TariffsService,
        private providedServicesService: ProvidedServicesService
    ) {}

    discounts: DiscountType[] = [];
    isConfirm = true;
    discountsQuery = new RequestHandler({
        requestFunction: this.tariffsService.fetchDiscounts,
        onSuccess: response => {
            this.discounts = response;
        },
        onError: error => {
            switch (error.status) {
                case 400:
                    this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                    break;
                default:
                    this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                    break;
            }
        },
    });
    deleteDiscountQuery = new RequestHandler({
        requestFunction: this.tariffsService.deleteDiscount,
        onSuccess: res => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.tariffs.discountDelete.toasts.successfulDelete')
            );
            this.discountsQuery.execute();
        },
        onError: error => {
            switch (error.status) {
                case 400:
                    this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                    break;
                default:
                    this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                    break;
            }
        },
    });

    ngOnInit() {
        this.discountsQuery.execute();
    }

    deleteDiscountConfirmation(data: any) {
        this.isConfirm = false;
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger',
            header: this.translateService.translate('modules.tariffs.discountDelete.title'),
            message: this.translateService.translate('modules.tariffs.discountDelete.description'),
            accept: () => {
                this.deleteDiscount(data);
            },
        });
    }

    deleteDiscount(data: any) {
        this.deleteDiscountQuery.execute(data.id);
    }

    navigateToEdit(data: any) {
        this.router.navigate(['tariffs/discounts/edit'], {
            state: { discount: data },
        });
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }

    formatDate(date: string | null): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale());
    }

    providedServiceName(providedServiceId: string) {
        return this.providedServicesService.getProvidedServiceTranslation(providedServiceId);
    }
}
