import { Component, Input } from '@angular/core';
import { ToastService } from '@app/shared/services/toast.service';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { AttachmentFull } from '@app/features/administrators/administrators.dto';
import { AdministratorsService } from '@app/features/administrators/administrators.service';

@Component({
    selector: 'app-files-table',
    templateUrl: './files-table.component.html',
    styleUrls: ['./files-table.component.scss'],
})
export class FilesTableComponent {
    constructor(
        private toastService: ToastService,
        private translateService: TranslocoService,
        private administratorService: AdministratorsService
    ) {}

    @Input() files: AttachmentFull[] = [];

    downloadFile = new RequestHandler({
        requestFunction: this.administratorService.downloadFile,
        onSuccess: (attachment, args) => {
            const fileName = args[0];
            const url = `data:application/pdf;base64,${attachment.content}`;
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
