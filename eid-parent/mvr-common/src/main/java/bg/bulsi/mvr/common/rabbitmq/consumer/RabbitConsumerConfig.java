package bg.bulsi.mvr.common.rabbitmq.consumer;

import java.util.Map;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.ApplicationContext;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.env.ConfigurableEnvironment;
import org.springframework.core.env.PropertySource;

import jakarta.annotation.PostConstruct;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Configuration
public class RabbitConsumerConfig {
	
	@Autowired
	private ConfigurableEnvironment confEnv;
	
//	@Autowired
//	private PropertySource<Map<String,Object>> rabbitMqManagementPropertySource;
	
	/*
	 * @Autowired private ApplicationContext context;
	 */

	@PostConstruct
	public void queues(){
		log.info("queues() START");

//		confEnv.getPropertySources().addFirst(rabbitMqManagementPropertySource);

		log.info("queues() confEnv.containsProperty(queuesToExclude) = " + confEnv.containsProperty("queuesToExclude"));

		log.info("queues() confEnv.resolvePlaceholders(queuesToExclude) = " + confEnv.resolvePlaceholders("queuesToExclude"));

		log.info("queues() END");
	}
}
