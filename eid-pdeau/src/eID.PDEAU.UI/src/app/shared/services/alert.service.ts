import { Injectable } from '@angular/core';
import { Message, MessageService } from 'primeng/api';

@Injectable({
    providedIn: 'root',
})
export class AlertService {
    constructor(private messageService: MessageService) {}

    private _messages: Message[] = [];

    get messages() {
        return this._messages;
    }

    set messages(data: Message[]) {
        this._messages = data;
    }

    addAlert(title: string, message: string, severity: string, id: string) {
        const messageIsAlreadyVisible = this.messages.find(message => message.id === id);
        if (!messageIsAlreadyVisible) {
            this.messages = [
                {
                    key: 'global-alert',
                    severity: severity,
                    summary: title,
                    detail: message,
                    id: id,
                },
            ];
        }
    }

    removeAlert(id: string) {
        const messageIsAlreadyVisible = this.messages.find(message => message.id === id);
        if (messageIsAlreadyVisible) {
            this.messages = this.messages.filter(message => message.id !== id);
        }
    }
}
