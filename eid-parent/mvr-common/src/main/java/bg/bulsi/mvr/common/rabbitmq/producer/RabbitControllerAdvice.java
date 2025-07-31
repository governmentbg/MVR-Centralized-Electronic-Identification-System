package bg.bulsi.mvr.common.rabbitmq.producer;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.reactive.HandlerMapping;
import org.springframework.web.server.ServerWebExchange;

import lombok.extern.slf4j.Slf4j;

@Slf4j
@ControllerAdvice
public class RabbitControllerAdvice {

	public static final String ROUTING_KEY_NAME = "routingKey";

	@Autowired
	private RabbitRoutingUtil rabbitRoutingUtil;
	
	/**
	 * Add routingKey to the {@link ServerWebExchange}.
	 * This way the every RequestMapping will have access to the routingKey.
	 * 
	 * @param exchange
	 * @param model
	 */
	@ModelAttribute
	public void addRoutingKey(ServerWebExchange exchange, Model model) {
		log.info(".addRoutingKey() [exchange={}]", exchange.getAttributes());
		
		String methodLocation = this.rabbitRoutingUtil.getMethodLocation(
				exchange.getAttributes().get(HandlerMapping.BEST_MATCHING_HANDLER_ATTRIBUTE).toString());
		
		String routingKey = rabbitRoutingUtil.getRoutingKey(methodLocation);
		
		if(routingKey != null) {
			exchange.getAttributes().put(ROUTING_KEY_NAME, routingKey);
		}
	}
}
