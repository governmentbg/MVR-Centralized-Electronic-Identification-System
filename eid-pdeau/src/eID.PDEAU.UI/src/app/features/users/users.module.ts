import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DividerModule } from 'primeng/divider';
import { TranslocoModule } from '@ngneat/transloco';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import { SharedModule } from '@app/shared/shared.module';
import { ChipModule } from 'primeng/chip';
import { ChipsModule } from 'primeng/chips';
import { ProvidersRoutingModule } from './users-routing.module';
import { TableModule } from 'primeng/table';
import { SkeletonModule } from 'primeng/skeleton';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { CardModule } from 'primeng/card';
import { InputNumberModule } from 'primeng/inputnumber';
import { MultiSelectModule } from 'primeng/multiselect';
import { UsersTableComponent } from './pages/users-table/users-table.component';
import { UsersFilterComponent } from './pages/users-table/components/users-filter/users-filter.component';
import { UserRegisterComponent } from './pages/user-register/user-register.component';
import { UserEditComponent } from './pages/user-edit/user-edit.component';
import { UserFormComponent } from './components/user-form/user-form.component';
import { UserPreviewComponent } from './pages/user-preview/user-preview.component';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@NgModule({
    declarations: [
        UsersTableComponent,
        UsersFilterComponent,
        UserRegisterComponent,
        UserEditComponent,
        UserFormComponent,
        UserPreviewComponent,
    ],
    imports: [
        ProvidersRoutingModule,
        TranslocoModule,
        DividerModule,
        ReactiveFormsModule,
        InputTextModule,
        ButtonModule,
        CommonModule,
        DropdownModule,
        FormsModule,
        SharedModule,
        TableModule,
        SkeletonModule,
        TranslocoLocaleModule,
        ChipModule,
        ChipsModule,
        CardModule,
        InputNumberModule,
        MultiSelectModule,
        ConfirmDialogModule,
    ],
    exports: [RouterModule, UserFormComponent],
})
export class UsersModule {}
