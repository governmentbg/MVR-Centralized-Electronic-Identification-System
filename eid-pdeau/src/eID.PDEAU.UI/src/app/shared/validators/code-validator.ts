import { AsyncValidatorFn } from '@angular/forms';
import { catchError, debounceTime, distinctUntilChanged, map, of, switchMap } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class UniqueCodeValidator {
    constructor(private http: HttpClient) {}

    checkCodeExists(isOfficeCode: boolean): AsyncValidatorFn {
        return control => {
            if (!control.valueChanges || control.pristine) {
                return of(null);
            } else {
                const queryParams = new HttpParams({ fromObject: { isOfficeCode: isOfficeCode } as any });
                return of(control.value).pipe(
                    debounceTime(500),
                    distinctUntilChanged(),
                    switchMap(() =>
                        this.http
                            .get<any>(`/raeicei/external/api/v1/checkcode/${control.value}`, { params: queryParams })
                            .pipe(
                                catchError(() => {
                                    return of(undefined);
                                }),
                                map((response: any) => {
                                    if (response) {
                                        return null;
                                    } else {
                                        return { codeExists: true };
                                    }
                                })
                            )
                    )
                );
            }
        };
    }
}
