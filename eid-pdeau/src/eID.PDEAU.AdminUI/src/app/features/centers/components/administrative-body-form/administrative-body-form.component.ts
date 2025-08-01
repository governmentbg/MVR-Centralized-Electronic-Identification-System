import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ApplicationForm } from '@app/features/centers/centers.dto';

@Component({
    selector: 'app-administrative-body-form',
    templateUrl: './administrative-body-form.component.html',
    styleUrls: ['./administrative-body-form.component.scss'],
})
export class AdministrativeBodyFormComponent {
    @Input() form!: FormGroup<ApplicationForm>;
}
