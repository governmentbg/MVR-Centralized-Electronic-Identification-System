import { Component, OnDestroy, OnInit } from '@angular/core';
import { NotificationsClientService } from '../../services/notifications-client.service';
import {
    IGetSystemsParams,
    INotificationEvent,
    IRejectedSystem,
    IRejectedSystemPaginatedData,
    ISystem,
    ISystemPaginatedData,
} from '../../interfaces/inotification';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService, LazyLoadEvent, MenuItem, MessageService } from 'primeng/api';
import { Table } from 'primeng/table/table';
import { Observable, Subscription } from 'rxjs';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-notifications',
    templateUrl: './notifications.component.html',
    styleUrls: ['./notifications.component.scss'],
    providers: [ConfirmationService, MessageService],
})
export class NotificationsComponent implements OnInit, OnDestroy {
    constructor(
        private notificationsClientService: NotificationsClientService,
        private translocoService: TranslocoService,
        private confirmationService: ConfirmationService,
        private messageService: MessageService
    ) {}

    systems: ISystem[] | IRejectedSystem[] = [];
    tabItems!: MenuItem[];
    activeItem!: string;
    actions = { approve: 'approve', deactivate: 'deactivate', reactivate: 'reactivate', reject: 'reject' };
    totalRecords = 0;
    pageSize = 10;
    value = 'approved';
    tabOptions: any[] = [];
    translocoSubscription!: Subscription;
    getSystemsSubscription: Subscription = new Subscription();
    approveSystemSubscription: Subscription = new Subscription();
    archiveSystemSubscription: Subscription = new Subscription();
    rejectSystemSubscription: Subscription = new Subscription();
    restoreSystemSubscription: Subscription = new Subscription();
    loading!: boolean;
    apiMethods: { [key: string]: (params: IGetSystemsParams) => Observable<any> } = {
        approved: (params: IGetSystemsParams) => this.notificationsClientService.getApprovedSystems(params),
        archived: (params: IGetSystemsParams) => this.notificationsClientService.getArchivedSystems(params),
        pending: (params: IGetSystemsParams) => this.notificationsClientService.getPendingSystems(params),
        rejected: (params: IGetSystemsParams) => this.notificationsClientService.getRejectedSystems(params),
    };
    rejectDialogVisible = false;
    selectedSystem!: ISystem;
    rejectForm = new FormGroup({
        reason: new FormControl<string>('', Validators.required),
    });
    table!: Table;

    ngOnInit() {
        this.loading = true;
        this.buildTabOptions();
        this.activeItem = this.tabOptions[0].value;
        this.translocoSubscription = this.translocoService.langChanges$.subscribe(() => {
            this.buildTabOptions();
        });
    }

    ngOnDestroy() {
        this.translocoSubscription.unsubscribe();
        this.getSystemsSubscription.unsubscribe();
        this.approveSystemSubscription.unsubscribe();
        this.archiveSystemSubscription.unsubscribe();
        this.rejectSystemSubscription.unsubscribe();
        this.restoreSystemSubscription.unsubscribe();
    }

    buildTabOptions(): void {
        this.tabOptions = [
            {
                label: this.translocoService.translate('modules.notifications.txtActiveNotificationsTabMenu'),
                value: 'approved',
            },
            {
                label: this.translocoService.translate('modules.notifications.txtArchivedNotificationsTabMenu'),
                value: 'archived',
            },
            {
                label: this.translocoService.translate('modules.notifications.txtAwaitingApprovalNotificationsTabMenu'),
                value: 'pending',
            },
            {
                label: this.translocoService.translate('modules.notifications.txtRejectedTab'),
                value: 'rejected',
            },
        ];
    }

    loadNotifications(event: LazyLoadEvent): void {
        this.loading = true;
        this.systems = [];
        this.totalRecords = 0;
        let page = 1;
        if (event && event.first && event.rows) {
            page = event.first / event.rows + 1;
        }

        this.getSystemsSubscription.add(
            this.apiMethods[this.activeItem]({
                pageSize: event.rows || this.pageSize,
                pageIndex: page,
            }).subscribe((response: ISystemPaginatedData | IRejectedSystemPaginatedData) => {
                this.systems = response.data;
                this.totalRecords = response.totalItems;
                this.loading = false;
            })
        );
    }

    activeItemChange(event: string, table: Table): void {
        this.activeItem = event;
        table.reset();
    }

    doAction(system: ISystem, action: string, table: Table): void {
        this.selectedSystem = system;
        this.table = table;
        switch (action) {
            case this.actions.approve:
                this.confirmationService.confirm({
                    rejectButtonStyleClass: 'p-button-danger',
                    message: this.translocoService.translate('modules.notifications.txtActivateConfirm'),
                    header: this.translocoService.translate('global.txtConfirmation'),
                    icon: 'pi pi-exclamation-triangle',
                    accept: () => {
                        this.approveSystemSubscription.add(
                            this.notificationsClientService.approveSystem(system.id).subscribe(() => {
                                table.reset();
                            })
                        );
                    },
                    reject: () => {
                        system.isApproved = false;
                    },
                });
                break;
            case this.actions.deactivate:
                this.confirmationService.confirm({
                    rejectButtonStyleClass: 'p-button-danger',
                    message: this.translocoService.translate('modules.notifications.txtArchiveConfirm'),
                    header: this.translocoService.translate('global.txtConfirmation'),
                    icon: 'pi pi-exclamation-triangle',
                    accept: () => {
                        this.archiveSystemSubscription.add(
                            this.notificationsClientService.archiveSystem(system.id).subscribe(() => {
                                table.reset();
                            })
                        );
                    },
                    reject: () => {
                        system.isDeleted = false;
                        system.isApproved = true;
                    },
                });
                break;
            case this.actions.reactivate:
                system.isDeleted = !system.isDeleted;
                this.confirmationService.confirm({
                    rejectButtonStyleClass: 'p-button-danger',
                    message: this.translocoService.translate('modules.notifications.txtActivateConfirm'),
                    header: this.translocoService.translate('global.txtConfirmation'),
                    icon: 'pi pi-exclamation-triangle',
                    accept: () => {
                        this.restoreSystemSubscription.add(
                            this.notificationsClientService.restoreSystem(system.id).subscribe(() => {
                                table.reset();
                            })
                        );
                    },
                    reject: () => {
                        system.isDeleted = true;
                    },
                });
                break;
            case this.actions.reject:
                this.toggleRejectFormVisibility();
                break;
        }
    }

    onRejectFormSubmit() {
        this.rejectForm.markAllAsTouched();
        this.rejectForm.controls.reason.markAsDirty();
        if (this.rejectForm.valid) {
            this.loading = true;
            this.rejectSystemSubscription.add(
                this.notificationsClientService
                    .rejectSystem(this.selectedSystem.id, this.rejectForm.controls.reason.value as string)
                    .subscribe({
                        next: () => {
                            this.toggleRejectFormVisibility();
                            this.table.reset();
                        },
                        error: error => {
                            switch (error.status) {
                                case 400:
                                    this.showErrorToast(this.translocoService.translate('global.txtInvalidDataError'));
                                    break;
                                default:
                                    this.showErrorToast(this.translocoService.translate('global.txtUnexpectedError'));
                                    break;
                            }
                            this.toggleRejectFormVisibility();
                            this.loading = false;
                            this.table.reset();
                        },
                    })
            );
        }
    }
    toggleRejectFormVisibility() {
        this.rejectForm.reset();
        this.rejectDialogVisible = !this.rejectDialogVisible;
    }

    showErrorToast(translation: string) {
        this.messageService.clear();
        this.messageService.add({
            key: 'toast',
            severity: 'error',
            summary: this.translocoService.translate('global.txtErrorTitle'),
            detail: translation,
        });
    }

    getEventTranslation(event: INotificationEvent): string {
        const foundTranslation = event.translations.find(
            (evt: any) => evt.language === this.translocoService.getActiveLang()
        );
        return foundTranslation ? foundTranslation.description : event.code;
    }

    getSystemNameTranslation(system: ISystem): string {
        const foundTranslation = system.translations.find(
            (sys: any) => sys.language === this.translocoService.getActiveLang()
        );
        return foundTranslation ? foundTranslation.name : system.name;
    }

    get modifiedOnColumnLabel() {
        switch (this.activeItem) {
            case 'approved':
                return this.translocoService.translate('modules.notifications.txtActiveChannelsModifiedOnTitle');
            case 'pending':
                return this.translocoService.translate('modules.notifications.txtAwaitingChannelsModifiedOnTitle');
            case 'archived':
                return this.translocoService.translate('modules.notifications.txtInactiveChannelsModifiedOnTitle');
            case 'rejected':
                return this.translocoService.translate('modules.notifications.txtRejectedChannelsModifiedOnTitle');
            default:
                return '';
        }
    }
}
