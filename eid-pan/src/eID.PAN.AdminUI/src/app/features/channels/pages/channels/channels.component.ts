import { Component, OnDestroy, OnInit } from '@angular/core';
import { ChannelsClientService } from '../../services/channels-client.service';
import { IChannel, IChannelResponseData } from '../../interfaces/ichannel';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-channels',
    templateUrl: './channels.component.html',
    styleUrls: ['./channels.component.scss'],
    providers: [ConfirmationService, MessageService],
})
export class ChannelsComponent implements OnInit, OnDestroy {
    constructor(
        private channelsClientService: ChannelsClientService,
        private translocoService: TranslocoService,
        private confirmationService: ConfirmationService,
        private messageService: MessageService
    ) {}

    channels: IChannel[] = [];
    selectedChannels: IChannel[] = [];
    value = 'approved';
    activeItem!: string;
    actions = {
        approve: 'approve',
        deactivate: 'deactivate',
        reactivate: 'reactivate',
        reject: 'reject',
        test: 'test',
    };
    tabOptions: any[] = [];
    translocoSubscription!: Subscription;
    getChannelsSubscription: Subscription = new Subscription();
    approveChannelSubscription: Subscription = new Subscription();
    archiveChannelSubscription: Subscription = new Subscription();
    rejectChannelSubscription: Subscription = new Subscription();
    restoreChannelSubscription: Subscription = new Subscription();
    loading!: boolean;
    rejectDialogVisible = false;
    selectedChannel!: IChannel;
    rejectForm = new FormGroup({
        reason: new FormControl<string>('', Validators.required),
    });

    ngOnInit() {
        this.loading = true;
        this.buildTabOptions();
        this.loadNotificationChannels();
        this.activeItem = this.tabOptions[0].value;
        this.translocoSubscription = this.translocoService.langChanges$.subscribe(() => {
            this.buildTabOptions();
        });
    }

    ngOnDestroy() {
        this.translocoSubscription.unsubscribe();
        this.getChannelsSubscription.unsubscribe();
        this.approveChannelSubscription.unsubscribe();
        this.archiveChannelSubscription.unsubscribe();
        this.rejectChannelSubscription.unsubscribe();
        this.restoreChannelSubscription.unsubscribe();
    }

    buildTabOptions() {
        this.tabOptions = [
            {
                label: this.translocoService.translate('modules.channels.txtActiveTab'),
                value: 'approved',
            },
            {
                label: this.translocoService.translate('modules.channels.txtInactiveTab'),
                value: 'archived',
            },
            {
                label: this.translocoService.translate('modules.channels.txtAwaitingTab'),
                value: 'pending',
            },
            {
                label: this.translocoService.translate('modules.channels.txtRejectedTab'),
                value: 'rejected',
            },
        ];
    }

    loadNotificationChannels() {
        this.loading = true;
        this.channels = [];
        this.getChannelsSubscription.add(
            this.channelsClientService.getChannels().subscribe((response: IChannelResponseData) => {
                switch (this.activeItem) {
                    case 'approved':
                        this.channels = response.approved;
                        break;
                    case 'pending':
                        this.channels = response.pending;
                        break;
                    case 'archived':
                        this.channels = response.archived;
                        break;
                    case 'rejected':
                        this.channels = response.rejected;
                        break;
                    default:
                        this.channels = [];
                }
                this.loading = false;
            })
        );
    }

    activeItemChange(event: string) {
        this.activeItem = event;
        this.loadNotificationChannels();
    }

    doAction(channel: IChannel, action: string): void {
        this.selectedChannel = channel;
        switch (action) {
            case this.actions.approve:
                this.confirmationService.confirm({
                    rejectButtonStyleClass: 'p-button-danger',
                    message: this.translocoService.translate('modules.channels.txtActivateConfirm'),
                    header: this.translocoService.translate('global.txtConfirmation'),
                    icon: 'pi pi-exclamation-triangle',
                    accept: () => {
                        this.loading = true;
                        this.approveChannelSubscription.add(
                            this.channelsClientService.approveChannel(channel.id).subscribe({
                                next: () => {
                                    this.loadNotificationChannels();
                                },
                                error: error => {
                                    switch (error.status) {
                                        case 400:
                                            this.showErrorToast(
                                                this.translocoService.translate('global.txtInvalidDataError')
                                            );
                                            break;
                                        default:
                                            this.showErrorToast(
                                                this.translocoService.translate('global.txtUnexpectedError')
                                            );
                                            break;
                                    }
                                    this.loading = false;
                                    this.loadNotificationChannels();
                                },
                            })
                        );
                    },
                    reject: () => {
                        this.loadNotificationChannels();
                    },
                });
                break;
            case this.actions.deactivate:
                this.confirmationService.confirm({
                    rejectButtonStyleClass: 'p-button-danger',
                    message: this.translocoService.translate('modules.channels.txtArchiveConfirm'),
                    header: this.translocoService.translate('global.txtConfirmation'),
                    icon: 'pi pi-exclamation-triangle',
                    accept: () => {
                        this.loading = true;
                        this.archiveChannelSubscription.add(
                            this.channelsClientService.archiveChannel(channel.id).subscribe({
                                next: () => {
                                    this.loadNotificationChannels();
                                },
                                error: error => {
                                    switch (error.status) {
                                        case 400:
                                            this.showErrorToast(
                                                this.translocoService.translate('global.txtInvalidDataError')
                                            );
                                            break;
                                        default:
                                            this.showErrorToast(
                                                this.translocoService.translate('global.txtUnexpectedError')
                                            );
                                            break;
                                    }
                                    this.loading = false;
                                    this.loadNotificationChannels();
                                },
                            })
                        );
                    },
                    reject: () => {
                        this.loadNotificationChannels();
                    },
                });
                break;
            case this.actions.reactivate:
                this.confirmationService.confirm({
                    rejectButtonStyleClass: 'p-button-danger',
                    message: this.translocoService.translate('modules.channels.txtActivateConfirm'),
                    header: this.translocoService.translate('global.txtConfirmation'),
                    icon: 'pi pi-exclamation-triangle',
                    accept: () => {
                        this.loading = true;
                        this.restoreChannelSubscription.add(
                            this.channelsClientService.restoreChannel(channel.id).subscribe({
                                next: () => {
                                    this.loadNotificationChannels();
                                },
                                error: error => {
                                    switch (error.status) {
                                        case 400:
                                            this.showErrorToast(
                                                this.translocoService.translate('global.txtInvalidDataError')
                                            );
                                            break;
                                        case 409:
                                            this.showErrorToast(
                                                this.translocoService.translate(
                                                    'modules.channels.txtErrorNameAlreadyExists'
                                                )
                                            );
                                            break;
                                        default:
                                            this.showErrorToast(
                                                this.translocoService.translate('global.txtUnexpectedError')
                                            );
                                            break;
                                    }
                                    this.loading = false;
                                    this.loadNotificationChannels();
                                },
                            })
                        );
                    },
                    reject: () => {
                        this.loadNotificationChannels();
                    },
                });
                break;
            case this.actions.reject:
                this.toggleRejectFormVisibility();
                break;
            case this.actions.test:
                this.testChannel(channel.id);
                break;
        }
    }

    getChannelNameTranslation(channel: IChannel) {
        const foundTranslation = channel.translations.find(
            (evt: any) => evt.language === this.translocoService.getActiveLang()
        );
        return foundTranslation ? foundTranslation.name : channel.name;
    }

    getChannelDescriptionTranslation(channel: IChannel) {
        const foundTranslation = channel.translations.find(
            (evt: any) => evt.language === this.translocoService.getActiveLang()
        );
        return foundTranslation ? foundTranslation.description : channel.description;
    }

    onRejectFormSubmit() {
        this.rejectForm.markAllAsTouched();
        this.rejectForm.controls.reason.markAsDirty();
        if (this.rejectForm.valid) {
            this.loading = true;
            this.rejectChannelSubscription.add(
                this.channelsClientService
                    .rejectChannel(this.selectedChannel.id, this.rejectForm.controls.reason.value as string)
                    .subscribe({
                        next: () => {
                            this.toggleRejectFormVisibility();
                            this.loadNotificationChannels();
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
                            this.loading = false;
                            this.loadNotificationChannels();
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

    showSuccessToast(translation: string) {
        this.messageService.clear();
        this.messageService.add({
            key: 'toast',
            severity: 'success',
            summary: this.translocoService.translate('global.txtSuccessTitle'),
            detail: translation,
        });
    }

    testChannel(id: string) {
        this.loading = true;
        this.channelsClientService.testChannel(id).subscribe({
            next: response => {
                this.loading = false;
                if (response.isSuccess) {
                    this.showSuccessToast(this.translocoService.translate('modules.channels.txtSuccessfulTest'));
                } else {
                    this.showErrorToast(
                        this.translocoService.translate('modules.channels.txtUnsuccessfulTest') +
                            ` (${response.statusCode})`
                    );
                }
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
                this.loading = false;
            },
        });
    }

    get modifiedOnColumnLabel() {
        switch (this.activeItem) {
            case 'approved':
                return this.translocoService.translate('modules.channels.txtActiveChannelsModifiedOnTitle');
            case 'pending':
                return this.translocoService.translate('modules.channels.txtAwaitingChannelsModifiedOnTitle');
            case 'archived':
                return this.translocoService.translate('modules.channels.txtInactiveChannelsModifiedOnTitle');
            case 'rejected':
                return this.translocoService.translate('modules.channels.txtRejectedChannelsModifiedOnTitle');
            default:
                return '';
        }
    }
}
