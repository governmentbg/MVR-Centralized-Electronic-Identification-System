import { TestBed } from '@angular/core/testing';

import { EidAdministratorService } from './eid-administrator.service';

describe('EidAdministratorService', () => {
    let service: EidAdministratorService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(EidAdministratorService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
