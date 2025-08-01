package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.mapper.NotificationEventMapper;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.pan_client.EventRegistrator.Event;
import bg.bulsi.mvr.pan_client.NotificationSender;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Component
@RequiredArgsConstructor
public class SendNotificationStep extends Step<AbstractApplication>  {
	
	@Autowired
	private NotificationSender notificationSender;
	
	@Autowired
	private NotificationEventMapper eventMapper;

    @Value("${notification-sending}")
	boolean shouldSendNotifications;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered SendNotificationStep", application.getId());
        if (!shouldSendNotifications) {
            return application;
        }
        Event event = eventMapper.applicationToEventMapper(application);
        
        if(event != null) {
            try {
            	if(ApplicationSubmissionType.BASE_PROFILE.equals(application.getSubmissionType())) {
            		this.notificationSender.sendByProfileId(event.code(), application.getCitizenProfileId());
            	} else {
            		this.notificationSender.send(event.code(), application.getEidentityId());
            	}
            } catch (Exception ex) {
                log.error("Notification with code: {} to eidentityId: {} cannot be sent", event.code(), application.getEidentityId());
            }
        }
        
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.SEND_NOTIFICATION;
    }
}
