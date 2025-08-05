import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationSettingsComponent } from './pages/notification-settings.component';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputSwitchModule } from 'primeng/inputswitch';
import { TranslocoRootModule } from 'src/app/transloco-root.module';
import { CheckboxModule } from 'primeng/checkbox';
import { NotificationSettingsRoutingModule } from './notification-settings-routing';
import { FormsModule } from '@angular/forms';
import { TabViewModule } from 'primeng/tabview';
import { SkeletonModule } from 'primeng/skeleton';
import { TriStateCheckboxModule } from 'primeng/tristatecheckbox';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';

@NgModule({
    declarations: [NotificationSettingsComponent],
    imports: [
        CommonModule,
        NotificationSettingsRoutingModule,
        FormsModule,
        TableModule,
        ButtonModule,
        InputSwitchModule,
        TranslocoRootModule,
        CheckboxModule,
        TabViewModule,
        SkeletonModule,
        TriStateCheckboxModule,
        CardModule,
        DividerModule,
    ],
})
export class NotificationSettingsModule {}
