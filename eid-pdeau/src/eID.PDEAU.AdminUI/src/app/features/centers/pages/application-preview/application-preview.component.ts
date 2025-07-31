import { Component, OnInit } from '@angular/core';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { UserService } from '@app/core/services/user.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CentersService } from '@app/features/centers/centers.service';
import { ApplicationDetails } from '@app/features/centers/centers.dto';

@Component({
    selector: 'app-application-preview',
    templateUrl: './application-preview.component.html',
    styleUrls: ['./application-preview.component.scss'],
})
export class ApplicationPreviewComponent implements OnInit {
    constructor(
        private translateService: TranslocoService,
        private userService: UserService,
        private centersService: CentersService,
        private route: ActivatedRoute,
        private router: Router
    ) {
        const langChanges$ = this.translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.centers.applications.title'),
                    onClick: () => this.router.navigate(['/centers']),
                },
                {
                    label: this.translateService.translate('modules.centers.center-preview.txtTitle'),
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
        requestFunction: this.centersService.fetchApplicationDetails,
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
            this.router.navigate(['/centers']);
            return;
        }
        this.detailsQuery.execute(applicationId);
    }

    get applicantPersonalIdentifier(): string {
        return this.translateService.translate('modules.centers.register.' + this.userService.user.uidType);
    }

    onClosePreview() {
        this.router.navigate(['/centers']);
    }

    roleNames(values: string[]) {
        return this.roles.filter(role => values.includes(role.value)).map(role => role.name);
    }

    getApplicationTypeText(type: string) {
        return this.translateService.translate(`modules.centers.applications.types.${type}`);
    }
}
