import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ConfigurationsClientService } from '../../services/configurations-client.service';
import { IConfiguration, IConfigurationsPaginatedData, securityProtocol } from '../../interfaces/iconfigurations';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService, LazyLoadEvent } from 'primeng/api';
import { Router } from '@angular/router';
import { Table } from 'primeng/table/table';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-configurations-list',
    templateUrl: './configurations-list.component.html',
    styleUrls: ['./configurations-list.component.scss'],
    providers: [ConfirmationService],
})
export class ConfigurationsListComponent implements OnInit, OnDestroy {
    constructor(
        private configurationsClientService: ConfigurationsClientService,
        private translocoService: TranslocoService,
        private confirmationService: ConfirmationService,
        private router: Router
    ) {}

    @ViewChild('dt') table!: Table;
    configurations: IConfiguration[] = [];
    pendingChanges = false;
    totalRecords = 0;
    pageSize = 10;
    getConfigurationsSubscription: Subscription = new Subscription();
    loading!: boolean;

    ngOnInit() {
        this.loading = true;
    }

    ngOnDestroy() {
        this.getConfigurationsSubscription.unsubscribe();
    }

    loadConfigurations(event: LazyLoadEvent): void {
        this.loading = true;
        this.configurations = [];
        this.totalRecords = 0;
        let page = 1;
        if (event.first && event.rows) {
            page = event.first / event.rows + 1;
        }

        this.getConfigurationsSubscription.add(
            this.configurationsClientService
                .getConfigurations({
                    pageSize: event.rows || this.pageSize,
                    pageIndex: page,
                })
                .subscribe((response: IConfigurationsPaginatedData) => {
                    this.configurations = response.data;
                    this.totalRecords = response.totalItems;
                    this.loading = false;
                })
        );
    }

    onRowEditInit(configuration: IConfiguration): void {
        this.router.navigate([`/configurations/edit/${configuration.id}`]);
    }

    onRowDelete(configuration: IConfiguration): void {
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger',
            message: this.translocoService.translate('modules.configurations.txtAreYouSureYouWantToDelete'),
            header: this.translocoService.translate('global.txtConfirmation'),
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                if (configuration.id) {
                    this.configurationsClientService.deleteConfiguration(configuration.id).subscribe(() => {
                        this.table.reset();
                    });
                }
            },
        });
    }

    addConfiguration(): void {
        this.router.navigate([`/configurations/add`]);
    }

    securityProtocolText(encryption: securityProtocol): string {
        switch (encryption) {
            case securityProtocol.TLS:
                return 'TLS';
            case securityProtocol.SSL:
                return 'SSL';
            default:
                return 'N/A';
        }
    }
}
