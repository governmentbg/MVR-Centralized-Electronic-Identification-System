import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { User } from './interfaces/user';
import { finalize, first, Subscription } from 'rxjs';
import { ProfileService } from './services/profile.service';
import { MessageService } from 'primeng/api';
import { TranslocoService } from '@ngneat/transloco';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
import { AuthenticationService } from 'src/app/authentication/authentication-service/authentication-service.service';
import { OAuthService } from 'angular-oauth2-oidc';
import { UserService } from 'src/app/core/services/user.service';
import { AuthService } from 'src/app/core/services/auth.service';
import jwt_decode from 'jwt-decode';
import { AppConfigService } from 'src/app/core/services/config.service';

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
    user!: User;
    open = false;
    openDialog = false;
    requestInProgress!: boolean;
    multifactorOptions!: any;
    is2FaEnabled = new FormControl<boolean>(false);
    constructor(
        private profileService: ProfileService,
        private messageService: MessageService,
        private translocoService: TranslocoService,
        private authenticationService: AuthenticationService,
        private userService: UserService,
        private oauthService: OAuthService,
        private authService: AuthService,
        private appConfigService: AppConfigService
    ) {}
    ngOnInit(): void {
        this.getUserData();
    }

    attachEidentity() {
        this.openDialog = true;
    }

    getUserData() {
        this.requestInProgress = true;
        this.profileService
            .getUser()
            .pipe(first())
            .subscribe({
                next: (user: User) => {
                    this.user = user;
                    this.is2FaEnabled.setValue(user.is2FaEnabled);
                    this.requestInProgress = false;
                },
                error: err => {
                    let showDefaultError = true;
                    err.errors?.forEach((el: string) => {
                        if (errorDataStorage.has(el)) {
                            this.messageService.add({
                                severity: 'error',
                                summary: this.translocoService.translate('global.txtErrorTitle'),
                                detail: this.translocoService.translate('errors.' + formatError(el)),
                            });
                            // if backend has an error/s, turn off the default message
                            showDefaultError = false;
                        }
                    });
                    // if not error was recognized by the storage, throw default message once
                    if (showDefaultError) {
                        this.messageService.add({
                            severity: 'error',
                            summary: this.translocoService.translate('global.txtErrorTitle'),
                            detail: this.translocoService.translate('global.txtUnexpectedError'),
                        });
                    }
                    this.requestInProgress = false;
                },
            });
    }

    handleDialogClose(data: any) {
        if (data.associated) {
            this.requestInProgress = true;
            this.authenticationService
                .refreshToken(this.appConfigService.config.oauth.clientId)
                .pipe(finalize(() => (this.requestInProgress = false)))
                .subscribe({
                    next: async res => {
                        this.authService.addTokenData(res.access_token, res.refresh_token);
                        this.userService.setUser(jwt_decode(res.access_token));
                        const user = this.userService.getUser();
                        sessionStorage.setItem('user', JSON.stringify(user));
                        this.oauthService.initCodeFlow();
                        this.userService.userSubject.next(true);
                        this.getUserData();
                        this.openDialog = false;
                        this.messageService.add({
                            severity: 'success',
                            summary: this.translocoService.translate('global.txtSuccessTitle'),
                            detail: this.translocoService.translate('profilePage.messages.txtSuccessAttachEid'),
                        });
                    },
                    error: error => {
                        let showDefaultError = true;
                        error.errors?.forEach((el: string) => {
                            if (errorDataStorage.has(el)) {
                                this.messageService.add({
                                    severity: 'error',
                                    summary: this.translocoService.translate('global.txtErrorTitle'),
                                    detail: this.translocoService.translate('errors.' + formatError(el)),
                                });
                                // if backend has an error/s, turn off the default message
                                showDefaultError = false;
                            }
                        });
                        // if not error was recognized by the storage, throw default message once
                        if (showDefaultError) {
                            this.messageService.add({
                                severity: 'error',
                                summary: this.translocoService.translate('global.txtErrorTitle'),
                                detail: this.translocoService.translate('global.txtUnexpectedError'),
                            });
                        }
                        this.requestInProgress = false;
                    },
                });
        }
        this.openDialog = false;
    }

    change2faOption(event: any): void {
        this.requestInProgress = true;
        const updatedData = { ...this.user, is2FaEnabled: this.is2FaEnabled.value };
        const formValue = this.convertEmptyStringsToNull(updatedData);
        this.profileService.updatePersonalInfo(formValue).subscribe({
            next: () => {
                this.messageService.add({
                    severity: 'success',
                    summary: this.translocoService.translate('global.txtSuccessTitle'),
                    detail: event.checked
                        ? this.translocoService.translate('updatePersonalInfo.txt2faEnableSuccess')
                        : this.translocoService.translate('updatePersonalInfo.txt2faDisableSuccess'),
                });

                this.requestInProgress = false;
            },
            error: error => {
                let showDefaultError = true;
                error.errors.forEach((el: string) => {
                    if (errorDataStorage.has(el)) {
                        this.messageService.add({
                            severity: 'error',
                            summary: this.translocoService.translate('global.txtErrorTitle'),
                            detail: this.translocoService.translate('errors.' + formatError(el)),
                        });
                        showDefaultError = false;
                    }
                });
                if (showDefaultError) {
                    this.messageService.add({
                        severity: 'error',
                        summary: this.translocoService.translate('global.txtErrorTitle'),
                        detail: this.translocoService.translate('global.txtUnexpectedError'),
                    });
                }
                this.requestInProgress = false;
            },
        });
    }

    convertEmptyStringsToNull(obj: any): any {
        const result: any = {};
        for (const key in obj) {
            if (Object.prototype.hasOwnProperty.call(obj, key)) {
                result[key] = obj[key] === '' ? null : obj[key];
            }
        }
        return result;
    }
}
