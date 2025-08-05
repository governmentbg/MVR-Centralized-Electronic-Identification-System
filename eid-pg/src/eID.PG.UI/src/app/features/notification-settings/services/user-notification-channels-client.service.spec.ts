import { TestBed } from '@angular/core/testing';
import { UserNotificationChannelsClientService } from './user-notification-channels-client.service';

describe('UserNotificationChannelsService', () => {
    let service: UserNotificationChannelsClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(UserNotificationChannelsClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
