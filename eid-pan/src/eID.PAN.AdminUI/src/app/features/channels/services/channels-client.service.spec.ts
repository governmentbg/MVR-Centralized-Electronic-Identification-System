import { TestBed } from '@angular/core/testing';

import { ChannelsClientService } from './channels-client.service';

describe('ChannelsClientService', () => {
    let service: ChannelsClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(ChannelsClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
