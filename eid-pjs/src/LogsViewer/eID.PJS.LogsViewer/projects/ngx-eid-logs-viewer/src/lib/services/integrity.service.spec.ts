import { TestBed } from '@angular/core/testing';

import { IntegrityClientService } from './integrity.service';

describe('JournalsService', () => {
    let service: IntegrityClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(IntegrityClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
