import { Component, Input, OnDestroy } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ApplicationForm, documentTypeFormGroup } from '@app/features/administrators/administrators.dto';
import { UserService } from '@app/core/services/user.service';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-register-form-preview',
    templateUrl: './register-form-preview.component.html',
    styleUrls: ['./register-form-preview.component.scss'],
})
export class RegisterFormPreviewComponent implements OnDestroy {
    @Input() form!: FormGroup<ApplicationForm>;
    @Input() documentForm!: FormGroup<documentTypeFormGroup>;

    constructor(private translateService: TranslocoService, private userService: UserService) {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                this.roles = [
                    {
                        name: this.translateService.translate('enums.administratorsAndCentersRoles.administrator'),
                        value: 'ADMINISTRATOR',
                    },
                    {
                        name: this.translateService.translate('enums.administratorsAndCentersRoles.user'),
                        value: 'USER',
                    },
                ];
            })
        );
    }
    private subscriptions = new Subscription();
    roles: { name: string; value: string }[] = [];

    ngOnDestroy() {
        this.subscriptions.unsubscribe();
    }

    get applicantPersonalIdentifier(): string {
        return this.translateService.translate('modules.providers.register.' + this.userService.user.uidType);
    }

    roleNames(values: string[]) {
        return this.roles.filter(role => values.includes(role.value)).map(role => role.name);
    }

    get documentsList() {
        const list: any = [];
        if (this.documentForm) {
            this.documentForm.getRawValue().attachments.forEach(attachment => list.push(...attachment.files));
        }
        return list;
    }
}
