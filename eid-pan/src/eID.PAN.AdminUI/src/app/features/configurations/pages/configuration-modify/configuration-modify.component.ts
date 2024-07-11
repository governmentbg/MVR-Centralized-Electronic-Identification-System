import { Component, OnInit } from '@angular/core';
import { ConfigurationsClientService } from '../../services/configurations-client.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { IConfiguration, securityProtocol } from '../../interfaces/iconfigurations';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslocoService } from '@ngneat/transloco';

@Component({
    selector: 'app-configuration-modify',
    templateUrl: './configuration-modify.component.html',
    styleUrls: ['./configuration-modify.component.scss'],
    providers: [ConfirmationService, MessageService],
})
export class ConfigurationModifyComponent implements OnInit {
    constructor(
        private configurationsClientService: ConfigurationsClientService,
        private route: ActivatedRoute,
        private router: Router,
        private translocoService: TranslocoService,
        private confirmationService: ConfirmationService,
        private messageService: MessageService
    ) {}

    configurationId = this.route.snapshot.paramMap.get('id');
    action = this.configurationId ? 'edit' : 'add';
    form = new FormGroup({
        server: new FormControl<string | null>('', Validators.required),
        port: new FormControl<number | null>(null, Validators.required),
        securityProtocol: new FormControl<securityProtocol | null>(securityProtocol.SSL, Validators.required),
        userName: new FormControl<string | null>('', [Validators.required, Validators.maxLength(50)]),
        password: new FormControl<string | null>('', this.action === 'add' ? Validators.required : []),
    });
    configuration: IConfiguration | null = null;
    getConfigurationsSubscription: Subscription = new Subscription();
    loading!: boolean;
    securityProtocol = securityProtocol;
    requestInProgress = false;

    ngOnInit() {
        this.loadConfiguration();
    }

    loadConfiguration() {
        this.loading = true;
        if (this.configurationId) {
            this.getConfigurationsSubscription.add(
                this.configurationsClientService
                    .getConfigurationById(this.configurationId)
                    .subscribe((config: IConfiguration) => {
                        this.configuration = config;
                        this.form.setValue({
                            server: this.configuration.server,
                            port: this.configuration.port,
                            securityProtocol: this.configuration.securityProtocol,
                            userName: this.configuration.userName,
                            password: '',
                        });
                        this.loading = false;
                    })
            );
        }
    }

    onSubmit() {
        Object.keys(this.form.controls).forEach(key => {
            if (
                (this.form.get(key) && typeof this.form.get(key)?.value === 'string') ||
                this.form.get(key)?.value instanceof String
            ) {
                this.form.get(key)?.setValue(this.form.get(key)?.value.trim());
            }
        });
        this.form.markAllAsTouched();
        if (this.form.valid) {
            const updatedConfig: IConfiguration = {
                ...this.configuration,
                ...{
                    server: this.form.value.server || '',
                    port: this.form.value.port || 0,
                    securityProtocol: this.form.value.securityProtocol || securityProtocol.SSL,
                    userName: this.form.value.userName || '',
                    password: this.form.value.password || '',
                },
            };
            this.form.disable();
            this.requestInProgress = true;
            if (this.configuration && this.configuration.id) {
                this.configurationsClientService.updateConfiguration(this.configuration.id, updatedConfig).subscribe({
                    next: () => {
                        this.router.navigate(['/configurations']);
                    },
                    error: error => {
                        switch (error.status) {
                            case 400:
                                this.showErrorToast(this.translocoService.translate('global.txtInvalidDataError'));
                                break;
                            case 409:
                                this.showErrorToast(
                                    this.translocoService.translate('modules.configurations.txtErrorNameAlreadyExists')
                                );
                                break;
                            default:
                                this.showErrorToast(this.translocoService.translate('global.txtUnexpectedError'));
                                break;
                        }
                        this.form.enable();
                        this.requestInProgress = false;
                    },
                });
            } else {
                this.configurationsClientService.createConfiguration(updatedConfig).subscribe({
                    next: () => {
                        this.router.navigate(['/configurations']);
                    },
                    error: error => {
                        switch (error.status) {
                            case 400:
                                this.showErrorToast(this.translocoService.translate('global.txtInvalidDataError'));
                                break;
                            case 409:
                                this.showErrorToast(
                                    this.translocoService.translate('modules.configurations.txtErrorNameAlreadyExists')
                                );
                                break;
                            default:
                                this.showErrorToast(this.translocoService.translate('global.txtUnexpectedError'));
                                break;
                        }
                        this.form.enable();
                        this.requestInProgress = false;
                    },
                });
            }
        }
    }

    cancel() {
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger',
            message: this.translocoService.translate('global.txtUnsavedChangesConfirmation'),
            header: this.translocoService.translate('global.txtConfirmation'),
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.router.navigate(['/configurations']);
            },
        });
    }

    showErrorToast(message: string) {
        this.messageService.clear();
        this.messageService.add({
            key: 'toast',
            severity: 'error',
            summary: this.translocoService.translate('global.txtErrorTitle'),
            detail: message,
        });
    }
}
