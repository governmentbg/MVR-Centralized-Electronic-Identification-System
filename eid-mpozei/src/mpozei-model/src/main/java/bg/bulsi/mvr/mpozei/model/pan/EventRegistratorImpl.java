package bg.bulsi.mvr.mpozei.model.pan;

import java.util.HashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;

import org.springframework.stereotype.Component;

import bg.bulsi.mvr.pan_client.EventRegistrator;

@Component
public class EventRegistratorImpl implements EventRegistrator{
	
	public static final String MPOZEI_E_ISSUED_EID = "MPOZEI_E_ISSUED_NEW_EID";
	public static final String MPOZEI_E_STOPPED_EID = "MPOZEI_E_STOPPED_EID";
	public static final String MPOZEI_E_RESUMED_EID = "MPOZEI_E_RESUMED_EID";
	public static final String MPOZEI_E_REVOKED_EID = "MPOZEI_E_REVOKED_EID";
	public static final String MPOZEI_E_PAY_FEE_EID = "MPOZEI_E_PAY_FEE_EID";
	public static final String MPOZEI_E_DENIED_DUE_TO_INVALID_CHECK_EID = "MPOZEI_E_DENIED_DUE_TO_INVALID_CHECK_EID";
	public static final String MPOZEI_E_SUBMITTED_FOR_PROCESSING_EID = "MPOZEI_E_SUBMITTED_FOR_PROCESSING_EID";
	public static final String MPOZEI_E_SUCCESSFUL_PIN_CHANGE = "MPOZEI_E_SUCCESSFUL_PIN_CHANGE";
	
	private static final Event issuedEid = new Event(MPOZEI_E_ISSUED_EID, true, List.of(new Translation(Locale.ENGLISH.getLanguage(), "Your eID certificate has been issued.", "Your Electronic Identity Certificate has been issued."), 
			new Translation("bg", "Вашето УЕИ е издадено.", "Вашето Удостоверение за електронна идентичност е издадено.")));
	private static final Event stoppedEid = new Event(MPOZEI_E_STOPPED_EID, true, List.of(new Translation(Locale.ENGLISH.getLanguage(), "Your eID certificate has been stopped.", "Your Electronic Identity Certificate has been stopped."), 
			new Translation("bg", "Вашето УЕИ е спряно.", "Вашето Удостоверение за електронна идентичност е спряно.")));
	private static final Event revokedEid = new Event(MPOZEI_E_REVOKED_EID, true, List.of(new Translation(Locale.ENGLISH.getLanguage(), "Your eID certificate has been terminated.", "Your Certificate of Electronic Identity has been terminated."), 
			new Translation("bg", "Вашето УЕИ е прекратено.", "Вашето Удостоверение за електронна идентичност е прекратено.")));
	private static final Event resumedEid = new Event(MPOZEI_E_RESUMED_EID, true, List.of(new Translation(Locale.ENGLISH.getLanguage(), "Your eID certificate has been resumed.", "Your Electronic Identity Certificate has been resumed."), 
			new Translation("bg", "Вашето УЕИ е възобновено.", "Вашето Удостоверение за електронна идентичност е възобновено.")));
	private static final Event submittedForProcessingEid = new Event(MPOZEI_E_SUBMITTED_FOR_PROCESSING_EID, true, List.of(new Translation(Locale.ENGLISH.getLanguage(), "eID application has been submitted for processing.", "Your Electronic Identity Certificate application has been submitted for processing."), 
			new Translation("bg", "Вашето заявление за УЕИ е подадено за обработка.", "Вашето заявление за Удостоверение за електронна идентичност е подадено за обработка.")));
	private static final Event deniedDueToInvalidChecksEid = new Event(MPOZEI_E_DENIED_DUE_TO_INVALID_CHECK_EID, true, List.of(new Translation(Locale.ENGLISH.getLanguage(), "eID application has been refused due to invalid checks.", "Your eID certificate application has been refused due to invalid checks."), 
			new Translation("bg", "Вашето заявление за УЕИ е отказано, поради невалидни проверки.", "Вашето заявление за Удостоверение за електронна идентичност е отказано, поради невалидни проверки.")));
	private static final Event paymentRequiredEid = new Event(MPOZEI_E_PAY_FEE_EID, true, List.of(new Translation(Locale.ENGLISH.getLanguage(), "eID application accepted. Please pay the fee.", "Your eID certificate application has been accepted, please pay the fee."), 
			new Translation("bg", "Вашето заявление за УЕИ е прието, моля платете такса.", "Вашето заявление за Удостоверение за електронна идентичност е прието, моля платете такса.")));
	private static final Event successfullPinChange = new Event(MPOZEI_E_SUCCESSFUL_PIN_CHANGE, true, List.of(new Translation(Locale.ENGLISH.getLanguage(), "The PIN for your card has been changed.", "The PIN for your card has been changed."), 
			new Translation("bg", "Пинът на картата Ви беше сменен", "Пинът на картата Ви беше сменен")));
	
	private static final Map<String, Event> events = new HashMap<>();

	static {
		events.put(MPOZEI_E_ISSUED_EID, issuedEid);
		events.put(MPOZEI_E_STOPPED_EID, stoppedEid);
		events.put(MPOZEI_E_REVOKED_EID, revokedEid);
		events.put(MPOZEI_E_RESUMED_EID, resumedEid);
		events.put(MPOZEI_E_SUBMITTED_FOR_PROCESSING_EID, submittedForProcessingEid);
		events.put(MPOZEI_E_DENIED_DUE_TO_INVALID_CHECK_EID, deniedDueToInvalidChecksEid);
		events.put(MPOZEI_E_PAY_FEE_EID, paymentRequiredEid);
		events.put(MPOZEI_E_SUCCESSFUL_PIN_CHANGE, successfullPinChange);
	}

	@Override
	public List<Event> getEvents() {
		return List.copyOf(events.values());
	}
	
	public Event getEvent(String eventCode) {
		return events.get(eventCode);
	}
}
