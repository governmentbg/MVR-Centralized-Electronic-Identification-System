package bg.bulsi.mvr.pan_client;

import java.util.concurrent.ExecutionException;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.context.event.ApplicationReadyEvent;
import org.springframework.context.event.EventListener;
import org.springframework.scheduling.annotation.Async;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import bg.bulsi.mvr.pan_client.PanClientAutoConfiguration.PanFeignClient;
import lombok.extern.slf4j.Slf4j;


/**
 * Executes a HTTP request to register System/Module in PAN.
 */
@Slf4j
public class SystemRegistratorService {

	@Autowired
	private PanFeignClient panFeignClient;
	
	@Autowired
	private SystemRegistrator systemRegistrator;
	
	@Async
	@EventListener(ApplicationReadyEvent.class)
	//@EventListener(ContextRefreshedEvent.class)
	public void register() throws JsonProcessingException, InterruptedException, ExecutionException {
	    String system = new ObjectMapper().writeValueAsString(systemRegistrator);
        log.info("register() [system={}]", system);
	    
	    var systemId = panFeignClient.createOrUpdateSystem(system);
		
        log.info("register() [systemId={}]", systemId);
	}
}
