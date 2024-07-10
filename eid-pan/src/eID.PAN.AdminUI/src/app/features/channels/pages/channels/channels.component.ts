import { Component, OnDestroy, OnInit } from '@angular/core';
import { ChannelsClientService } from '../../services/channels-client.service';
import { IChannel, IChannelResponseData } from '../../interfaces/ichannel';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';

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
    actions = { approve: 'approve', deactivate: 'deactivate', reactivate: 'reactivate', reject: 'reject' };
    tabOptions: any[] = [];
    translocoSubscription!: Subscription;
    getChannelsSubscription: Subscription = new Subscription();
    approveChannelSubscription: Subscription = new Subscription();
    archiveChannelSubscription: Subscription = new Subscription();
    rejectChannelSubscription: Subscription = new Subscription();
    restoreChannelSubscription: Subscription = new Subscription();
    loading!: boolean;

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
        switch (action) {
            case this.actions.approve:
                this.confirmationService.confirm({
                    rejectButtonStyleClass: 'p-button-danger',
                    message: this.translocoService.translate('modules.channels.txtActivateConfirm'),
                    header: this.translocoService.translate('global.txtConfirmation'),
                    icon: 'pi pi-exclamation-triangle',
                    accept: () => {
                        this.approveChannelSubscription.add(
                            this.channelsClientService.approveChannel(channel.id).subscribe(() => {
                                this.loadNotificationChannels();
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
                        this.archiveChannelSubscription.add(
                            this.channelsClientService.archiveChannel(channel.id).subscribe(() => {
                                this.loadNotificationChannels();
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
                        this.restoreChannelSubscription.add(
                            this.channelsClientService.restoreChannel(channel.id).subscribe({
                                next: () => {
                                    this.loadNotificationChannels();
                                },
                                error: error => {
                                    if (error.status === 409) {
                                        this.showErrorToast();
                                    }
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
                this.confirmationService.confirm({
                    rejectButtonStyleClass: 'p-button-danger',
                    message: this.translocoService.translate('modules.channels.txtRejectConfirm'),
                    header: this.translocoService.translate('global.txtConfirmation'),
                    icon: 'pi pi-exclamation-triangle',
                    accept: () => {
                        this.rejectChannelSubscription.add(
                            this.channelsClientService.rejectChannel(channel.id).subscribe({
                                next: () => {
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

    showErrorToast() {
        this.messageService.clear();
        this.messageService.add({
            key: 'toast',
            severity: 'error',
            summary: this.translocoService.translate('global.txtErrorTitle'),
            detail: this.translocoService.translate('modules.channels.txtErrorNameAlreadyExists'),
        });
    }
}
