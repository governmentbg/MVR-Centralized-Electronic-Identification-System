//package bg.bulsi.mvr.common.rabbitmq.consumer;
//
//
//import com.rabbitmq.http.client.Client;
//import com.rabbitmq.http.client.ClientParameters;
//import com.rabbitmq.http.client.domain.QueueInfo;
//import jakarta.annotation.PostConstruct;
//import org.slf4j.LoggerFactory;
//import org.springframework.amqp.rabbit.annotation.RabbitListener;
//import org.springframework.beans.factory.annotation.Autowired;
//import org.springframework.beans.factory.annotation.Value;
//import org.springframework.boot.autoconfigure.amqp.RabbitProperties;
//import org.springframework.boot.autoconfigure.amqp.RabbitProperties.Ssl;
//import org.springframework.core.env.PropertySource;
//import org.springframework.stereotype.Component;
//import org.springframework.web.util.UriComponents;
//import org.springframework.web.util.UriComponentsBuilder;
//
//import java.lang.reflect.Method;
//import java.net.MalformedURLException;
//import java.net.URISyntaxException;
//import java.util.*;
//import java.util.stream.Collectors;
//
///**
// * This class provides additional @{link PropertySource} responsible for handling the custom RabbitMq properties.
// */
//@Component
//public class RabbitMqManagementPropertySource extends PropertySource<Map<String,Object>> {
//
//	private static final org.slf4j.Logger LOGGER = LoggerFactory.getLogger(RabbitMqManagementPropertySource.class);
//
//	/**
//	 * Source of the properties for the current class
//	 */
//	private Map<String,Object> properties = new HashMap<>();
//
//	@Autowired
//	private RabbitProperties rabbitProperties;
//
//	@Value("${custom.rabbitmq.management.port}")
//	private Integer managementPort;
//
//	@Value("${custom.rabbitmq.queue-prefix}")
//	private String queuePrefixToListen;
//
//	@Value("${custom.rabbitmq.listener.dispatcher}")
//	private String rabbitListenerDispatcher;
//
//	@Value("${server.servlet.context-path:}")
//	private String basePath;
//
//	public RabbitMqManagementPropertySource() {
//		super(RabbitMqManagementPropertySource.class.getSimpleName());
//	}
//
//	@Override
//	public Object getProperty(String name) {
//		return properties.get(name);
//	}
//
//	@Override
//	public Map<String,Object> getSource() {
//		return this.properties;
//	}
//
//	/**
//	 * This method populates {@link properties}
//	 *
//	 * @throws MalformedURLException
//	 * @throws URISyntaxException
//	 * @throws ClassNotFoundException
//	 * @throws SecurityException
//	 */
//	@PostConstruct
//	private void getRabbitQueues() throws MalformedURLException, URISyntaxException, SecurityException, ClassNotFoundException {
//		String host = this.rabbitProperties.getHost();
//		Ssl ssl = this.rabbitProperties.getSsl();
//		String username = this.rabbitProperties.getUsername();
//		String password = this.rabbitProperties.getPassword();
//
//		boolean isHttps = Optional.ofNullable(ssl.getEnabled()).orElse(false);
//
//		UriComponents uriComponents = UriComponentsBuilder.newInstance()
//				.scheme(isHttps ? "https" : "http")
//				.host(host).port(managementPort).path("api").build();
//
//		Client client = new Client(
//		  new ClientParameters()
//		    .url(uriComponents.toUriString())
//		    .username(username)
//		    .password(password));
//
//		queuePrefixToListen = basePath.length() != 0
//				? (basePath.substring(1) + "." + queuePrefixToListen.toLowerCase())
//				: queuePrefixToListen.toLowerCase();
//
//		List<String> allQueueNames = extractAllQueues(client, queuePrefixToListen);
//
//		this.extractQueuesToExclude(allQueueNames);
//	}
//
//	/**
//	 * Gets all queues that start with a given prefix.
//	 * The prefix is specified in the application.properties.
//	 *
//	 * @param client
//	 * @return
//	 */
//	private List<String> extractAllQueues(Client client, String queuePrefix) {
//		List<String> allQueueNames = new ArrayList<>();
//		for (QueueInfo queueInfo : client.getQueues()) {
//			if(!queueInfo.getName().toLowerCase().contains(queuePrefix)) {
//				continue;
//			}
//
//			allQueueNames.add(queueInfo.getName());
//		}
//
//		LOGGER.info(".extractAllQueues() allQueueNames = {};", allQueueNames);
//
//		this.properties.put("allQueueNames", allQueueNames);
//		return allQueueNames;
//	}
//
//	/**
//	 * Add to the current {@link PropertySource} all queues that do not have handler
//	 *
//	 * @param allQueueNames
//	 * @throws ClassNotFoundException
//	 * @throws SecurityException
//	 */
//	private void extractQueuesToExclude(List<String> allQueueNames) throws SecurityException, ClassNotFoundException {
//		List<String> availableQueueHandlers = new LinkedList<>();
//
//		Method[] methods = Class.forName(this.rabbitListenerDispatcher).getMethods();
//		for(Method method: methods) {
//
//			DefaultRabbitListener[] rabbitListener = method.getAnnotationsByType(DefaultRabbitListener.class);
//			if(rabbitListener.length != 0) {
//
//				String queueName = rabbitListener[0].queues()[0];
//				//Check if it has {} for expression language or placeholder
//				if(queueName.contains("{") || queueName.contains("}")) {
//					continue;
//				}
//
//				availableQueueHandlers.add(queueName);
//			}
//		}
//
//		List<String> queuesToExclude = allQueueNames.stream().filter(queue -> !availableQueueHandlers.contains(queue)).collect(Collectors.toList());
//
//		LOGGER.info(".getRabbitQueues() queuesToExclude = {};", queuesToExclude);
//
//		this.properties.put("queuesToExclude", queuesToExclude);
//	}
//}
