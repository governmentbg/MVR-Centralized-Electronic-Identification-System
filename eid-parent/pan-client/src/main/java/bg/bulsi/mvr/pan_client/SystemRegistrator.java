package bg.bulsi.mvr.pan_client;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;

import bg.bulsi.mvr.pan_client.EventRegistrator.Event;


/**
 * This class is responsible for declaring System to be registered in PAN(usually the module using this library).
 * Every module that needs to use PAN Client must implement this class.
 */
public abstract class SystemRegistrator {
	
	@Autowired(required = false)
	private EventRegistrator eventRegistrator;
	
	public abstract String getSystemName();
	
	public abstract List<Translation> getTranslations();
	
	public List<Event> getEvents(){
		return this.eventRegistrator.getEvents();
	}
	
	public record Translation (String language, String name) {}
}
