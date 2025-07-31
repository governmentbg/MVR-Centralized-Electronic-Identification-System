import { Component, ElementRef, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { UserService } from '@app/core/services/user.service';
import { DetailedProviderType, providerStatus } from '@app/features/providers/provider.dto';
import { ProviderService } from '@app/features/providers/provider.service';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { RoleType } from '@app/core/enums/auth.enum';

@Component({
    selector: 'app-provider-details',
    templateUrl: './provider-details.component.html',
    styleUrls: ['./provider-details.component.scss'],
})
export class ProviderDetailsComponent implements OnInit, OnDestroy, OnChanges {
    constructor(
        private translateService: TranslocoService,
        private userService: UserService,
        private providerService: ProviderService
    ) {}

    @Input() application: DetailedProviderType | null = null;
    @Input() isLoading = false;
    @ViewChild('helpSection') helpSection: ElementRef | undefined;

    ngOnInit(): void {
        const isEmployee = this.userService.hasRole(RoleType.EMPLOYEE);

        if (isEmployee || !this.application?.id) return;

        this.historyQuery.execute(this.application?.id);
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    ngOnChanges(changes: SimpleChanges): void {
        const application: DetailedProviderType | undefined = changes['application']?.currentValue;

        if (!application || !application.id) return;

        if (application.status === providerStatus.Values.ReturnedForCorrection) {
            this.historyQuery.execute(application.id);
        }
    }

    subscriptions = new Subscription();
    get administrator() {
        return this.application?.users.find(user => user.isAdministrator);
    }

    correctionComment = '';

    historyQuery = new RequestHandler({
        requestFunction: this.providerService.fetchProviderStatusHistory,
        onSuccess: data => {
            if (data[0].comment) {
                this.correctionComment = data[0].comment;
            }
        },
    });

    scrollToHelpSection() {
        this.helpSection?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
}
