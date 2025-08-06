import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LandingPagesService } from '../services/landing-pages.service';
import { TranslocoService } from '@ngneat/transloco';
import { ToastService } from 'src/app/shared/services/toast.service';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';

@Component({
    selector: 'app-update-email-landing-page',
    templateUrl: './update-email-landing-page.component.html',
    styleUrls: ['./update-email-landing-page.component.scss'],
})
export class UpdateEmailLandingPageComponent implements OnInit {
    constructor(
        private route: ActivatedRoute,
        private landingPageService: LandingPagesService,
        private translocoService: TranslocoService,
        private toastService: ToastService,
        private router: Router
    ) {}

    message!: string;

    ngOnInit() {
        this.route.queryParams.subscribe(params => {
            const token = params['token'];
            if (token) {
                this.landingPageService.updateEmail(token).subscribe({
                    next: () => {
                        this.message = this.translocoService.translate(
                            'updateEmail.landing-page.txtEmailUpdateSuccess'
                        );
                    },
                    error: error => {
                        this.message = this.translocoService.translate('updateEmail.landing-page.txtMessageError');
                        let showDefaultError = true;
                        error.errors.forEach((el: string) => {
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
            } else {
                this.router.navigate(['/home']);
                this.toastService.showErrorToast(
                    this.translocoService.translate('global.txtErrorTitle'),
                    this.translocoService.translate('global.txtErrorMissingToken')
                );
            }
        });
    }
}
