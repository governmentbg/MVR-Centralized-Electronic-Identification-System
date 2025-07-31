import { Component, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService } from 'primeng/api';
import { ToastService } from '@app/shared/services/toast.service';
import { Router } from '@angular/router';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { TariffsService } from '@app/features/tariffs/tariffs.service';
import { TariffType } from '@app/features/tariffs/tariffs.dto';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ProvidedServicesService } from '@app/features/tariffs/services/provided-services.service';

@Component({
    selector: 'app-services-tariffs',
    templateUrl: './services-tariffs.component.html',
    styleUrls: ['./services-tariffs.component.scss'],
    providers: [ConfirmationService],
})
export class ServicesTariffsComponent implements OnInit {
    constructor(
        public translateService: TranslocoService,
        private confirmationService: ConfirmationService,
        private toastService: ToastService,
        private router: Router,
        private translocoLocaleService: TranslocoLocaleService,
        private tariffsService: TariffsService,
        private providedServicesService: ProvidedServicesService
    ) {}

    tariffs: TariffType[] = [];
    isConfirm = true;
    tariffsQuery = new RequestHandler({
        requestFunction: this.tariffsService.fetchTariffs,
        onSuccess: response => {
            this.tariffs = response;
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
    deleteTariffQuery = new RequestHandler({
        requestFunction: this.tariffsService.deleteTariff,
        onSuccess: res => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.tariffs.tariffDelete.toasts.successfulDelete')
            );
            this.tariffsQuery.execute();
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
        this.tariffsQuery.execute();
    }

    deleteTariffConfirmation(data: any) {
        this.isConfirm = false;
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger',
            header: this.translateService.translate('modules.tariffs.tariffDelete.title'),
            message: this.translateService.translate('modules.tariffs.tariffDelete.description'),
            accept: () => {
                this.deleteTariff(data);
            },
        });
    }

    deleteTariff(tariff: any) {
        this.deleteTariffQuery.execute(tariff.id);
    }

    navigateToEdit(data: TariffType) {
        this.router.navigate(['tariffs/services/edit'], {
            state: { tariff: data },
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

    price(tariff: any) {
        return `${tariff.primaryPrice} ${tariff.primaryCurrency} / ${tariff.secondaryPrice}  ${tariff.secondaryCurrency}`;
    }
}
