package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.mpozei.backend.service.NotificationService;
import bg.bulsi.mvr.pan_client.EventRegistrator;
import bg.bulsi.mvr.pan_client.NotificationSender;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.UUID;

@Service
public class NotificationServiceImpl implements NotificationService {
    @Autowired
    private NotificationSender notificationSender;

    @Autowired
    private EventRegistrator eventRegistrator;

    @Override
    public void sendNotification(String notificationType, UUID eidentityId) {
        EventRegistrator.Event event = eventRegistrator.getEvent(notificationType);
        this.notificationSender.send(event.code(), eidentityId);
    }
}
