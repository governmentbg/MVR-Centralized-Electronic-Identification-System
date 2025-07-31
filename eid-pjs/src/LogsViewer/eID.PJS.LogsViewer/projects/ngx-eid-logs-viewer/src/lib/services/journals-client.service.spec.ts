import { TestBed } from '@angular/core/testing';

import { JournalsClientService } from './journals-client.service';

describe('JournalsService', () => {
    let service: JournalsClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(JournalsClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
