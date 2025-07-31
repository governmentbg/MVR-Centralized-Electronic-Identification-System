import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-preview-field',
    templateUrl: './preview-field.component.html',
    styleUrls: ['./preview-field.component.scss'],
})
export class PreviewFieldComponent {
    @Input() label!: string;
    @Input() value?: any;
    @Input() isLoading?: boolean;
}
