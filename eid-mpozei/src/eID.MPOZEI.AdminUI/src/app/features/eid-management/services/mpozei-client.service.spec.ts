import { TestBed } from '@angular/core/testing';

import { MpozeiClientService } from './mpozei-client.service';

describe('MpozeiClientService', () => {
    let service: MpozeiClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(MpozeiClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
