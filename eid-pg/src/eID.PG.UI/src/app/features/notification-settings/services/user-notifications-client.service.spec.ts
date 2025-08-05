import { TestBed } from '@angular/core/testing';
import { UserNotificationsClientService } from './user-notifications-client.service';

describe('NotificationSettingsService', () => {
    let service: UserNotificationsClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(UserNotificationsClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
