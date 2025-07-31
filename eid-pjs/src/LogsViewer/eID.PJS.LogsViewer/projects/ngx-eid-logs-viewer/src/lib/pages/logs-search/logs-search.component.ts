import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { TranslocoService } from '@ngneat/transloco';
import { FormArray, FormBuilder, FormControl, Validators } from '@angular/forms';
import { IBreadCrumbItems } from '../../interfaces/IBreadCrumbItems';
import { Subscription } from 'rxjs';
import { Operators } from '../../enums/operators.enum';
import { dateMoreThanValidate } from '../../validators/date';

@Component({
    selector: 'app-logs-search',
    templateUrl: './logs-search.component.html',
    styleUrls: ['./logs-search.component.scss'],
})
export class LogsSearchComponent implements OnDestroy {
    constructor(private router: Router, private translocoService: TranslocoService, private fb: FormBuilder) {
        this.languageChangeSubscription = translocoService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translocoService.translate('modules.journalLogs.txtJournal'),
                    routerLink: '/logs-viewer/journals',
                },
                {
                    label: this.translocoService.translate('modules.journalLogs.txtSearching'),
                },
            ];
            this.operatorsOptions = [
                {
                    name: this.translocoService.translate('modules.journalLogs.operators.txtEqual'),
                    code: Operators.Equal,
                },
                {
                    name: this.translocoService.translate('modules.journalLogs.operators.txtLessThan'),
                    code: Operators.LessThan,
                },
                {
                    name: this.translocoService.translate('modules.journalLogs.operators.txtGreaterThan'),
                    code: Operators.GreaterThan,
                },
                {
                    name: this.translocoService.translate('modules.journalLogs.operators.txtGreaterThanOrEqual'),
                    code: Operators.GreaterThanOrEqual,
                },
                {
                    name: this.translocoService.translate('modules.journalLogs.operators.txtLessThanOrEqual'),
                    code: Operators.LessThanOrEqual,
                },
                {
                    name: this.translocoService.translate('modules.journalLogs.operators.txtContains'),
                    code: Operators.Contains,
                },
                {
                    name: this.translocoService.translate('modules.journalLogs.operators.txtStartsWith'),
                    code: Operators.StartsWith,
                },
                {
                    name: this.translocoService.translate('modules.journalLogs.operators.txtEndsWith'),
                    code: Operators.EndsWith,
                },
            ];
        });

        const navigation = this.router.getCurrentNavigation();
        if (navigation) {
            const state = navigation.extras.state as any;
            if (state) {
                this.form.patchValue({
                    fromDate: state.fromDate,
                    toDate: state.toDate,
                });

                (this.propertiesForm as any).removeControl('field');
                (this.propertiesForm as any).removeControl('value');
                (this.propertiesForm as any).removeControl('operator');
                this.removeProperty(0);

                state.properties.map((propertyObject: any) => {
                    const temporaryFormGroup = this.fb.group({});
                    for (const [key, value] of Object.entries(propertyObject)) {
                        temporaryFormGroup.addControl(key as any, new FormControl(value, [Validators.required]));
                    }
                    this.properties.push(temporaryFormGroup);
                });
            }
        }
    }

    requestInProgress = false;

    propertiesForm = this.fb.group({
        field: new FormControl('', [Validators.required]),
        value: new FormControl('', [Validators.required]),
        operator: new FormControl(Operators.Contains, [Validators.required]),
    });

    form = this.fb.group({
        fromDate: new FormControl<Date | null>(
            new Date(new Date().setDate(new Date().getDate() - 5)),
            Validators.required
        ),
        toDate: new FormControl<Date | null>(new Date(), [Validators.required, dateMoreThanValidate()]),
        properties: this.fb.array([this.propertiesForm]),
    });

    maxPropertyFieldsNumbers = 10;
    breadcrumbItems: IBreadCrumbItems[] = this._initialBreadcrumbs;
    languageChangeSubscription: Subscription;

    operatorsOptions = [
        {
            name: this.translocoService.translate('modules.journalLogs.operators.txtEqual'),
            code: Operators.Equal,
        },
        {
            name: this.translocoService.translate('modules.journalLogs.operators.txtLessThan'),
            code: Operators.LessThan,
        },
        {
            name: this.translocoService.translate('modules.journalLogs.operators.txtGreaterThanOrEqual'),
            code: Operators.GreaterThanOrEqual,
        },
        {
            name: this.translocoService.translate('modules.journalLogs.operators.txtLessThanOrEqual'),
            code: Operators.LessThanOrEqual,
        },
        {
            name: this.translocoService.translate('modules.journalLogs.operators.txtContains'),
            code: Operators.Contains,
        },
        {
            name: this.translocoService.translate('modules.journalLogs.operators.txtStartsWith'),
            code: Operators.StartsWith,
        },
        {
            name: this.translocoService.translate('modules.journalLogs.operators.txtEndsWith'),
            code: Operators.EndsWith,
        },
    ];

    get _initialBreadcrumbs(): IBreadCrumbItems[] {
        return ([] as IBreadCrumbItems[]).concat([
            {
                label: this.translocoService.translate('modules.journalLogs.txtJournal'),
                routerLink: '/logs-viewer/journals',
            },
            {
                label: this.translocoService.translate('modules.journalLogs.txtSearching'),
            },
        ]);
    }

    get properties() {
        return this.form.get('properties') as FormArray;
    }

    validateToDate() {
        this.form.controls['toDate'].updateValueAndValidity();
    }

    cancel() {
        this.router.navigate(['/logs-viewer/journals']);
    }

    onSubmit() {
        this.form.markAllAsTouched();
        if (this.form.valid) {
            this.form.markAsPristine();
            this.router.navigate(['/logs-viewer/journals/log-file-results'], {
                skipLocationChange: true,
                state: { payload: this.form.value, shouldRefresh: true },
            });
        }
    }

    addProperty() {
        this.properties.push(
            this.fb.group({
                field: new FormControl('', [Validators.required]),
                value: new FormControl('', [Validators.required]),
                operator: new FormControl(Operators.Contains, [Validators.required]),
            })
        );
    }

    removeProperty(i: number) {
        this.properties.removeAt(i);
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }
}
