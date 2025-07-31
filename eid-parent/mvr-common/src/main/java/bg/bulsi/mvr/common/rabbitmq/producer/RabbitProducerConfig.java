package bg.bulsi.mvr.common.rabbitmq.producer;


import org.springframework.amqp.core.Binding;
import org.springframework.amqp.core.BindingBuilder;
import org.springframework.amqp.core.DirectExchange;
import org.springframework.amqp.core.Queue;
import org.springframework.amqp.rabbit.AsyncRabbitTemplate;
import org.springframework.amqp.rabbit.connection.ConnectionFactory;
import org.springframework.amqp.rabbit.core.RabbitTemplate;
import org.springframework.amqp.rabbit.listener.SimpleMessageListenerContainer;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class RabbitProducerConfig {
	
	@Autowired
	private RabbitRoutingUtil rabbitRoutingUtil;
	
	@Autowired
	private CustomProperties customProperties;
	
	@Bean("replyQueue")
	public Queue replyQueue() {
		String queueName = this.rabbitRoutingUtil.createQueueName(this.customProperties.getReplyQueue());
		return new Queue(queueName);
	}

	@Bean("replyExchange")
	public DirectExchange replyExchange() {
		String exchangeName = this.rabbitRoutingUtil.createExchangeName(this.customProperties.getReplyExchange());

		return new DirectExchange(exchangeName);
	}

	@Bean
	public Binding replyBinding(Queue replyQueue, DirectExchange replyExchange) {
		return BindingBuilder.bind(replyQueue).to(replyExchange).with(CustomProperties.REPLY_QUEUE_ROUTING_KEY);
	}

	@Bean
	public AsyncRabbitTemplate asyncRabbitTemplate(ConnectionFactory connectionFactory,
			DirectExchange rpcExchange, DirectExchange replyExchange,Queue replyQueue, 
			ExtendedMessageConverter messageConverter) {

		RabbitTemplate rabbitTemplate = new RabbitTemplate(connectionFactory);
		rabbitTemplate.setExchange(rpcExchange.getName());
		rabbitTemplate.setRoutingKey("");
		rabbitTemplate.setMessageConverter(messageConverter);
		
		SimpleMessageListenerContainer container = new SimpleMessageListenerContainer(connectionFactory);
		container.setQueues(replyQueue);
		
		return new AsyncRabbitTemplate(rabbitTemplate, 
				container, 
				replyExchange.getName() + "/" + CustomProperties.REPLY_QUEUE_ROUTING_KEY);
	}

	@Bean("rpcExchange")
	public DirectExchange rpcExchange() {
		String exchangeName = this.rabbitRoutingUtil.createExchangeName(this.customProperties.getRpcExchange());
		
		return new DirectExchange(exchangeName);
	}
}
