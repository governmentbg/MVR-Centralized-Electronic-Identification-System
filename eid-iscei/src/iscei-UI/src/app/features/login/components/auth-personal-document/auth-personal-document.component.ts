import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslocoService } from '@ngneat/transloco';
import { AuthenticationService } from 'src/app/authentication/authentication-service/authentication-service.service';
import { IUser } from 'src/app/core/interfaces/IUser';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
    selector: 'app-auth-personal-document',
    templateUrl: './auth-personal-document.component.html',
    styleUrls: ['./auth-personal-document.component.scss'],
})
export class AuthPersonalDocumentComponent implements OnInit {
    user: IUser | any = null;
    requestInProgress!: boolean;
    code!: string;
    originUrl = '';
    clientId = '';
    scope = '';
    system_id = '';
    constructor(
        private authenticationService: AuthenticationService,
        private translocoService: TranslocoService,
        private route: ActivatedRoute,
        private toastService: ToastService
    ) {}

    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.code = params['code'];
            this.originUrl = params['origin'];
            this.clientId = params['clientId'];
            this.scope = params['scope'];
            this.system_id = params['system_id'];
            if (this.code) {
                this.authenticationToken(this.code, this.clientId);
            } else {
                this.toastService.showErrorToast(
                    this.translocoService.translate('global.txtErrorTitle'),
                    this.translocoService.translate('global.txtErrorMissingToken')
                );
            }
        });
    }

    authenticationToken(code: string, clientId: string) {
        this.authenticationService.getCodeFlowToken(code, clientId).subscribe({
            next: res => {
                window.opener.postMessage(
                    { status: 'success', access_token: res.access_token, refresh_token: res.refresh_token },
                    this.originUrl
                );
                window.close();
            },
            error: error => {
                let showDefaultError = true;
                error.errors?.forEach((el: string) => {
                    if (errorDataStorage.has(el)) {
                        this.toastService.showErrorToast(
                            this.translocoService.translate('global.txtErrorTitle'),
                            this.translocoService.translate('errors.' + formatError(el))
                        );
                        showDefaultError = false;
                    }
                });
                if (showDefaultError) {
                    this.toastService.showErrorToast(
                        this.translocoService.translate('global.txtErrorTitle'),
                        this.translocoService.translate('global.txtUnexpectedError')
                    );
                }
            },
        });
    }
}
