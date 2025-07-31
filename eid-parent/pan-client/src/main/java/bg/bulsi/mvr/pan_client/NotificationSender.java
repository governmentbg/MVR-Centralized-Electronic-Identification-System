package bg.bulsi.mvr.pan_client;

import bg.bulsi.mvr.common.dto.IdentifierType;
import bg.bulsi.mvr.pan_client.Notification.Translation;
import bg.bulsi.mvr.pan_client.PanClientAutoConfiguration.PanFeignClient;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.scheduling.annotation.Async;

import java.util.UUID;

/**
 * Class responsible for internationalisation and sending of notifications to PAN.
 * Should send all available translations of the template.
 */
@Slf4j
public class NotificationSender {
	
	
	@Autowired
	private PanFeignClient panFeignClient;
	
	@Autowired(required = false)
	private EventRegistrator eventRegistrator;
	
	public void sendDirectEmail(DirectEmailRequest request) {
		log.info(".send() [emailAddress={}, subject={}]", request.getEmailAddress(), request.getSubject());
		
		request.setBody(request.getBody() + " \n");
		panFeignClient.sendDirectEmail(request);
	}
	
	@Async
	public void send(String eventCode, UUID eid) {
		log.info(".send() [eventCode={}, eid={}]", eventCode, eid);

	    Notification notification = new Notification();
	    notification.setEId(eid);
	    notification.setEventCode(eventCode);
	    
	    processNotification(eventCode, notification);
	}

	@Async
	public void sendByProfileId(String eventCode, UUID userId) {
		log.info(".sendByProfileId() [eventCode={}, userId={}]", eventCode, userId);

	    Notification notification = new Notification();
	    notification.setUserId(userId);
	    notification.setEventCode(eventCode);
	    
	    processNotification(eventCode, notification);
	}
	
	@Async
	public void sendByUid(String eventCode, String uid, IdentifierType uidType) {
		log.info(".sendByUid() [eventCode={}]", eventCode);

	    Notification notification = new Notification();
	    notification.setUid(uid);
	    notification.setUidType(uidType);
	    notification.setEventCode(eventCode);
	    
	    processNotification(eventCode, notification);
	}
	
	private void processNotification(String eventCode, Notification notification) {
		notification.setTranslations(
	    		this.eventRegistrator.getEvent(eventCode).translations()
	    		.stream()
	    		.map(t -> new Translation(t.language(), t.description()))
	    		.toList());
	    
	    this.panFeignClient.sendNotification(notification);
	}
}
