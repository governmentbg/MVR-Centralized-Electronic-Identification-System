import { Component, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { IpType } from '@app/shared/enums';
import { ipv6AllowedCharsPattern } from '@app/shared/validators/ip-validator';
import { TranslocoService } from '@ngneat/transloco';

@Component({
    selector: 'app-ais-form',
    templateUrl: './ais-form.component.html',
    styleUrls: ['./ais-form.component.scss'],
})
export class AisFormComponent {
    @Input() form!: FormGroup<{
        name: FormControl<string | null>;
        project: FormControl<string | null>;
        sourceIp: FormControl<string | null>;
        destinationIp: FormControl<string | null>;
        destinationIpType: FormControl<IpType | null>;
        protocolPort: FormControl<string | null>;
    }>;

    constructor(private translateService: TranslocoService) {}

    destinationIpTypes = [
        {
            name: this.translateService.translate('modules.providers.register.oneway'),
            id: IpType.OneWay,
        },
        {
            name: this.translateService.translate('modules.providers.register.twoway'),
            id: IpType.TwoWay,
        },
    ];

    onSourceIpInput(event: Event): void {
        const input = event.target as HTMLInputElement;
        const filteredValue = input.value
            .split('')
            .filter(char => ipv6AllowedCharsPattern.test(char))
            .join('');
        this.form.controls['sourceIp'].setValue(filteredValue);
    }

    onDestinationIpInput(event: Event): void {
        const input = event.target as HTMLInputElement;
        const filteredValue = input.value
            .split('')
            .filter(char => ipv6AllowedCharsPattern.test(char))
            .join('');
        this.form.controls['destinationIp'].setValue(filteredValue);
    }
}
