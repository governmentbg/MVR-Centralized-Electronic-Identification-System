import { Component, OnDestroy } from '@angular/core';
import { UserNotificationsClientService } from '../services/user-notifications-client.service';
import { UserNotificationChannelsClientService } from '../services/user-notification-channels-client.service';
import { TranslocoService } from '@ngneat/transloco';
import {
    IChannel,
    INotification,
    INotificationEvent,
    IUserNotificationChannel,
    IUserNotificationChannelParams,
} from '../interfaces/inotification';
import { LazyLoadEvent } from 'primeng/api';
import { NotificationType } from '../enums/notification-settings.enum';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-notification-settings',
    templateUrl: './notification-settings.component.html',
    styleUrls: ['./notification-settings.component.scss'],
})
export class NotificationSettingsComponent implements OnDestroy {
    systems: INotification[] = [];
    notificationChannels: any[] = [];
    selectedNotificationChannels: any[] = [];

    get deactivatedNotifications(): any[] {
        return this.systems
            .map((system: INotification) => {
                return (
                    system.events
                        // We are interested of the ids from events that are NOT activated
                        .filter(ev => !ev.activated)
                        .map((event: INotificationEvent) => {
                            return event.id;
                        })
                );
            })
            .flat(Infinity);
    }

    totalRecordsChannelsTable = 0;
    pageSizeChannelsTable = 10;
    totalRecordsNotificationsTable = 0;
    pageSizeNotificationsTable = 10;
    isSMTPDisabled = false;
    notificationType = NotificationType;
    loading = true;

    notificationChannelsSubscription$: Subscription = new Subscription();
    userNotificationsSubscription$: Subscription = new Subscription();
    userNotificationChannelsSelectionSubscription$: Subscription = new Subscription();
    userNotificationsNodeUnselectDeactivateSubscription$: Subscription = new Subscription();
    userNotificationsNodeSelectDeactivateSubscription$: Subscription = new Subscription();

    constructor(
        private userNotificationChannelsClientService: UserNotificationChannelsClientService,
        private userNotificationsClientService: UserNotificationsClientService,
        private transLocoService: TranslocoService
    ) {}

    getCombinedNotificationChannels(event: LazyLoadEvent) {
        let page = 1;
        this.pageSizeChannelsTable = event && event.rows ? event.rows : 10;
        if (event && event.first && event.rows) {
            page = event.first / event.rows + 1;
        }
        const params: IUserNotificationChannelParams = {
            pageSize: this.pageSizeChannelsTable,
            pageIndex: page,
        };
        this.notificationChannelsSubscription$ = this.userNotificationChannelsClientService
            .getCombinedNotificationChannels(params)
            .subscribe((data: any) => {
                this.notificationChannels = data[0].data;
                this.selectedNotificationChannels = data[1].data;
                this.totalRecordsChannelsTable = data[0].totalItems;
                this.setNotificationChannels();
            });
    }

    setNotificationChannels() {
        this.notificationChannels.map((notificationChannel: IUserNotificationChannel) => {
            notificationChannel.activated = false;

            // SMTP programmatically controlled
            if (notificationChannel.name === NotificationType.SMTP) {
                // If no channels are selected
                if (this.selectedNotificationChannels.length === 0) {
                    // SMTP is selected and activated
                    this.isSMTPDisabled = true;
                    notificationChannel.activated = true;
                    // If SMTP is previously selected
                } else if (this.ifSMTPSelectedByTheUser()) {
                    // We should activate it
                    notificationChannel.activated = true;
                    // But if it is the only one selected channel we should disable it
                    if (this.selectedNotificationChannels.length === 1) {
                        this.isSMTPDisabled = true;
                        // Otherwise enable it
                    } else {
                        this.isSMTPDisabled = false;
                    }
                }
            } else if (this.selectedNotificationChannels.includes(notificationChannel.id)) {
                notificationChannel.activated = true;
            }
        });
    }

    toggleChannel(value: boolean, notificationChannel: IUserNotificationChannel): void {
        notificationChannel.activated = value;

        // Toggle on
        if (value) {
            this.selectedNotificationChannels.push(notificationChannel.id);
            this.controlSMTPWhenToggleOn(notificationChannel);
            // Toggle off
        } else {
            // Remove every switched off notification
            const index = this.selectedNotificationChannels.indexOf(notificationChannel.id);
            this.selectedNotificationChannels.splice(index, 1);

            this.controlSMTPWhenToggleOff();
        }
        this.userNotificationChannelsSelectionSubscription$ = this.userNotificationChannelsClientService
            .doSelection(this.selectedNotificationChannels)
            .subscribe();
    }

    enableAndSwitchOnSMTPChannel() {
        // Enable SMTP channel
        this.isSMTPDisabled = false;
        // Switch on SMTP channel
        this.notificationChannels.find((notificationChannel: IUserNotificationChannel) => {
            if (notificationChannel.name === NotificationType.SMTP) {
                notificationChannel.activated = true;
            }
        });
    }

    disableAndSwitchOffSMTPChannel() {
        // Disable SMTP channel
        this.isSMTPDisabled = false;
        // Switch off SMTP channel
        this.notificationChannels.find((notificationChannel: IUserNotificationChannel) => {
            if (notificationChannel.name === NotificationType.SMTP) {
                notificationChannel.activated = false;
            }
        });
    }

    ifSMTPSelectedByTheUser() {
        let output = false;
        this.notificationChannels.find((nC: IUserNotificationChannel) => {
            if (nC.name === NotificationType.SMTP) {
                output = this.selectedNotificationChannels.includes(nC.id);
            }
        });
        return output;
    }

    controlSMTPWhenToggleOn(notificationChannel: IUserNotificationChannel) {
        // On every other channel than SMTP is toggled on , we should control SMTP if it was disabled
        // If it is enabled user can control it by himself
        if (notificationChannel.name !== NotificationType.SMTP && this.isSMTPDisabled) {
            // If selected by the user before
            if (this.ifSMTPSelectedByTheUser()) {
                this.enableAndSwitchOnSMTPChannel();
                // If not selected by the user
            } else {
                this.disableAndSwitchOffSMTPChannel();
            }
        }
    }

    controlSMTPWhenToggleOff() {
        // If non of the channels is selected, switch smtp on
        if (this.selectedNotificationChannels.length === 0) {
            // Enable SMTP channel
            this.isSMTPDisabled = true;
            // Switch on SMTP channel
            this.notificationChannels.find((nC: IUserNotificationChannel) => {
                if (nC.name === NotificationType.SMTP) {
                    nC.activated = true;
                    return;
                }
            });
            // If only one channel is selected
        } else if (this.selectedNotificationChannels.length === 1) {
            this.notificationChannels.find((nC: IUserNotificationChannel) => {
                // We should check if it is SMTP
                if (
                    nC.name === NotificationType.SMTP &&
                    this.selectedNotificationChannels.includes(nC.id) &&
                    !this.isSMTPDisabled
                ) {
                    // And if the last one selected channel is SMTP, we should disable and activate it
                    this.isSMTPDisabled = true;
                    nC.activated = true;
                    return;
                }
            });
        }
    }

    getCombinedUserNotifications(event: LazyLoadEvent) {
        this.loading = true;
        this.pageSizeNotificationsTable = event && event.rows ? event.rows : 10;
        let page = 1;
        if (event && event.first && event.rows) {
            page = event.first / event.rows + 1;
        }
        this.userNotificationsSubscription$ = this.userNotificationsClientService
            .getCombinedUserNotifications({
                pageSize: this.pageSizeNotificationsTable,
                pageIndex: page,
            })
            .subscribe(data => {
                this.loading = false;
                this.systems = data[0].data.map((system: INotification) => {
                    system.events.forEach((event: INotificationEvent) => {
                        // The event is considered activated in two occasions
                        // 1. The event is mandatory
                        // 2. The event id is not present in deactivated event ids array.
                        event.activated = event.isMandatory || data[1].data.indexOf(event.id) == -1;
                    });
                    system.events.sort((e1: any, e2: any) => e1.isMandatory - e2.isMandatory);
                    system.disabled = system.events.every(e => e.isMandatory);
                    this.controlParentCheckbox(system);
                    return system;
                });
                this.totalRecordsNotificationsTable = data[0].totalItems;
            });
    }

    /*  There is a requirement based on mandatory notifications which causing
        clicking on tristate checkbox to work with two states in three scenarios:
           - If we have mandatory notifications, we should never marked parent checkbox as unchecked
           - If we don't have mandatory notifications, we should not marked parent checkbox as indeterminate
           -

        States: True->Checked; False->Indeterminate; Null->Unchecked;
        Rotations:
            No mandatory events:  Checked -> Unchecked -> Checked
            Some mandatory events:  Checked -> Indeterminate -> Checked
            Only mandatory events: Disabled, Checked only (we should not call this function at all)
    */
    toggleParentNode(data: any, system: INotification) {
        switch (data.value) {
            case true: // Checked
                this.activateNotifications(system);
                break;
            case false: // Indeterminate
                if (system.events.every(e => e.isMandatory === false)) {
                    // Override default rotation and set parent checkbox as unchecked
                    system.indeterminate = null;
                }
                this.deactivateNotifications(system);
                break;
            case null: // Unchecked
                // If there are mandatory notifications we override the default unchecking behavior and parent checkbox becomes checked.
                // This is required due to deselection of mandatory events is not allowed operation.
                if (system.events.some(e => e.isMandatory === true)) {
                    system.indeterminate = true;
                    this.activateNotifications(system);
                } else {
                    this.deactivateNotifications(system);
                }
                break;
        }

        this.userNotificationsNodeSelectDeactivateSubscription$ = this.userNotificationsClientService
            .deactivate(this.deactivatedNotifications)
            .subscribe();
    }

    toggleChildNode(data: any, event: INotificationEvent, system: INotification) {
        switch (data.checked) {
            case true:
                this.activateNotification(event);
                break;
            case false:
                this.deactivateNotification(event);
                break;
        }
        this.controlParentCheckbox(system);

        this.userNotificationsNodeUnselectDeactivateSubscription$ = this.userNotificationsClientService
            .deactivate(this.deactivatedNotifications)
            .subscribe();
    }

    controlParentCheckbox(system: INotification) {
        const allEventsActivated = system.events.every(e => e.activated === true);
        const someEventsActivated = system.events.some(e => e.activated === true);

        if (allEventsActivated) {
            // checked
            system.indeterminate = true;
        } else if (someEventsActivated && !allEventsActivated) {
            // indeterminate
            system.indeterminate = false;
        } else {
            // unchecked
            system.indeterminate = null;
        }
    }

    activateNotifications(system: INotification) {
        system.events.forEach((event: INotificationEvent) => {
            if (!event.isMandatory) {
                this.activateNotification(event);
            }
        });
    }

    deactivateNotifications(system: INotification) {
        system.events.forEach((event: INotificationEvent) => {
            if (!event.isMandatory) {
                this.deactivateNotification(event);
            }
        });
    }

    activateNotification(event: INotificationEvent) {
        if (event.isMandatory) {
            return;
        }
        event.activated = true;
    }

    deactivateNotification(event: INotificationEvent) {
        if (event.isMandatory) {
            return;
        }
        event.activated = false;
    }

    getSystemTranslation(system: INotification): string {
        const foundTranslation = system.translations.find(
            (evt: any) => evt.language === this.transLocoService.getActiveLang()
        );
        return foundTranslation ? foundTranslation.name : system.name;
    }

    getChannelNameTranslation(notificationChannel: IChannel): string {
        const foundTranslation = notificationChannel.translations.find(
            (evt: any) => evt.language === this.transLocoService.getActiveLang()
        );
        return foundTranslation ? foundTranslation.name : notificationChannel.name;
    }

    getChannelDescriptionTranslation(notificationChannel: IChannel): string {
        const foundTranslation = notificationChannel.translations.find(
            (evt: any) => evt.language === this.transLocoService.getActiveLang()
        );
        return foundTranslation ? foundTranslation.description : notificationChannel.description;
    }

    getEventTranslation(event: INotificationEvent): string {
        const foundTranslation = event.translations.find(
            (evt: any) => evt.language === this.transLocoService.getActiveLang()
        );
        return foundTranslation ? foundTranslation.description : event.code;
    }

    setDisabledRowClass(notificationChannel: IUserNotificationChannel): string {
        let output = '';
        if (!notificationChannel.activated || (notificationChannel && this.isSMTPDisabled)) {
            output = 'table-row-disabled';
        }
        return output;
    }

    ngOnDestroy() {
        this.notificationChannelsSubscription$.unsubscribe();
        this.userNotificationsSubscription$.unsubscribe();
        this.userNotificationChannelsSelectionSubscription$.unsubscribe();
        this.userNotificationsNodeUnselectDeactivateSubscription$.unsubscribe();
        this.userNotificationsNodeSelectDeactivateSubscription$.unsubscribe();
    }
}
