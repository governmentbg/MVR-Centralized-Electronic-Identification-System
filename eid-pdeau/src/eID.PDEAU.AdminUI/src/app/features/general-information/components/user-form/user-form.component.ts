import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { IdentifierType } from '@app/shared/enums';
import { EGNAdultValidator, EGNValidator, PinValidator } from '@app/shared/validators/uid-validator';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { cyrillicValidator } from '@app/shared/validators/cyrillic-validator';
import { createInputSanitizer } from '@app/shared/utils/create-input-sanitized';
import { mobilePhoneValidator } from '@app/shared/validators/mobile-phone-validator';
import { CreateUserType } from '@app/features/users/users.dto';

@Component({
    selector: 'app-user-form',
    templateUrl: './user-form.component.html',
    styleUrls: ['./user-form.component.scss'],
})
export class UserFormComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(private translateService: TranslocoService) {}

    @Input() user?: CreateUserType | null = null;
    @Input() isLoading?: boolean = false;
    @Output() submitEvent = new EventEmitter<CreateUserType>();
    @Output() cancelEvent = new EventEmitter();

    ngOnInit(): void {
        if (this.user) {
            this.form.patchValue({
                uidType: this.user?.uidType as IdentifierType,
                uid: this.user?.uid,
                name: this.user?.name,
                email: this.user?.email,
                phone: this.user?.phone,
                isAdministrator: this.user?.isAdministrator,
            });
        }

        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                // Force dropdown value clear because we update the options, but the selectedRoles are not being updated
                // this.form.controls.selectedRoles.setValue([]);

                this.identifierTypes = [
                    {
                        name: this.translateService.translate('enums.identifierType.EGN'),
                        id: IdentifierType.EGN,
                    },
                    {
                        name: this.translateService.translate('enums.identifierType.LNCh'),
                        id: IdentifierType.LNCh,
                    },
                ];

                this.roles = [
                    {
                        name: this.translateService.translate('enums.userRole.administrator'),
                        isAdministrator: true,
                    },
                    {
                        name: this.translateService.translate('enums.userRole.user'),
                        isAdministrator: false,
                    },
                ];
            })
        );
        this.form.controls.uidType.valueChanges.subscribe(value => {
            const validators = [Validators.required];
            if (value === IdentifierType.EGN) {
                validators.push(EGNValidator(), EGNAdultValidator());
            } else if (value === IdentifierType.LNCh) {
                validators.push(PinValidator());
            }
            const control = this.form.controls.uid;
            control.setValidators(validators);
            control.updateValueAndValidity();
        });
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    form = new FormGroup({
        uidType: new FormControl(IdentifierType.EGN, [Validators.required]),
        uid: new FormControl('', [Validators.required, EGNAdultValidator(), EGNValidator()]),
        name: new FormControl('', [Validators.required, Validators.maxLength(200), cyrillicValidator()]),
        email: new FormControl('', [Validators.required, Validators.email]),
        isAdministrator: new FormControl(false),
        phone: new FormControl('', [Validators.required, mobilePhoneValidator()]),
    });

    identifierTypes: { name: string; id: string }[] = [];

    roles: { name: string; isAdministrator: boolean }[] = [];

    onSubmit() {
        markFormGroupAsDirty(this.form);
        if (this.form.invalid) return;
        // Cast to formValues to RegisterUserData as we already passed validation
        const formValues = this.form.value as CreateUserType;
        this.submitEvent.emit(formValues);
    }

    onCancel() {
        this.cancelEvent.emit();
    }

    sanitizeNameInput = createInputSanitizer(this.form.controls.name);
}
