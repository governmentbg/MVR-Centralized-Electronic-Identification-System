import { TestBed } from '@angular/core/testing';

import { PjsClientService } from './pjs-client.service';

describe('PjsClientService', () => {
    let service: PjsClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(PjsClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
