import { Component, OnDestroy } from '@angular/core';
import { AppConfigService } from 'src/app/core/services/config.service';
import { UsefulInformationService } from 'src/app/core/services/useful-information.service';
import { VERSION } from 'src/assets/js/version';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-footer',
    templateUrl: './footer.component.html',
    styleUrls: ['./footer.component.scss'],
})
export class FooterComponent implements OnDestroy {
    constructor(
        private appConfigService: AppConfigService,
        private usefulInformationService: UsefulInformationService,
        public translateService: TranslocoService
    ) {
        this.subscription.add(
            translateService.langChanges$.subscribe(() => {
                this.popopUrl = `${
                    this.appConfigService.config.externalLinks.popopUrl
                }?Lang=${this.translateService.getActiveLang()}`;
            })
        );
    }
    private subscription: Subscription = new Subscription();
    paymentUrl = this.appConfigService.config.externalLinks.paymentUrl;
    eDeliveryUrl = this.appConfigService.config.externalLinks.eDeliveryUrl;
    providersUrl = this.appConfigService.config.externalLinks.providersUrl;
    popopUrl!: string;
    currentYear = new Date().getFullYear();
    version = VERSION;

    resetView() {
        this.usefulInformationService.informationTabSubject.next(false);
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }
}
