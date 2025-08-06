import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileComponent } from './profile.component';
import { RouterModule } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ChipModule } from 'primeng/chip';
import { ReactiveFormsModule } from '@angular/forms';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CardModule } from 'primeng/card';
import { UpdateEmailComponent } from './components/update-email/update-email.component';
import { UpdatePasswordComponent } from './components/update-password/update-password.component';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { SharedModule } from '../../shared/shared.module';
import { DividerModule } from 'primeng/divider';
import { InplaceModule } from 'primeng/inplace';
import { InputMaskModule } from 'primeng/inputmask';
import { KeyFilterModule } from 'primeng/keyfilter';
import { UpdatePersonalInfoComponent } from './components/update-personal-info/update-personal-info.component';
import { ProfileRoutingModule } from './profile-routing.module';
import { DialogModule } from 'primeng/dialog';
import { AssociateProfilesAuthComponent } from './components/associate-profiles-auth/associate-profiles-auth.component';
import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { PasswordModule } from 'primeng/password';
import { InputSwitchModule } from 'primeng/inputswitch';

@NgModule({
    declarations: [
        ProfileComponent,
        UpdateEmailComponent,
        UpdatePasswordComponent,
        UpdatePersonalInfoComponent,
        AssociateProfilesAuthComponent,
    ],
    exports: [RouterModule],
    imports: [
        CommonModule,
        TranslocoLocaleModule,
        TranslocoModule,
        ChipModule,
        ReactiveFormsModule,
        ToastModule,
        InputTextModule,
        ButtonModule,
        InputNumberModule,
        CardModule,
        ConfirmDialogModule,
        SharedModule,
        DividerModule,
        InplaceModule,
        InputMaskModule,
        KeyFilterModule,
        ProfileRoutingModule,
        DialogModule,
        DropdownModule,
        CheckboxModule,
        PasswordModule,
        InputSwitchModule,
    ],
})
export class ProfileModule {}
