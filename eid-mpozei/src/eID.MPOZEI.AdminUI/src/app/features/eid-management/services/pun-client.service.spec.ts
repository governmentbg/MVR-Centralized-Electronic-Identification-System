import { TestBed } from '@angular/core/testing';

import { PunClientService } from './pun-client.service';

describe('PunClientService', () => {
    let service: PunClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(PunClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
