import { ModuleWithProviders, NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SearchFormComponent } from './pages/search-form/search-form.component';
import { EidManagementRoutingModule } from './eid-management-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { DigitOnlyModule } from '@uiowa/digit-only';
import { ButtonModule } from 'primeng/button';
import { SearchResultsComponent } from './pages/search-results/search-results.component';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { ApplicationFormComponent } from './pages/application-form/application-form.component';
import { CalendarModule } from 'primeng/calendar';
import { SkeletonModule } from 'primeng/skeleton';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TableModule } from 'primeng/table';
import { SelectButtonModule } from 'primeng/selectbutton';
import { ApplicationsFilterComponent } from './components/applications-filter/applications-filter.component';
import { CertificatesFilterComponent } from './components/certificates-filter/certificates-filter.component';
import { ChipModule } from 'primeng/chip';
import { CheckboxModule } from 'primeng/checkbox';
import { ApplicationDetailsComponent } from './components/application-details/application-details.component';
import { CertificateDetailsComponent } from './components/certificate-details/certificate-details.component';
import { DialogModule } from 'primeng/dialog';
import { StepsComponent } from './components/steps/steps.component';
import { CertificateActionFormComponent } from './components/certificate-action-form/certificate-action-form.component';
import { CertificateHistoryComponent } from './components/certificate-history/certificate-history.component';
import { MultiSelectModule } from 'primeng/multiselect';
import { NomenclatureService } from './services/nomenclature.service';
import { GuardiansFormComponent } from './components/guardians-form/guardians-form.component';
import { StyleClassModule } from 'primeng/styleclass';
import { InputSwitchModule } from 'primeng/inputswitch';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { PinChangeFormComponent } from './components/pin-change-form/pin-change-form.component';
import { MessagesModule } from 'primeng/messages';
import { PanelModule } from 'primeng/panel';
import { TooltipModule } from 'primeng/tooltip';
import { ApplicationsListComponent } from './pages/applications-list/applications-list.component';

@NgModule({
    declarations: [
        SearchFormComponent,
        SearchResultsComponent,
        ApplicationFormComponent,
        ApplicationsFilterComponent,
        CertificatesFilterComponent,
        ApplicationDetailsComponent,
        CertificateDetailsComponent,
        StepsComponent,
        CertificateActionFormComponent,
        CertificateHistoryComponent,
        GuardiansFormComponent,
        PinChangeFormComponent,
        ApplicationsListComponent,
    ],
    imports: [
        CommonModule,
        EidManagementRoutingModule,
        SharedModule,
        DropdownModule,
        ReactiveFormsModule,
        InputTextModule,
        DigitOnlyModule,
        ButtonModule,
        CardModule,
        DividerModule,
        CalendarModule,
        SkeletonModule,
        ConfirmDialogModule,
        TableModule,
        SelectButtonModule,
        FormsModule,
        ChipModule,
        CheckboxModule,
        DialogModule,
        MultiSelectModule,
        StyleClassModule,
        InputSwitchModule,
        TranslocoLocaleModule,
        MessagesModule,
        PanelModule,
        TooltipModule,
    ],
})
export class EidManagementModule {
    constructor(@Optional() @SkipSelf() parentModule?: EidManagementModule) {
        if (parentModule) {
            throw new Error('EidManagementModule is already loaded. Import it in the AppModule only');
        }
    }
    static forRoot(): ModuleWithProviders<EidManagementModule> {
        return {
            ngModule: EidManagementModule,
            providers: [NomenclatureService],
        };
    }
}
