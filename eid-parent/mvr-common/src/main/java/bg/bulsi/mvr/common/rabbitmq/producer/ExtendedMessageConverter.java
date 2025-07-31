package bg.bulsi.mvr.common.rabbitmq.producer;

import org.springframework.amqp.core.Message;
import org.springframework.amqp.support.converter.MessageConversionException;
import org.springframework.amqp.support.converter.RemoteInvocationResult;
import org.springframework.amqp.support.converter.SimpleMessageConverter;
import org.springframework.stereotype.Component;

@Component
public class ExtendedMessageConverter extends SimpleMessageConverter{

	@Override
	public Object fromMessage(Message message) throws MessageConversionException {
		Object content = super.fromMessage(message);
		if(content instanceof RemoteInvocationResult result) {
			content = new ExtendedRemoteInvocationResult(result, message.getMessageProperties());
		}
		
		return content;
	}
}
