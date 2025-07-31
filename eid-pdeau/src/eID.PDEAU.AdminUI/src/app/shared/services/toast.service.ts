import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({
    providedIn: 'root',
})
export class ToastService {
    constructor(private messageService: MessageService) {}

    showSuccessToast(title: string, message: string) {
        this.messageService.clear();
        this.messageService.add({
            key: 'global-toast',
            severity: 'success',
            summary: title,
            detail: message,
        });
    }

    showErrorToast(title: string, message: string) {
        this.messageService.clear();
        this.messageService.add({
            key: 'global-toast',
            severity: 'error',
            summary: title,
            detail: message,
        });
    }
}
