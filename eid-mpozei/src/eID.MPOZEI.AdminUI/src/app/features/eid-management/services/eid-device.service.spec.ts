import { TestBed } from '@angular/core/testing';

import { EidDeviceService } from './eid-device.service';

describe('EidDeviceService', () => {
    let service: EidDeviceService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(EidDeviceService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
