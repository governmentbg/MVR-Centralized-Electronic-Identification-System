import { Component, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { AdministratorDetails } from '@app/features/administrators/administrators.dto';
import { Subscription } from 'rxjs';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { ApplicationStatus } from '@app/features/administrators/enums/administrators.enum';

@Component({
    selector: 'app-circumstance-preview',
    templateUrl: './circumstance-preview.component.html',
    styleUrls: ['./circumstance-preview.component.scss'],
})
export class CircumstancePreviewComponent implements OnInit {
    constructor(
        private translateService: TranslocoService,
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
                    label: this.translateService.translate('modules.administrators.circumstances.txtTitle'),
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
        requestFunction: this.administratorService.fetchAdministratorCircumstanceDetails,
        onSuccess: data => {
            this.circumstanceDetails = data;
        },
        onInit: () => {
            this.loading = true;
        },
        onComplete: () => {
            this.loading = false;
        },
    });
    circumstanceDetails: AdministratorDetails | null = null;
    subscriptions = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    loading = false;
    roles: { name: string; value: string }[] = [];

    ngOnInit(): void {
        this.loading = true;
        const circumstanceDetailsId = this.route.snapshot.paramMap.get('id');
        if (!circumstanceDetailsId) {
            this.router.navigate(['/administrators']);
            return;
        }
        this.detailsQuery.execute(circumstanceDetailsId);
    }

    onClosePreview() {
        this.router.navigate(['/administrators']);
    }

    roleNames(values: string[]) {
        return this.roles.filter(role => values.includes(role.value)).map(role => role.name);
    }

    protected readonly ApplicationStatus = ApplicationStatus;
}
