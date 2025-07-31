import { Component, OnInit } from '@angular/core';
import { IntegrityClientService } from '../../services/integrity.service';
import { TranslocoService } from '@ngneat/transloco';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { ConfirmationService, MessageService } from 'primeng/api';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import {
    Subject,
    Subscription,
    catchError,
    delay,
    mergeMap,
    of,
    repeat,
    takeUntil,
    timeout,
} from 'rxjs';
import {
    dateIsBeforeOtherDate,
    notMoreThanThreeMonths,
} from '../../validators/date';
import { Status, Type } from '../../enums/integrity.enum';
import { IIntegrityConfig } from '../../interfaces/IAppConfig';

@Component({
    selector: 'app-integrity',
    templateUrl: './integrity.component.html',
    styleUrls: ['./integrity.component.scss'],
    providers: [MessageService, ConfirmationService],
})
export class IntegrityComponent implements OnInit {
    constructor(
        private translocoService: TranslocoService,
        private messageService: MessageService,
        private integrityClientService: IntegrityClientService,
        private confirmationService: ConfirmationService,
        private translocoLocaleService: TranslocoLocaleService
    ) {
        const integrity = localStorage.getItem('integrity') as string;
        if (integrity.length > 0) {
            this.integrityConfig = JSON.parse(integrity);
        }
        this.lastUpdateTimestamp = new Date();
    }

    isLoading = false;
    verificationStatus: any;
    integrityConfig: IIntegrityConfig = {
        downloadBytesFileSize: 500000,
    };
    tabOptions: any[] = [];
    value = Type.System;
    activeItem!: string;
    translocoSubscription!: Subscription;
    isSubSystem = false;
    isSystemSuccess = false;
    isSubSystemSuccess = true;
    selectedSubSystem: any = {};
    subSystemVerifyPeriodDate: any;
    isSearchManuallyTriggered = false;
    lastUpdateTimestamp: any;
    delayedLoadingText = false;
    form = new FormGroup({
        fromDate: new FormControl<Date | null>(
            new Date(new Date().setDate(new Date().getDate() - 1)),
            [
                Validators.required,
                notMoreThanThreeMonths(),
                dateIsBeforeOtherDate(),
            ]
        ),
        toDate: new FormControl<Date | null>(new Date(), [
            Validators.required,
            notMoreThanThreeMonths(),
            dateIsBeforeOtherDate(),
        ]),
    });
    areDatesWithoutLogsVisible = false;

    toDateMinValue(): Date {
        return this.form.controls.fromDate.value || new Date();
    }

    get isProcessing() {
        return (
            this.verificationStatus &&
            this.verificationStatus.currentStatus &&
            this.verificationStatus.currentStatus === Status.Processing
        );
    }

    get isLastStateUnknown() {
        return this.verificationStatus && this.verificationStatus.lastState === null;
    }

    get subSystemLastProcessingStart() {
        let lastProcessingStart = this.formatFullDate(this.lastUpdateTimestamp);
        if (
            this.verificationStatus &&
            this.verificationStatus.lastProcessingStart &&
            !this.isSearchManuallyTriggered
        ) {
            lastProcessingStart = this.formatFullDate(
                this.verificationStatus.lastProcessingStart
            );
        }
        return lastProcessingStart;
    }

    get subSystemVerifyPeriod() {
        if (
            this.verificationStatus.serviceConfiguration &&
            this.verificationStatus.serviceConfiguration.verifyPeriod
        ) {
            if (
                this.subSystemVerifyPeriodDate &&
                this.isSearchManuallyTriggered
            ) {
                return this.subSystemVerifyPeriodDate;
            } else {
                return this.translocoService.translate(
                    'modules.integrity.periods.' +
                    this.verificationStatus.serviceConfiguration
                        .verifyPeriod
                );
            }
        } else {
            return '';
        }
    }

    get systems() {
        return this.verificationStatus?.lastState?.systems;
    }

    ngOnInit() {
        this.buildTabOptions();
        this.getVerificationStatus();
        this.activeItem = this.tabOptions[0].value;
        this.translocoSubscription =
            this.translocoService.langChanges$.subscribe(() => {
                this.buildTabOptions();
            });
    }

    loading(flag: boolean) {
        this.isLoading = flag;
        if (this.isLoading || this.isProcessing) {
            this.form.get('fromDate')?.disable();
            this.form.get('toDate')?.disable();
        } else {
            this.form.get('fromDate')?.enable();
            this.form.get('toDate')?.enable();
        }
    }

    buildTabOptions() {
        this.tabOptions = [
            {
                label: this.translocoService.translate(
                    'modules.integrity.txtSystemStatus'
                ),
                value: Type.System,
            },
            {
                label: this.translocoService.translate(
                    'modules.integrity.txtSubSystemStatus'
                ),
                value: Type.SubSystem,
            },
        ];
    }

    activeItemChange(event: string) {
        this.activeItem = event;
        this.isSubSystem = !this.isSubSystem;
    }

    getVerificationStatus() {
        this.loading(true);
        // When refresh is triggered set values to initial state
        this.form.patchValue({
            fromDate: new Date(new Date().setDate(new Date().getDate() - 1)),
            toDate: new Date(),
        });
        this.subSystemVerifyPeriodDate = null;
        this.integrityClientService.getVerificationStatus().subscribe({
            next: (response: any) => {
                this.verificationStatus = response;
                this.isSystemSuccess =
                    this.verificationStatus.lastState?.isValid;
                this.loading(false);
                this.preselectCurrentSubSystem();
                this.getFilesForEachSystemForPreview();
            },
            error: (error) => {
                this.showErrorToast(
                    this.translocoService.translate(
                        'modules.journalLogs.errors.txtError'
                    )
                );
                this.loading(false);
            },
        });
    }

    checkIntegrityConfirmationPopup() {
        this.form.markAllAsTouched();
        if (this.form.valid) {
            this.form.markAsPristine();
            this.confirmationService.confirm({
                rejectButtonStyleClass: 'p-button-danger',
                message: this.translocoService.translate(
                    'modules.integrity.txtPleaseConfirmYourAction'
                ),
                header: this.translocoService.translate(
                    'global.txtConfirmation'
                ),
                icon: 'pi pi-exclamation-triangle',
                accept: () => {
                    this.isSearchManuallyTriggered = true;
                    this.checkIntegrityByPeriodRequest();
                },
            });
        }
    }

    checkIntegrityByPeriodRequest() {
        this.loading(true);
        setTimeout(() => {
            this.delayedLoadingText = true;
        }, 4000);
        const payload = {
            systemId: this.integrityConfig.systemId,
            startDate: new Date(
                this.form.controls.fromDate.value as Date
            ).toISOString(),
            endDate: new Date(
                this.form.controls.toDate.value as Date
            ).toISOString(),
        };

        this.integrityClientService.checkIntegrityByPeriod(payload).subscribe({
            next: (response: any) => {
                if (response.resultType === 'TaskAlreadyInProgress') {
                    this.loading(false);
                    this.showErrorToast(
                        this.translocoService.translate(
                            'modules.integrity.errors.txtTaskAlreadyInProgressError'
                        ),
                        'Info'
                    );
                } else {
                    this.integrityCheckRequestPolling(response);
                }
            },
            error: (error) => {
                this.showErrorToast(
                    this.translocoService.translate(
                        'modules.integrity.errors.txtError'
                    )
                );
                this.loading(false);
            },
        });
    }

    integrityCheckRequestPolling(requestData: any) {
        const stopPolling = new Subject();
        const poll = of({}).pipe(
            // restart http when taskId is 0000000
            mergeMap((_) =>
                this.integrityClientService.getResult(requestData).pipe(
                    catchError((error) => {
                        return of(false);
                    })
                )
            ),
            delay(this.integrityConfig.poolingRequestInterval as number),
            repeat(),
            takeUntil(stopPolling),
            timeout(this.integrityConfig.poolingTimeout as number)
        );

        poll.subscribe({
            next: (response: any) => {
                if (
                    response.status === 200 &&
                    response.body.systems.length > 0
                ) {
                    // Emit the stop
                    stopPolling.next(true);

                    // Endpoint will always return one item in the systems array
                    // because request is invoked with applied filter query param 'systemId'

                    this.selectedSubSystem = response.body.systems[0];
                    this.isSubSystemSuccess = this.selectedSubSystem.isValid;
                    this.getFileForSubSystemForPreview();

                    this.subSystemVerifyPeriodDate = `${this.formatDate(
                        this.form.controls.fromDate.value
                    )} - ${this.formatDate(this.form.controls.toDate.value)}`;
                    this.lastUpdateTimestamp = new Date();
                    this.loading(false);
                    this.delayedLoadingText = false;
                }
            },
            error: (error) => {
                this.showErrorToast(
                    this.translocoService.translate(
                        'modules.integrity.errors.txtError'
                    )
                );
                this.loading(false);
                this.delayedLoadingText = false;
            },
        });
    }

    preselectCurrentSubSystem() {
        this.selectedSubSystem = this.verificationStatus.lastState?.systems.find(
            (system: any) => {
                return system.systemId === this.integrityConfig.systemId;
            }
        );
        if (
            this.verificationStatus.lastState?.systems.length > 0 &&
            this.selectedSubSystem &&
            Object.keys(this.selectedSubSystem).length > 0
        ) {
            this.isSubSystemSuccess = this.selectedSubSystem.isValid;
        }
    }

    showErrorToast(message: string, severity: string = 'error') {
        this.messageService.clear();
        this.messageService.add({
            key: 'toast',
            severity: severity,
            summary: this.translocoService.translate('global.txtErrorTitle'),
            detail: message,
        });
    }

    formatFullDate(date: Date): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(
            new Date(date),
            this.translocoLocaleService.getLocale(),
            {timeStyle: 'medium'}
        );
    }

    formatDate(date: Date | null): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(
            new Date(date),
            this.translocoLocaleService.getLocale()
        );
    }

    validateToDate() {
        this.form.controls['toDate'].updateValueAndValidity();
    }

    validateFromDate() {
        this.form.controls['fromDate'].updateValueAndValidity();
    }

    refresh() {
        this.isSearchManuallyTriggered = false;
        this.getVerificationStatus();
    }

    getResultFileForDownload(filePath: string) {
        this.integrityClientService.getResultFile(filePath).subscribe({
            next: (response: any) => {
                const fileName = response.headers
                    .get('content-disposition')
                    ?.split(';')[1]
                    .split('=')[1];
                const blob: Blob = new Blob([JSON.stringify(response.body)], {
                    type: 'application/octet-stream',
                });
                const a = document.createElement('a');
                a.download = fileName;
                a.href = window.URL.createObjectURL(blob);
                a.click();
                this.loading(false);
            },
            error: (error) => {
                this.showErrorToast(
                    this.translocoService.translate(
                        'modules.journalLogs.errors.txtError'
                    )
                );
                this.loading(false);
            },
        });
    }

    getFilesForEachSystemForPreview() {
        this.systems.map((system: any) => {
            if (
                !system.isValid &&
                system.resultLogFileSize <=
                this.integrityConfig.downloadBytesFileSize
            ) {
                this.integrityClientService
                    .getResultFile(system.resultLogFile)
                    .subscribe({
                        next: (response: any) => {
                            system.files = response.body.Files;
                            this.loading(false);
                        },
                        error: (error) => {
                            this.showErrorToast(
                                this.translocoService.translate(
                                    'modules.journalLogs.errors.txtError'
                                )
                            );
                            this.loading(false);
                        },
                    });
            }
        });
    }

    getFileForSubSystemForPreview() {
        if (
            !this.selectedSubSystem.isValid &&
            this.selectedSubSystem.resultLogFileSize <=
            this.integrityConfig.downloadBytesFileSize
        ) {
            this.loading(true);
            this.integrityClientService
                .getResultFile(this.selectedSubSystem.resultLogFile)
                .subscribe({
                    next: (response: any) => {
                        this.selectedSubSystem.files = response.body.Files;
                        this.loading(false);
                    },
                    error: (error) => {
                        this.showErrorToast(
                            this.translocoService.translate(
                                'modules.journalLogs.errors.txtError'
                            )
                        );
                        this.loading(false);
                    },
                });
        }
    }

    toggleDatesWithoutLogs() {
        this.areDatesWithoutLogsVisible = !this.areDatesWithoutLogsVisible;
    }
}
