package bg.bulsi.mvr.common.rabbitmq.producer;

import org.springframework.amqp.core.MessageProperties;
import org.springframework.amqp.support.converter.RemoteInvocationResult;


/**
 * This class is needed for accessing {@link MessageProperties} in {@link EventSender}
 */
public class ExtendedRemoteInvocationResult extends RemoteInvocationResult {
	
	private MessageProperties messageProperties;

	public ExtendedRemoteInvocationResult(RemoteInvocationResult remoteInvocationResult, MessageProperties messageProperties) {
		this.setException(remoteInvocationResult.getException());
		this.setValue(remoteInvocationResult.getValue());
		
		this.messageProperties = messageProperties;
	}
	
	public MessageProperties getMessageProperties() {
		return messageProperties;
	}
}
