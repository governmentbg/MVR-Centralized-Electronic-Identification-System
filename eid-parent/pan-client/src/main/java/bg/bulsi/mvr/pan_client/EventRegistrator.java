package bg.bulsi.mvr.pan_client;

import java.util.List;


/**
 * This class is responsible for declaring {@link Event}s to be associated with the 
 * system to be registered in PAN.
 * Every module that needs to use PAN Client must implement this class and provide its own {@link Event}s.
 */
public interface EventRegistrator {

	List<Event> getEvents();
	
    Event getEvent(String eventCode);
	
	record Translation(String language, String shortDescription, String description) {}
	
	record Event(String code, boolean isMandatory, List<Translation> translations) {}
}
