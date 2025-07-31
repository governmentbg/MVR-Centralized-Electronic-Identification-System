import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ToastService } from '@app/shared/services/toast.service';
import { UsersService } from '../../users.service';
import { CreateUserType, GetUserType } from '../../users.dto';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { UserFormComponent } from '../../components/user-form/user-form.component';

@Component({
    selector: 'app-user-edit',
    templateUrl: './user-edit.component.html',
    styleUrls: ['./user-edit.component.scss'],
})
export class UserEditComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(
        private usersService: UsersService,
        private translateService: TranslocoService,
        private toastService: ToastService
    ) {}

    @Input() user: GetUserType | null = null;
    @Output() closeEdit = new EventEmitter();
    @ViewChild(UserFormComponent) userFormComponent!: UserFormComponent;

    ngOnInit(): void {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                this.breadcrumbItems = [
                    {
                        label: this.translateService.translate('modules.users.userRegister.users'),
                        onClick: () => this.closeEdit.emit(),
                    },
                    {
                        label: `${this.translateService.translate('modules.users.userRegister.editUser')} - ${
                            this.user?.id
                        }`,
                    },
                ];
            })
        );
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }
    breadcrumbItems: IBreadCrumbItems[] = [];

    editUserMutation = new RequestHandler({
        requestFunction: this.usersService.editUser,
        onInit: () => {
            this.userFormComponent.form.disable();
        },
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.users.userEdit.toasts.successfulEdit')
            );
            this.closeEdit.emit();
        },
        onError: errorResponse => {
            this.userFormComponent.form.enable();
            let errorMessage = this.translateService.translate('modules.users.userEdit.toasts.failedEdit');
            switch (errorResponse.status) {
                case 400: {
                    errorMessage = this.translateService.translate('global.txtInvalidDataError');
                    break;
                }
                case 409: {
                    const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;
                    if (firstKey === 'Id') {
                        errorMessage = this.translateService.translate('modules.users.userEdit.toasts.Id');
                    }
                }
            }
            this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), errorMessage);
        },
        onComplete: () => {
            this.userFormComponent.form.enable();
        },
    });

    identifierTypes: { name: string; id: string }[] = [];

    roles: { name: string; id: string }[] = [];

    onSubmit(id: string | undefined, userData: CreateUserType) {
        this.editUserMutation.execute(id, userData);
    }

    onCancel() {
        this.closeEdit.emit();
    }
}
