package bg.bulsi.mvr.common.rabbitmq.producer;

import java.util.HashMap;
import java.util.Map;
import java.util.Set;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.springframework.amqp.core.Binding;
import org.springframework.amqp.core.Queue;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.reactive.result.method.RequestMappingInfo;
import org.springframework.web.util.pattern.PathPattern;


@Component
public class RabbitRoutingUtil {

	private final Map<String, String> requestRabbitRouting = new HashMap<>();

	/**
	 * {@link Pattern} for extracting class name and method name
	 */
	private Pattern patternClassMethodName = Pattern.compile("(.*?)\\(.*");
	
	@Autowired
	private CustomProperties customProperties;
	
	private RabbitRoutingUtil() {
	}

	public String getRoutingKey(String methodLocation) {
		return requestRabbitRouting.get(methodLocation);
	}

	public void setRoutingKey(String methodLocation, String routingKey) {
		requestRabbitRouting.put(methodLocation, routingKey);
	}

	/**
	 * Creates basic part of the name for the dynamic {@link Queue} and {@link Binding}.
	 * The name is based on the HTTP Method and the path for the {@link RequestMappingInfo}
	 * 
	 * @param rmi
	 * @return
	 */
	public String createCommonName(RequestMappingInfo rmi) {
		StringBuilder builder = new StringBuilder();

		// The code bellow is from {@link
		// org.springframework.web.reactive.result.method.RequestMappingInfo.toString()}
		if (!rmi.getMethodsCondition().isEmpty()) {
			Set<RequestMethod> httpMethods = rmi.getMethodsCondition().getMethods();
			builder.append(httpMethods.size() == 1 ? httpMethods.iterator().next() : httpMethods);
		}

		if (!rmi.getPatternsCondition().isEmpty()) {
			Set<PathPattern> patterns = rmi.getPatternsCondition().getPatterns();

			String patternsString = patterns.iterator().next().toString();
			builder.append(patternsString.toLowerCase());
		}

		return builder.toString();
	}
	
	public String getMethodLocation(String input) {
		/** org.springframework.web.reactive.HandlerMapping.bestMatchingHandler=
		* com.example.springWebflux_springamqp.Controller2#test1(ServerWebExchange, Model)
		 */
		Matcher matcher = patternClassMethodName.matcher(input);
		String methodLocation = null;
		if (matcher.matches()) {
			methodLocation = matcher.group(1);
		}
		
		return methodLocation;
	}
	
	public String createQueueNameWithPrefix(String commonName) {
		return this.formatString(customProperties.getQueuePrefix() + commonName);
	}

	public String createRoutingKeyWithPrefix(String commonName) {
		return this.formatString(customProperties.getRoutingKeyPrefix() + commonName);
	}
	
	public String createQueueName(String commonName) {
		return this.formatString(commonName);
	}

	public String createRoutingKey(String commonName) {
		return this.formatString(commonName);
	}
	
	public String createExchangeName(String commonName) {
		return this.formatString(commonName);
	}
	
	public String formatString(String input) {
		input = input.replaceAll("[{}]", "");
		input = input.replaceAll("/+", ".");

		if (input.charAt(0) == '.') {
			input = input.substring(1);
		}
		
		if (input.charAt(input.length() - 1) == '.') {
			input = input.substring(0, input.length() - 1);
		}		
		
		return input;
	}
}
