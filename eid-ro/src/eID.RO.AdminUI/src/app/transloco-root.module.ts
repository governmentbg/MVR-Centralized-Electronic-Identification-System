import { HttpClient } from '@angular/common/http';
import {
    TRANSLOCO_LOADER,
    Translation,
    TranslocoLoader,
    TRANSLOCO_CONFIG,
    translocoConfig,
    TranslocoModule,
} from '@ngneat/transloco';
import { Injectable, isDevMode, NgModule } from '@angular/core';
import { TranslocoPreloadLangsModule } from '@ngneat/transloco-preload-langs';
import {
    DateFormatOptions,
    DefaultDateTransformer,
    TRANSLOCO_DATE_TRANSFORMER,
    TranslocoLocaleModule,
} from '@ngneat/transloco-locale';
import { forkJoin, map } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class TranslocoHttpLoader implements TranslocoLoader {
    constructor(private http: HttpClient) {}

    getTranslation(lang: string) {
        return this.http.get<Translation>(`/assets/i18n/${lang}.json`);
    }
}

export class CustomDateTransformer extends DefaultDateTransformer {
    public override transform(date: Date, locale: string, options: DateFormatOptions): string {
        // https://github.com/ngneat/transloco/blob/a363009e/libs/transloco-locale/src/lib/helpers.ts#L45-47
        if (date instanceof Date === false || isNaN(<any>date)) {
            return '';
        }

        const offset = date.getTimezoneOffset();
        date = new Date(date.getTime() - offset * 60 * 1000);

        switch (options.timeStyle) {
            case 'medium':
                return date.toISOString().split('.')[0].replace('T', ', ');
            default:
                return date.toISOString().split('T')[0];
        }
    }
}

@NgModule({
    exports: [TranslocoModule],
    imports: [
        TranslocoPreloadLangsModule.forRoot(['en']),
        TranslocoLocaleModule.forRoot({
            defaultLocale: 'bg',
            langToLocaleMapping: {
                en: 'en-US',
                bg: 'bg-BG',
            },
        }),
    ],
    providers: [
        {
            provide: TRANSLOCO_CONFIG,
            useValue: translocoConfig({
                availableLangs: ['en', 'bg'],
                defaultLang: 'bg',
                // Remove this option if your application doesn't support changing language in runtime.
                reRenderOnLangChange: true,
                prodMode: !isDevMode(),
            }),
        },
        { provide: TRANSLOCO_LOADER, useClass: TranslocoHttpLoader },
        {
            provide: TRANSLOCO_DATE_TRANSFORMER,
            useClass: CustomDateTransformer,
        },
    ],
})
export class TranslocoRootModule {}
