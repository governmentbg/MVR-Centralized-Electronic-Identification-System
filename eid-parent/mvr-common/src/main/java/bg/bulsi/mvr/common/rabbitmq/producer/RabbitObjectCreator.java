package bg.bulsi.mvr.common.rabbitmq.producer;

import jakarta.annotation.PostConstruct;
import lombok.extern.slf4j.Slf4j;
import org.springframework.amqp.core.Queue;
import org.springframework.amqp.core.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.ApplicationContext;
import org.springframework.stereotype.Component;
import org.springframework.web.method.HandlerMethod;
import org.springframework.web.reactive.result.method.AbstractHandlerMethodMapping;
import org.springframework.web.reactive.result.method.RequestMappingInfo;

import java.util.*;

/**
 * This class is responsible for creating the dynamic {@link Queue} and {@link Binding}
 */
@Slf4j
@Component
public class RabbitObjectCreator {
	
	/**
	* Controller FQN containing one of the elements from the the collection bellow
	* will be skipped from the logic for rabbit queue/routing creation
	*/
	private static final Set<String> excludedControllers;
	
	static {
		excludedControllers = new HashSet<>();
		excludedControllers.add("org.springdoc");
	}
	
	@Autowired
	private RabbitRoutingUtil rabbitRoutingUtil;
	
	@Autowired
	private ApplicationContext applicationContext;
	
	@Autowired
	private AmqpAdmin amqpAdmin;
	
	@Autowired
	private DirectExchange rpcExchange;

	@Value("${custom.rabbitmq.skipped-queues:}#{T(java.util.Collections).emptyList()}")
	private List<String> skippedExtGatewayQueues = new ArrayList<>();
	
	@SuppressWarnings("unchecked")
	@PostConstruct
	public void setupRabbitObjects() {
		Map<RequestMappingInfo, HandlerMethod> handlerMethods = 
				((AbstractHandlerMethodMapping<RequestMappingInfo>) this.applicationContext
				.getBean("requestMappingHandlerMapping")).getHandlerMethods();
		
		for (Map.Entry<RequestMappingInfo, HandlerMethod> entry : handlerMethods.entrySet()) {
			log.info(".setupRabbitObjects() Part_1 [rmi={}, handlerMethod={}]", entry.getKey(), entry.getValue());
			
			//This should have the same value as methodLocation in the {@link RabbitControllerAdvice.java}
			String methodLocation = this.rabbitRoutingUtil.getMethodLocation(entry.getValue().toString());
			
			if(this.containExludedController(methodLocation)) {
				continue;
			}
			
			String commonName =  this.rabbitRoutingUtil.createCommonName(entry.getKey());

			/*
			Endpoints containing /external/ will use existing queues instead of creating new ones
			Here we are skipping the creation of queues and making sure routing key is the same as the existing queue
			*/
			if (skippedExtGatewayQueues.contains(commonName)) {
				commonName = commonName.replace("/external", "");
				String routingKey = rabbitRoutingUtil.createRoutingKeyWithPrefix(commonName);
				rabbitRoutingUtil.setRoutingKey(methodLocation, routingKey);
				continue;
			}

			String queueName = rabbitRoutingUtil.createQueueNameWithPrefix(commonName);
			Queue queue = new Queue(queueName, true);
			this.amqpAdmin.declareQueue(queue);

			String routingKey = rabbitRoutingUtil.createRoutingKeyWithPrefix(commonName);
			this.amqpAdmin.declareBinding(BindingBuilder.bind(queue).to(this.rpcExchange).with(routingKey));
			
			log.info(".setupRabbitObjects() Part_2 [methodLocation={}, routingKey={}]", methodLocation, routingKey);
			
			rabbitRoutingUtil.setRoutingKey(methodLocation, routingKey);
		}
	}
	
	private boolean containExludedController(String methodLocation) {
		for(String excludedController: excludedControllers) {
			if(methodLocation.contains(excludedController)) {
				return true;
			}
		}
		
		return false;
	}
}
