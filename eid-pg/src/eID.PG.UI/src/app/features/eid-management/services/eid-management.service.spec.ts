import { TestBed } from '@angular/core/testing';

import { EidManagementService } from './eid-management.service';

describe('EidManagementService', () => {
    let service: EidManagementService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(EidManagementService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
