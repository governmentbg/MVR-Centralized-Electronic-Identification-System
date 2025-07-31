import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmpowermentCheckRoutingModule } from './empowerment-check-routing.module';
import { TranslocoModule } from '@ngneat/transloco';
import { ReactiveFormsModule } from '@angular/forms';
import { EmpowermentCheckBreadcrumbComponent } from './components/empowerment-check-breadcrumb/empowerment-check-breadcrumb.component';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { EmpowermentCheckFooterComponent } from './components/empowerment-check-footer/empowerment-check-footer.component';
import { EmpowermentCheckLanguageSelectorComponent } from './components/empowerment-check-language-selector/empowerment-check-language-selector.component';
import { EmpowermentCheckSidebarComponent } from './components/empowerment-check-sidebar/empowerment-check-sidebar.component';
import { EmpowermentCheckSearchComponent } from './pages/empowerment-check-search/empowerment-check-search.component';
import { EmpowermentCheckSearchResultDetailsComponent } from './pages/empowerment-check-search-result-details/empowerment-check-search-result-details.component';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { CalendarModule } from 'primeng/calendar';
import { SkeletonModule } from 'primeng/skeleton';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { EmpowermentCheckSearchResultsComponent } from './pages/empowerment-check-search-results/empowerment-check-search-results.component';
import { DigitOnlyModule } from '@uiowa/digit-only';
import { TooltipModule } from 'primeng/tooltip';
import { DividerModule } from 'primeng/divider';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@NgModule({
    declarations: [
        EmpowermentCheckFooterComponent,
        EmpowermentCheckBreadcrumbComponent,
        EmpowermentCheckLanguageSelectorComponent,
        EmpowermentCheckSidebarComponent,
        EmpowermentCheckSearchComponent,
        EmpowermentCheckSearchResultDetailsComponent,
        EmpowermentCheckSearchResultsComponent,
    ],
    imports: [
        CommonModule,
        EmpowermentCheckRoutingModule,
        TranslocoModule,
        ReactiveFormsModule,
        ButtonModule,
        DropdownModule,
        InputTextModule,
        MultiSelectModule,
        CalendarModule,
        SkeletonModule,
        TableModule,
        CardModule,
        DigitOnlyModule,
        TooltipModule,
        DividerModule,
        TranslocoLocaleModule,
        ToastModule,
        DialogModule,
        ConfirmDialogModule,
    ],
    providers: [],
})
export class EmpowermentCheckModule {}
