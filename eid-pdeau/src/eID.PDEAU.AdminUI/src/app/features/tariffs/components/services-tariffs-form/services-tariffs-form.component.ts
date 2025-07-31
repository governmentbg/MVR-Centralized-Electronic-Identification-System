import { Component, OnDestroy } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService } from 'primeng/api';
import { ToastService } from '@app/shared/services/toast.service';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { TariffsService } from '@app/features/tariffs/tariffs.service';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ProvidedServicesService } from '@app/features/tariffs/services/provided-services.service';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';

@Component({
    selector: 'app-services-tariffs-form',
    templateUrl: './services-tariffs-form.component.html',
    styleUrls: ['./services-tariffs-form.component.scss'],
    providers: [ConfirmationService],
})
export class ServicesTariffsFormComponent implements OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private confirmationService: ConfirmationService,
        private toastService: ToastService,
        private tariffsService: TariffsService,
        private router: Router,
        private providedServicesService: ProvidedServicesService,
        private translocoLocaleService: TranslocoLocaleService
    ) {
        if (this.router.url === '/tariffs/services/new') {
            this.isEditMode = false;
        } else if (this.router.url === '/tariffs/services/edit') {
            this.isEditMode = true;
        }

        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.tariffs.servicesTariffsTable.title'),
                    routerLink: '/tariffs',
                },
                {
                    label: this.translateService.translate(this.breadcrumbTitle),
                },
            ];
            this.providedServicesNameKey = this.translateService.getActiveLang() === 'bg' ? 'name' : 'nameLatin';
        });

        if (!this.isEditMode) return;
        const navigation = this.router.getCurrentNavigation();
        if (navigation) {
            this.state = navigation.extras.state as any;
            if (this.state) {
                this.form.patchValue({
                    startDate: this.state.tariff.startDate,
                    price: this.state.tariff.primaryPrice,
                    currency: this.state.tariff.primaryCurrency,
                    providedServiceId: this.state.tariff.providedServiceId,
                });
                this.tariffId = this.state.tariff.id;
            }
        } else {
            this.router.navigate(['tariffs/services']);
        }
    }

    isEditMode = false;
    languageChangeSubscription: Subscription;
    breadcrumbItems: IBreadCrumbItems[] = [];
    isLoading = false;
    providedServices = this.providedServicesService.providedServices;
    state: any = {};
    minDate = new Date();
    providedServicesNameKey = 'name';
    tariffId = '';
    currencies = ['BGN', 'EUR'];

    get breadcrumbTitle() {
        return this.isEditMode ? 'modules.tariffs.tariffEdit.title' : 'modules.tariffs.tariffCreate.title';
    }

    form = new FormGroup({
        startDate: new FormControl<string | null>(null, Validators.required),
        price: new FormControl<string | null>(null, Validators.required),
        providedServiceId: new FormControl<string | null>(null, Validators.required),
        currency: new FormControl<string>('BGN', Validators.required),
    });

    addTariffQuery = new RequestHandler({
        requestFunction: this.tariffsService.createTariff,
        onSuccess: res => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.tariffs.tariffCreate.toasts.successfulCreation')
            );
            this.router.navigate(['/tariffs/services']);
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
        onInit: () => {
            this.form.disable();
        },
        onComplete: () => {
            this.form.enable();
        },
    });

    updateTariffQuery = new RequestHandler({
        requestFunction: this.tariffsService.updateTariff,
        onSuccess: res => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.tariffs.tariffDelete.toasts.successfulCreation')
            );
            this.router.navigate(['/tariffs/services']);
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
        onInit: () => {
            this.form.disable();
        },
        onComplete: () => {
            this.form.enable();
        },
    });

    addTariff() {
        this.isLoading = true;
        const payload = this.form.getRawValue();
        payload.startDate = this.formatDate(payload.startDate);
        this.addTariffQuery.execute(payload);
    }

    editTariffConfirmation() {
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger',
            icon: 'pi pi-exclamation-triangle',
            header: this.translateService.translate('global.txtConfirmation'),
            message: this.translateService.translate('modules.tariffs.confirmChange'),
            accept: () => {
                this.editTariff();
            },
        });
    }

    editTariff() {
        const payload = this.form.getRawValue();
        payload.startDate = this.formatDate(payload.startDate);
        this.updateTariffQuery.execute(payload, this.tariffId);
    }

    onSubmit() {
        Object.keys(this.form.controls).forEach(key => {
            if (
                (this.form.get(key) && typeof this.form.get(key)?.value === 'string') ||
                this.form.get(key)?.value instanceof String
            ) {
                this.form.get(key)?.setValue(this.form.get(key)?.value.trim());
            }
            this.form.get(key)?.markAsDirty();
        });
        this.form.markAllAsTouched();
        if (this.form.valid) {
            if (this.isEditMode) {
                this.editTariffConfirmation();
            } else {
                this.addTariff();
            }
        }
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }

    formatDate(date: string | null | Date): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale());
    }
}
