package bg.bulsi.mvr.iscei.pan;

import jakarta.annotation.PostConstruct;
import org.springframework.stereotype.Component;

import bg.bulsi.mvr.pan_client.EventRegistrator;

import java.util.HashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;

@Component
public class EventRegistratorImpl implements EventRegistrator {

	public static final String ISCEI_SUCCESSFUL_AUTHENTICATION_WITH_EID = "ISCEI_SUCCESSFUL_AUTHENTICATION_WITH_EID";
	public static final String ISCEI_SUCCESSFUL_AUTHENTICATION_WITH_BASIC_PROFILE = "ISCEI_SUCCESSFUL_AUTHENTICATION_WITH_BASIC_PROFILE";
	public static final String ISCEI_ATTEMPT_AUTHENTICATION_WITH_EID = "ISCEI_ATTEMPT_AUTHENTICATION_WITH_EID";
//	public static final String ISCEI_ATTEMPT_AUTHENTICATION_WITH_BASIC_PROFILE = "ISCEI_ATTEMPT_AUTHENTICATION_WITH_BASIC_PROFILE";
	public static final String ISCEI_NEW_APPROVAL_REQUEST = "ISCEI_NEW_APPROVAL_REQUEST";

	private static final Map<String, Event> events = new HashMap<>();

	@PostConstruct
	public void init() {
		Event authSuccessEid = new Event(ISCEI_SUCCESSFUL_AUTHENTICATION_WITH_EID, true,
				List.of(
						new Translation(Locale.ENGLISH.getLanguage(), "Successful authentication with your electronic identity.", "Successful authentication with your electronic identity."),
				        new Translation("bg", "Извършена е успешна автентикация с Вашата електронна идентичност", "Извършена е успешна автентикация с Вашата електронна идентичност.")));
		
		Event authSuccessBasicProfile = new Event(ISCEI_SUCCESSFUL_AUTHENTICATION_WITH_BASIC_PROFILE, true,
				List.of(
						new Translation(Locale.ENGLISH.getLanguage(), "Successful authentication with username and password.", "Successful authentication with username and password."),
				        new Translation("bg", "Извършена е успешна автентикация с потребителско име и парола.", "Извършена е успешна автентикация с потребителско име и парола.")));

		Event authAttemptEid = new Event(ISCEI_ATTEMPT_AUTHENTICATION_WITH_EID, true,
				List.of(
						new Translation(Locale.ENGLISH.getLanguage(), "Authentication has been attempted with your electronic identity.", "Authentication has been attempted with your electronic identity."),
				        new Translation("bg", "Извършен е опит за автентикация с Вашата електронна идентичност.", "Извършен е опит за автентикация с Вашата електронна идентичност.")));
		
//		Event authAttemptBasicProfile = new Event(ISCEI_ATTEMPT_AUTHENTICATION_WITH_BASIC_PROFILE, true,
//				List.of(
//						new Translation(Locale.ENGLISH.getLanguage(), "Authentication attempt with your basic profile", "Authentication attempt with your basic profile!"),
//				        new Translation("bg", "Извършен е опит за автентикация с вашият базов профил", "Извършен е опит за автентикация с вашият базов профил!")));
		
		Event authNewApprovalRequest = new Event(ISCEI_NEW_APPROVAL_REQUEST, true,
		List.of(
				new Translation(Locale.ENGLISH.getLanguage(), "You have new approval authentication request with EID.", "You have new approval authentication request with EID."),
		        new Translation("bg", "Имате нова чакаща заявка за автентикация с УЕИ.", "Имате нова чакаща заявка за автентикация с УЕИ.")));
		
		events.put(ISCEI_SUCCESSFUL_AUTHENTICATION_WITH_EID, authSuccessEid);
		events.put(ISCEI_SUCCESSFUL_AUTHENTICATION_WITH_BASIC_PROFILE, authSuccessBasicProfile);
		events.put(ISCEI_ATTEMPT_AUTHENTICATION_WITH_EID, authAttemptEid);
//		events.put(ISCEI_ATTEMPT_AUTHENTICATION_WITH_BASIC_PROFILE, authAttemptBasicProfile);
		events.put(ISCEI_NEW_APPROVAL_REQUEST, authNewApprovalRequest);
	}
	
	@Override
	public List<Event> getEvents() {
		return List.copyOf(events.values());
	}
	
	public Event getEvent(String eventCode) {
		return events.get(eventCode);
	}
}
