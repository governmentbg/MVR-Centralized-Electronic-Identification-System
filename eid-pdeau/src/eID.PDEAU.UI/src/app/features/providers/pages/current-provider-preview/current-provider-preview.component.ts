import { Component } from '@angular/core';
import { CurrentProviderService } from '@app/core/services/current-provider.service';
import { providerStatus } from '@app/features/providers/provider.dto';

@Component({
    selector: 'app-current-provider-preview',
    templateUrl: './current-provider-preview.component.html',
    styleUrls: ['./current-provider-preview.component.scss'],
})
export class CurrentProviderPreviewComponent {
    constructor(private currentProviderService: CurrentProviderService) {}

    get providerData() {
        return this.currentProviderService.getProvider();
    }

    get providerFiles() {
        return this.currentProviderService.getProvider()?.files;
    }

    get providerIsLoading() {
        return this.currentProviderService.providerIsLoading;
    }

    protected readonly providerStatus = providerStatus;
}
