package bg.bulsi.mvr.common.rabbitmq.producer;


import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

@Component
public class CustomProperties {
	
	public static final String REPLY_QUEUE_ROUTING_KEY = "reply.routingkey";
	
	@Value("${custom.rabbitmq.queue-prefix}")
	private String queuePrefix;
	
	@Value("${custom.rabbitmq.routing-key-prefix}")
	private String routingKeyPrefix;

	@Value("${custom.rabbitmq.reply-queue}")
	private String replyQueue;
	
	@Value("${custom.rabbitmq.reply-exchange}")
	private String replyExchange;
	
	@Value("${custom.rabbitmq.rpc-exchange}")
	private String rpcExchange;
	
	public CustomProperties() {
		//constructor is empty
	}

	public String getQueuePrefix() {
		return queuePrefix;
	}

	public void setQueuePrefix(String queuePrefix) {
		this.queuePrefix = queuePrefix;
	}

	public String getRoutingKeyPrefix() {
		return routingKeyPrefix;
	}

	public void setRoutingKeyPrefix(String routingKeyPrefix) {
		this.routingKeyPrefix = routingKeyPrefix;
	}

	public String getReplyQueue() {
		return replyQueue;
	}

	public void setReplyQueue(String replyQueue) {
		this.replyQueue = replyQueue;
	}

	public String getReplyExchange() {
		return replyExchange;
	}

	public void setReplyExchange(String replyExchange) {
		this.replyExchange = replyExchange;
	}

	public String getRpcExchange() {
		return rpcExchange;
	}

	public void setRpcExchange(String rpcExchange) {
		this.rpcExchange = rpcExchange;
	}
}
