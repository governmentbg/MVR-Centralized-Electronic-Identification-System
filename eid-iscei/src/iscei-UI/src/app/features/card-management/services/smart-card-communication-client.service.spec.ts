import { TestBed } from '@angular/core/testing';

import { SmartCardCommunicationClientService } from './smart-card-communication-client.service';

describe('SmartCardCommunicationClientService', () => {
    let service: SmartCardCommunicationClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(SmartCardCommunicationClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
