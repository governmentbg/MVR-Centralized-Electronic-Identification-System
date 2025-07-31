import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ToastService } from '@app/shared/services/toast.service';
import { UsersService } from '../../users.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { UserFormComponent } from '../../components/user-form/user-form.component';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-user-preview',
    templateUrl: './user-preview.component.html',
    styleUrls: ['./user-preview.component.scss'],
})
export class UserPreviewComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(
        private usersService: UsersService,
        private translateService: TranslocoService,
        private toastService: ToastService,
        private route: ActivatedRoute,
        private router: Router
    ) {}

    @Output() closeEdit = new EventEmitter();
    @ViewChild(UserFormComponent) userFormComponent!: UserFormComponent;

    ngOnInit() {
        const userId = this.route.snapshot.paramMap.get('id');
        if (!userId) {
            this.router.navigate(['/users']);
            return;
        }
        this.userQuery.execute(userId);

        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                this.breadcrumbItems = [
                    {
                        label: this.translateService.translate('modules.users.userPreview.users'),
                        onClick: () => {
                            this.router.navigate(['/users']);
                        },
                    },
                    {
                        label: userId,
                    },
                ];
            })
        );
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }
    breadcrumbItems: IBreadCrumbItems[] = [];

    userQuery = new RequestHandler({
        requestFunction: this.usersService.getUserById,

        onError: () => {
            this.toastService.showErrorToast('Error', 'error');
            this.router.navigate(['/users']);
        },
    });

    onClose() {
        this.router.navigate(['/users']);
    }
}
