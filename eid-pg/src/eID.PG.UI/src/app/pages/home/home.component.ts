import { Component } from '@angular/core';
import { AppConfigService } from 'src/app/core/services/config.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
})
export class HomeComponent {
    constructor(private appConfigService: AppConfigService) {}
    providersUrl = this.appConfigService.config.externalLinks.providersUrl;

    redirectToProviders(): void {
        window.open(this.providersUrl, '_blank');
    }
}
