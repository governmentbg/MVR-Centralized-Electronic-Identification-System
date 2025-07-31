package bg.bulsi.mvr.common.rabbitmq.consumer;

import org.springframework.amqp.core.Message;
import org.springframework.amqp.rabbit.listener.adapter.ReplyPostProcessor;
import org.springframework.stereotype.Component;

import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;

@Component
public class RabbitReplyPostProcessor implements ReplyPostProcessor {
	
	@Override
	public Message apply(Message request, Message response) {
		response.getMessageProperties().setHeader(UserContext.USER_CONTEXT_KEY, UserContextHolder.toJson());
		
		return response;
	}
}
