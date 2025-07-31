import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { LayoutComponent } from './components/layout/layout.component';
import { InputComponent } from './components/input/input.component';
import { ShellComponent } from './components/shell/shell.component';
import { TranslocoRootModule } from '../transloco-root.module';
import { LanguageSelectorComponent } from './components/language-selector/language-selector.component';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ErrorFieldComponent } from './components/error-field/error-field.components';
import { ToastModule } from 'primeng/toast';
import { FooterComponent } from './components/footer/footer.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { AppDigitOnlyDirective } from './directives/digit-only';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';
import { PreviewFieldComponent } from './components/preview-field/preview-field.component';
import { SkeletonModule } from 'primeng/skeleton';
import { AppCoordinatesDirective } from './directives/coordinates';
import { PageUnauthorizedComponent } from '@app/shared/components/page-unauthorized/page-unauthorized.component';
import { HasRoleDirective } from '@app/shared/directives/has-role.directive';
import { MessagesModule } from 'primeng/messages';
import { NotesComponent } from '@app/shared/components/notes/notes.component';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { TagModule } from 'primeng/tag';
import { PersonalIdentifierDirective } from './directives/personal-identifier.directive';
import { PrefixInputDirective } from '@app/shared/directives/phone-prefix';

@NgModule({
    declarations: [
        PageNotFoundComponent,
        LayoutComponent,
        FooterComponent,
        NavbarComponent,
        SidebarComponent,
        InputComponent,
        ShellComponent,
        LanguageSelectorComponent,
        ErrorFieldComponent,
        AppDigitOnlyDirective,
        AppCoordinatesDirective,
        BreadcrumbComponent,
        PreviewFieldComponent,
        PageUnauthorizedComponent,
        HasRoleDirective,
        NotesComponent,
        PersonalIdentifierDirective,
        PrefixInputDirective,
    ],
    imports: [
        CommonModule,
        SkeletonModule,
        RouterModule,
        TranslocoRootModule,
        FormsModule,
        ToastModule,
        MessagesModule,
        TranslocoLocaleModule,
        TagModule,
    ],
    exports: [
        NavbarComponent,
        PageNotFoundComponent,
        InputComponent,
        TranslocoRootModule,
        ErrorFieldComponent,
        AppDigitOnlyDirective,
        AppCoordinatesDirective,
        BreadcrumbComponent,
        PreviewFieldComponent,
        SkeletonModule,
        HasRoleDirective,
        NotesComponent,
        PersonalIdentifierDirective,
        PrefixInputDirective,
    ],
    providers: [MessageService],
})
export class SharedModule {}
