import { Component, Input } from '@angular/core';
import { IStep } from '../../interfaces/eid-management.interfaces';

@Component({
    selector: 'app-steps',
    templateUrl: './steps.component.html',
    styleUrls: ['./steps.component.scss'],
})
export class StepsComponent {
    @Input() currentStep = 0;
    @Input() steps: IStep[] = [{ name: '', currentInnerStep: 0, id: 0, ref: '' }];
}
