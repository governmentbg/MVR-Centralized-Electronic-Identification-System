import { TestBed } from '@angular/core/testing';

import { PivrClientService } from './pivr-client.service';

describe('EmpowermentClientService', () => {
    let service: PivrClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(PivrClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
