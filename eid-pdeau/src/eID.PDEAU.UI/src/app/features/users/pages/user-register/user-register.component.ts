import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { IdentifierType, UserRolesType } from '@app/shared/enums';
import { EGNAdultValidator, EGNValidator, PinValidator } from '@app/shared/validators/uid-validator';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ToastService } from '@app/shared/services/toast.service';
import { UsersService } from '../../users.service';
import { CreateUserType } from '../../users.dto';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { Router } from '@angular/router';
import { UserFormComponent } from '../../components/user-form/user-form.component';

@Component({
    selector: 'app-user-register',
    templateUrl: './user-register.component.html',
    styleUrls: ['./user-register.component.scss'],
})
export class UserRegisterComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(
        private usersService: UsersService,
        private translateService: TranslocoService,
        private toastService: ToastService,
        private router: Router
    ) {}
    @ViewChild(UserFormComponent) userFormComponent!: UserFormComponent;
    ngOnInit(): void {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                this.breadcrumbItems = [
                    {
                        label: this.translateService.translate('modules.users.userRegister.users'),
                        onClick: () => this.router.navigate(['/users']),
                    },
                    {
                        label: this.translateService.translate('modules.users.userRegister.newUser'),
                    },
                ];
            })
        );
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }
    breadcrumbItems: IBreadCrumbItems[] = [];

    registerUserMutation = new RequestHandler({
        requestFunction: this.usersService.registerUser,
        onInit: () => {
            this.userFormComponent.form.disable();
        },
        onSuccess: () => {
            this.router.navigate(['/users']);
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.users.userRegister.toasts.successfulRegistration')
            );
        },
        onError: errorResponse => {
            this.userFormComponent.form.enable();
            // Default error message
            let errorMessage = this.translateService.translate('modules.users.userRegister.toasts.failedRegistration');
            switch (errorResponse.status) {
                case 400: {
                    errorMessage = this.translateService.translate('global.txtInvalidDataError');
                    break;
                }
                case 409: {
                    const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;
                    if (firstKey === 'Uid') {
                        errorMessage = this.translateService.translate('modules.users.userRegister.toasts.Uid');
                    }
                }
            }
            this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), errorMessage);
        },
    });

    onSubmit(userData: CreateUserType) {
        this.registerUserMutation.execute(userData);
    }

    onCancel() {
        this.router.navigate(['/users']);
    }
}
