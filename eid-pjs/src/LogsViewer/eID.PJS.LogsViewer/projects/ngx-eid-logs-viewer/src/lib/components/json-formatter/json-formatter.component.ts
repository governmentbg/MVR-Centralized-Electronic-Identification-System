import { Component, Input } from '@angular/core';

@Component({
    selector: 'lib-json-formatter',
    templateUrl: './json-formatter.component.html',
    styleUrls: ['./json-formatter.component.scss'],
})
export class JsonFormatterComponent {
    @Input() files: any;

    pickUpRightColor(key: any, value: any) {
        if (key === 'errors' || typeof value === 'boolean') {
            return 'text-button-reject';
        } else if (key === 'sourceFile') {
            return 'text-button-confirm`';
        } else if (key === 'status' || key === 'eventId') {
            return 'json-bold';
        } else {
            return '';
        }
    }

    checkDataType(data: any) {
        if (this.isObject(data)) {
            return 'object';
        } else if (Array.isArray(data) && this.isObject(data[0])) {
            return 'array';
        } else {
            return;
        }
    }

    isObject(value: any): boolean {
        return (
            typeof value === 'object' && value !== null && !Array.isArray(value)
        );
    }

    isArray(data: any) {
        return Array.isArray(data);
    }
}
