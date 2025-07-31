import { TestBed } from '@angular/core/testing';

import { EmpowermentCheckClientService } from './empowerment-check-client.service';

describe('EmpowermentCheckClientService', () => {
    let service: EmpowermentCheckClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(EmpowermentCheckClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
