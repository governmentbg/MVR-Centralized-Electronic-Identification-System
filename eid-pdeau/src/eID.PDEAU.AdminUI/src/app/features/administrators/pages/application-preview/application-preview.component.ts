import { Component, OnInit } from '@angular/core';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { UserService } from '@app/core/services/user.service';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { ApplicationDetails } from '@app/features/administrators/administrators.dto';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { ApplicationType } from '@app/features/administrators/enums/administrators.enum';

@Component({
    selector: 'app-application-preview',
    templateUrl: './application-preview.component.html',
    styleUrls: ['./application-preview.component.scss'],
})
export class ApplicationPreviewComponent implements OnInit {
    constructor(
        private translateService: TranslocoService,
        private userService: UserService,
        private administratorService: AdministratorsService,
        private route: ActivatedRoute,
        private router: Router
    ) {
        const langChanges$ = this.translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.administrators.applications.title'),
                    onClick: () => this.router.navigate(['/administrators']),
                },
                {
                    label: this.translateService.translate('modules.administrators.administrator-preview.txtTitle'),
                },
            ];
            this.roles = [
                {
                    name: this.translateService.translate('enums.administratorsAndCentersRoles.administrator'),
                    value: 'ADMINISTRATOR',
                },
                {
                    name: this.translateService.translate('enums.administratorsAndCentersRoles.user'),
                    value: 'USER',
                },
            ];
        });
        this.subscriptions.add(langChanges$);
    }

    detailsQuery = new RequestHandler({
        requestFunction: this.administratorService.fetchApplicationDetails,
        onSuccess: data => {
            this.application = data;
        },
        onInit: () => {
            this.loading = true;
        },
        onComplete: () => {
            this.loading = false;
        },
    });
    application: ApplicationDetails | null = null;
    subscriptions = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    loading = false;
    roles: { name: string; value: string }[] = [];

    ngOnInit(): void {
        this.loading = true;
        const applicationId = this.route.snapshot.paramMap.get('id');
        if (!applicationId) {
            this.router.navigate(['/administrators']);
            return;
        }
        this.detailsQuery.execute(applicationId);
    }

    get applicantPersonalIdentifier(): string {
        return this.translateService.translate('modules.providers.register.' + this.userService.user.uidType);
    }

    onClosePreview() {
        this.router.navigate(['/administrators']);
    }

    roleNames(values: string[]) {
        return this.roles.filter(role => values.includes(role.value)).map(role => role.name);
    }

    getApplicationTypeText(type: string) {
        return this.translateService.translate(`modules.administrators.applications.types.${type}`);
    }
}
