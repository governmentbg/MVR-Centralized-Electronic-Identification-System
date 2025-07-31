package bg.bulsi.mvr.mpozei.backend.mapper;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.pan.EventRegistratorImpl;
import bg.bulsi.mvr.pan_client.EventRegistrator;
import bg.bulsi.mvr.pan_client.EventRegistrator.Event;

/**
 *  Gets the {@link Event} depending on the status of the {@link AbstractApplication}
 */
@Component
public class NotificationEventMapper {

	@Autowired
	private EventRegistrator eventRegistrator;
	
	public Event applicationToEventMapper(AbstractApplication application) {
		
		if(application.getStatus() == ApplicationStatus.PENDING_PAYMENT) {
			return eventRegistrator.getEvent(EventRegistratorImpl.MPOZEI_E_PAY_FEE_EID);
		}
		
		return switch (application.getApplicationType()) {
		    case ISSUE_EID -> eventRegistrator.getEvent(EventRegistratorImpl.MPOZEI_E_ISSUED_EID);
		    case RESUME_EID -> eventRegistrator.getEvent(EventRegistratorImpl.MPOZEI_E_RESUMED_EID);
		    case REVOKE_EID -> eventRegistrator.getEvent(EventRegistratorImpl.MPOZEI_E_REVOKED_EID);
		    case STOP_EID -> eventRegistrator.getEvent(EventRegistratorImpl.MPOZEI_E_STOPPED_EID);
		    
		    default -> null;
		};
	}
}
