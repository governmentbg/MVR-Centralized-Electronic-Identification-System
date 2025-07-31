package bg.bulsi.mvr.mpozei.backend.service;

import java.util.UUID;

public interface NotificationService {
    void sendNotification(String notificationType, UUID eidentityId);
}
