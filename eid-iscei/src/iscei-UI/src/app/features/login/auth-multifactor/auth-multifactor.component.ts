import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslocoService } from '@ngneat/transloco';
import { first } from 'rxjs';
import { AuthenticationService } from 'src/app/authentication/authentication-service/authentication-service.service';
import { IUser } from 'src/app/core/interfaces/IUser';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
    selector: 'app-auth-multifactor',
    templateUrl: './auth-multifactor.component.html',
    styleUrls: ['./auth-multifactor.component.scss'],
})
export class AuthMultifactorComponent implements OnInit, OnDestroy {
    requestInProgress = false;
    user: IUser | null = null;
    originUrl = '';
    sessionId = '';
    ttl = 150;
    initialTtl = 150;
    displayValue = '';
    timerInterval: any;
    otp = '';
    remainingTries = 3;

    constructor(
        private route: ActivatedRoute,
        private authenticationService: AuthenticationService,
        private toastService: ToastService,
        public translateService: TranslocoService
    ) {}

    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.originUrl = params['origin'] || '';
            this.sessionId = params['sessionId'] || '';
            this.initialTtl = +params['ttl'] || 150;
            this.ttl = this.initialTtl;
            this.displayValue = this.formatTime(this.ttl);
            this.startTimer();
        });
    }

    startTimer(): void {
        if (this.timerInterval) clearInterval(this.timerInterval);

        this.timerInterval = setInterval(() => {
            if (this.ttl > 0) {
                this.ttl--;
                this.displayValue = this.formatTime(this.ttl);
            } else {
                clearInterval(this.timerInterval);
                this.notifyMainPageTimeout();
            }
        }, 1000);
    }

    formatTime(seconds: number): string {
        const m = Math.floor(seconds / 60)
            .toString()
            .padStart(2, '0');
        const s = (seconds % 60).toString().padStart(2, '0');
        return `${m}:${s}`;
    }

    notifyMainPageTimeout(): void {
        window.opener?.postMessage(
            {
                status: 'timeout',
            },
            this.originUrl
        );
        window.close();
    }

    ngOnDestroy(): void {
        if (this.timerInterval) clearInterval(this.timerInterval);
    }

    onOtpChange(value: string): void {
        if (value && value.length === 8 && /^\d{8}$/.test(value)) {
            this.requestInProgress = true;
            this.authenticationService
                .submitOtpCode(value, this.sessionId)
                .pipe(first())
                .subscribe({
                    next: (res: any) => {
                        window.opener.postMessage(
                            {
                                status: 'success',
                                access_token: res.access_token,
                                refresh_token: res.refresh_token,
                            },
                            this.originUrl
                        );
                        this.requestInProgress = false;
                        window.close();
                    },
                    error: err => {
                        this.remainingTries--;
                        this.otp = '';
                        if (this.remainingTries === 0) {
                            this.notifyMainPageTimeout();
                            return;
                        }
                        this.requestInProgress = false;
                        switch (err.status) {
                            case 400:
                                this.toastService.showErrorToast(
                                    this.translateService.translate('global.txtErrorTitle'),
                                    this.translateService.translate('global.txtOtpNotValidError')
                                );
                                break;
                            case 403:
                                this.toastService.showErrorToast(
                                    this.translateService.translate('global.txtErrorTitle'),
                                    this.translateService.translate('global.txtMaxAttemptsReachedError')
                                );
                                break;
                            case 404:
                                this.toastService.showErrorToast(
                                    this.translateService.translate('global.txtErrorTitle'),
                                    this.translateService.translate('global.txtSessionnotValidError')
                                );
                                break;
                            default:
                                this.toastService.showErrorToast(
                                    this.translateService.translate('global.txtErrorTitle'),
                                    this.translateService.translate('global.txtUnexpectedError')
                                );
                                break;
                        }
                    },
                });
        }
    }
}
