import { TestBed } from '@angular/core/testing';

import { NotificationsClientService } from './notifications-client.service';

describe('ChannelsClientService', () => {
    let service: NotificationsClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(NotificationsClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
