import { Component, Input, OnInit } from '@angular/core';
import { ProviderService } from '@app/features/providers/provider.service';
import { RequestHandler } from '@app/shared/utils/request-handler';

@Component({
    selector: 'app-status-history',
    templateUrl: './status-history.component.html',
    styleUrls: ['./status-history.component.scss'],
})
export class StatusHistoryComponent implements OnInit {
    constructor(private providerService: ProviderService) {}

    ngOnInit(): void {
        this.statusHistoryQuery.execute(this.providerId);
    }

    @Input() providerId!: string;

    statusHistoryQuery = new RequestHandler({
        requestFunction: this.providerService.fetchProviderStatusHistory,
    });
}
