import { Component, OnDestroy } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { AppConfigService } from '../../../../core/services/config.service';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-system-processes-list',
    templateUrl: './system-processes-list.component.html',
    styleUrls: ['./system-processes-list.component.scss'],
})
export class SystemProcessesListComponent implements OnDestroy {
    constructor(private translocoService: TranslocoService, private appConfigService: AppConfigService) {
        this.languageChangeSubscription = translocoService.langChanges$.subscribe(() => {
            this.statusOfPrivateSystems = [
                {
                    address: this.appConfigService.config.systemProcesses?.prometheus,
                    description: this.translocoService.translate('modules.systemProcesses.descriptions.Prometheus'),
                },
                {
                    address: this.appConfigService.config.systemProcesses?.grafana,
                    description: this.translocoService.translate('modules.systemProcesses.descriptions.Grafana'),
                },
            ];
            this.configurationSystems = [
                {
                    address: this.appConfigService.config.systemProcesses?.['ddosSystem1'],
                    description: this.translocoService.translate(
                        'modules.systemProcesses.descriptions.DDOSProtectionServer1'
                    ),
                },
                {
                    address: this.appConfigService.config.systemProcesses?.['ddosSystem2'],
                    description: this.translocoService.translate(
                        'modules.systemProcesses.descriptions.DDOSProtectionServer2'
                    ),
                },
            ];
        });
    }

    languageChangeSubscription: Subscription;
    statusOfPrivateSystems: { address: string; description: string }[] = [];
    configurationSystems: { address: string; description: string }[] = [];

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }
}
