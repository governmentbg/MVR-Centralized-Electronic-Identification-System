import { Component, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ActivatedRoute, Router } from '@angular/router';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { Subscription } from 'rxjs';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { ApplicationStatus } from '@app/features/administrators/enums/administrators.enum';
import { CentersService } from '@app/features/centers/centers.service';
import { CenterDetails } from '@app/features/centers/centers.dto';

@Component({
    selector: 'app-circumstance-preview',
    templateUrl: './circumstance-preview.component.html',
    styleUrls: ['./circumstance-preview.component.scss'],
})
export class CircumstancePreviewComponent implements OnInit {
    constructor(
        private translateService: TranslocoService,
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
                    label: this.translateService.translate('modules.centers.circumstances.txtTitle'),
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
        requestFunction: this.centersService.fetchCenterCircumstanceDetails,
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
    circumstanceDetails: CenterDetails | null = null;
    subscriptions = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    loading = false;
    roles: { name: string; value: string }[] = [];

    ngOnInit(): void {
        this.loading = true;
        const circumstanceDetailsId = this.route.snapshot.paramMap.get('id');
        if (!circumstanceDetailsId) {
            this.router.navigate(['/centers']);
            return;
        }
        this.detailsQuery.execute(circumstanceDetailsId);
    }

    onClosePreview() {
        this.router.navigate(['/centers']);
    }

    roleNames(values: string[]) {
        return this.roles.filter(role => values.includes(role.value)).map(role => role.name);
    }

    protected readonly ApplicationStatus = ApplicationStatus;
}
