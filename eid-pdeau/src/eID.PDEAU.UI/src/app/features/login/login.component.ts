import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslocoService } from '@ngneat/transloco';
import { AuthenticationService } from 'src/app/authentication/authentication-service/authentication-service.service';
import { IUser } from 'src/app/core/interfaces/IUser';
import { AuthService } from 'src/app/core/services/auth.service';
import { UserService } from 'src/app/core/services/user.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import jwt_decode from 'jwt-decode';
import { OAuthService } from 'angular-oauth2-oidc';
import { AppConfigService } from '@app/core/services/config.service';
@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
    form!: FormGroup;
    requestInProgress!: boolean;
    user: IUser | any = null;
    loginUrl = this.appConfigService.config.oauth.authUrl;
    homeUrl = '/';
    constructor(
        private fb: FormBuilder,
        private authenticationService: AuthenticationService,
        private authService: AuthService,
        private userService: UserService,
        private router: Router,
        private oauthService: OAuthService,
        private toastService: ToastService,
        private route: ActivatedRoute,
        private translocoService: TranslocoService,
        private appConfigService: AppConfigService
    ) {}
    ngOnInit(): void {
        this.form = this.fb.group({
            client_id: this.appConfigService.config.oauth.clientId,
            email: [null, Validators.required],
            password: [null, [Validators.required, Validators.maxLength(255)]],
        });
        const state = this.route.snapshot.queryParams['state'];
        if (this.oauthService.redirectUri === '/register') {
            this.oauthService.redirectUri = this.homeUrl;
        }
        if (state) {
            this.toastService.showSuccessToast(
                this.translocoService.translate('global.txtSuccessTitle'),
                this.translocoService.translate('global.txtLogoutSuccessMessage')
            );
            this.router.navigate([]);
        }
        if (this.oauthService.hasValidAccessToken()) {
            this.router.navigate([this.homeUrl]);
        }
        window.addEventListener('message', this.receiveMessage.bind(this), false);
    }

    markFormGroupTouched(formGroup: FormGroup) {
        Object.values(formGroup.controls).forEach(control => {
            if (control instanceof FormGroup) {
                this.markFormGroupTouched(control);
            } else {
                control.markAsTouched();
                control.markAsDirty();
            }
        });
    }

    redirectToLogin(type: 'employee' | 'citizen'): void {
        const origin = encodeURIComponent(window.location.href);
        const realm = encodeURIComponent(this.appConfigService.config.oauth.realm);
        const clientId = encodeURIComponent(this.appConfigService.config.oauth.clientId);
        const provider = encodeURIComponent(this.appConfigService.config.oauth.provider);
        const loginType = type === 'citizen' ? 'login' : 'administration-login';
        const loginUrl = `${this.loginUrl}/${loginType}?origin=${origin}&realm=${realm}&client_id=${clientId}&provider=${provider}`;
        window.open(loginUrl, '_blank', 'popup');
    }

    receiveMessage(event: MessageEvent): void {
        if (event.origin !== `${this.loginUrl}`) {
            return;
        }

        const { status, access_token, refresh_token } = event.data;

        if (status === 'success' && access_token && refresh_token) {
            this.authService.addTokenData(access_token, refresh_token);
            this.userService.setUser(jwt_decode(access_token));
            this.user = this.userService.getUser();
            if (!this.user) {
                this.toastService.showErrorToast(
                    this.translocoService.translate('global.txtErrorTitle'),
                    this.translocoService.translate('global.txtInvalidLoginDataError')
                );
                return;
            }
            this.oauthService.initCodeFlow();
            this.userService.userSubject.next(true);
            this.requestInProgress = false;
            this.userService.initSystemTypeForUser();

            const returnUrl =
                this.route.snapshot.queryParams['post_logout_redirect_uri'] ||
                this.oauthService.redirectUri ||
                this.homeUrl;
            this.router.navigate([returnUrl]);
        }
    }

    get usefulInformationUrl() {
        return this.appConfigService.config.uiFeatures.usefulInformationUrl;
    }
}
