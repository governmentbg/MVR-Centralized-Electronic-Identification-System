import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class DateParserService {
    parseDateString(dateString: string): string | null {
        // Possible formats:
        // dd/MM/YYYY
        // yyyy-MM-dd
        // dd.MM.yyyy
        // dd.MM.yyyy
        // 00.mm.yyyy
        // 00.00.yyyy
        const formats = [
            { regex: /^(\d{2})\/(\d{2})\/(\d{4})$/, order: ['day', 'month', 'year'] },
            { regex: /^(\d{4})-(\d{2})-(\d{2})$/, order: ['year', 'month', 'day'] },
            { regex: /^(\d{2})\.(\d{2})\.(\d{4})$/, order: ['day', 'month', 'year'] },
            { regex: /^(\d{2})\.(\d{2})\.(\d{4})$/, order: ['day', 'month', 'year'] },
            { regex: /^00\.(\d{2})\.(\d{4})$/, order: ['day', 'month', 'year'], defaultDay: '01' },
            { regex: /^00\.00\.(\d{4})$/, order: ['day', 'month', 'year'], defaultDay: '01', defaultMonth: '01' },
        ];

        for (const format of formats) {
            const match = dateString.match(format.regex);
            if (match) {
                const dateParts: { [key: string]: string } = {};
                format.order.forEach((part, index) => {
                    dateParts[part] = match[index + 1];
                });

                const day = dateParts['day'] === '00' ? format.defaultDay || '01' : dateParts['day'];
                const month = dateParts['month'] === '00' ? format.defaultMonth || '01' : dateParts['month'];
                const year = dateParts['year'];

                let parsedDate = new Date(parseInt(year, 10), parseInt(month, 10) - 1, parseInt(day, 10));
                const offset = parsedDate.getTimezoneOffset();
                parsedDate = new Date(parsedDate.getTime() - offset * 60 * 1000);
                return parsedDate.toISOString().split('T')[0];
            }
        }

        return null; // Return null if no format matches
    }
}
