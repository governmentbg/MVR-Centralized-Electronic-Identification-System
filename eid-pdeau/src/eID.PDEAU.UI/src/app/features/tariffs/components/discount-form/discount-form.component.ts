import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService } from 'primeng/api';
import { ToastService } from '@app/shared/services/toast.service';
import { TariffsService } from '@app/features/tariffs/tariffs.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { moreThanOrEqualValidate } from '@app/shared/validators/range-validator';
import { ProvidedServicesService } from '@app/features/tariffs/services/provided-services.service';

@Component({
    selector: 'app-discount-form',
    templateUrl: './discount-form.component.html',
    styleUrls: ['./discount-form.component.scss'],
    providers: [ConfirmationService],
})
export class DiscountFormComponent implements OnDestroy, OnInit {
    constructor(
        public translateService: TranslocoService,
        private confirmationService: ConfirmationService,
        private toastService: ToastService,
        private tariffsService: TariffsService,
        private router: Router,
        private translocoLocaleService: TranslocoLocaleService,
        private providedServicesService: ProvidedServicesService,
    ) {
        if (this.router.url === '/tariffs/discounts/new') {
            this.isEditMode = false;
        } else if (this.router.url === '/tariffs/discounts/edit') {
            this.isEditMode = true;
        }

        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.tariffs.discountsTable.title'),
                    routerLink: '/tariffs/discounts',
                },
                {
                    label: this.translateService.translate(this.breadcrumbTitle),
                },
            ];
        });

        if (!this.isEditMode) return;
        const navigation = this.router.getCurrentNavigation();
        if (navigation) {
            this.state = navigation.extras.state as any;
            if (this.state) {
                this.form.patchValue({
                    startDate: this.state.discount.startDate,
                    ageFrom: this.state.discount.ageFrom,
                    ageUntil: this.state.discount.ageUntil,
                    disability: this.state.discount.disability,
                    discount: this.state.discount.discount,
                    providedServiceId: this.state.discount.providedServiceId,
                });
                this.discountId = this.state.discount.id;
            }
        } else {
            this.router.navigate(['tariffs/discounts']);
        }
    }

    isEditMode = false;
    languageChangeSubscription: Subscription;
    formChangeSubscription = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    isLoading = false;
    state: any = {};
    minDate = new Date();
    discountId = '';
    providedServicesNameKey = 'name';
    providedServices = this.providedServicesService.providedServices;

    get breadcrumbTitle() {
        return this.isEditMode ? 'modules.tariffs.discountEdit.title' : 'modules.tariffs.discountCreate.title';
    }

    form = new FormGroup({
        startDate: new FormControl<string | null>(null, Validators.required),
        ageFrom: new FormControl(1, Validators.required),
        ageUntil: new FormControl(1, [Validators.required, moreThanOrEqualValidate('ageFrom'), Validators.max(130)]),
        discount: new FormControl(1, Validators.required),
        disability: new FormControl(false, Validators.required),
        providedServiceId: new FormControl<string | null>(null, Validators.required),
    });

    addDiscountQuery = new RequestHandler({
        requestFunction: this.tariffsService.createDiscount,
        onSuccess: res => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.tariffs.discountCreate.toasts.successfulCreation')
            );
            this.router.navigate(['/tariffs/discounts']);
        },
        onError: error => {
            this.form.enable();
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
    });

    updateDiscountQuery = new RequestHandler({
        requestFunction: this.tariffsService.updateDiscount,
        onSuccess: res => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.tariffs.discountCreate.toasts.successfulCreation')
            );
            this.router.navigate(['/tariffs/discounts']);
        },
        onError: error => {
            this.form.enable();
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
    });

    ngOnInit() {
        this.formChangeSubscription = this.form.controls.ageFrom.valueChanges.subscribe({
            next: () => {
                this.form.controls.ageUntil.markAsDirty();
                this.form.controls.ageUntil.updateValueAndValidity();
            },
        });
    }

    addTariff() {
        this.isLoading = true;
        const payload = this.form.getRawValue();
        payload.startDate = this.formatDate(payload.startDate);
        this.addDiscountQuery.execute(payload);
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
        this.updateDiscountQuery.execute(payload, this.discountId);
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
        this.formChangeSubscription.unsubscribe();
    }

    formatDate(date: string | null | Date): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale());
    }
}
