import { TestBed } from '@angular/core/testing';

import { JournalConfigurationService } from './journal-configuration.service';

describe('JournalConfigurationService', () => {
    let service: JournalConfigurationService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(JournalConfigurationService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
