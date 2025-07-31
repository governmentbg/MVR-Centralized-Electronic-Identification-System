import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FileType } from '@app/features/providers/provider.dto';
import { ProviderService } from '@app/features/providers/provider.service';
import { ToastService } from '@app/shared/services/toast.service';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { Table } from 'primeng/table';

@Component({
    selector: 'app-files-table',
    templateUrl: './files-table.component.html',
    styleUrls: ['./files-table.component.scss'],
})
export class FilesTableComponent {
    constructor(
        private providerService: ProviderService,
        private toastService: ToastService,
        private translateService: TranslocoService
    ) {}
    @ViewChild('usersTable') table!: Table;
    @ViewChild('filterInput') filterInput!: ElementRef<HTMLInputElement>;

    @Input() files: FileType[] = [];
    @Input() providerId: string | null = null;

    downloadFile = new RequestHandler({
        requestFunction: this.providerService.downloadFile,
        onSuccess: (blob, args) => {
            const [{ fileName }] = args;
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = fileName;
            a.click();
            window.URL.revokeObjectURL(url);
            a.remove();
        },
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.providers.provider-preview.errors.file-unavailable')
            );
        },
    });
}
