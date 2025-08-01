import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { UserService } from '@app/core/services/user.service';
import { TranslocoService } from '@ngneat/transloco';
import { ApplicationForm } from '@app/features/centers/centers.dto';

@Component({
    selector: 'app-register-form-preview',
    templateUrl: './register-form-preview.component.html',
    styleUrls: ['./register-form-preview.component.scss'],
})
export class RegisterFormPreviewComponent {
    @Input() form!: FormGroup<ApplicationForm>;

    constructor(private translateService: TranslocoService, private userService: UserService) {}

    get applicantPersonalIdentifier(): string {
        return this.translateService.translate('modules.centers.register.' + this.userService.user.uidType);
    }
}
