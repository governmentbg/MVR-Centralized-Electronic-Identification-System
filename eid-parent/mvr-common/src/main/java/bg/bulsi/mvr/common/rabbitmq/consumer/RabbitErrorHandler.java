package bg.bulsi.mvr.common.rabbitmq.consumer;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.RegistryMVRException;
import feign.RetryableException;
import lombok.extern.slf4j.Slf4j;
import org.springframework.amqp.core.Message;
import org.springframework.amqp.rabbit.listener.api.RabbitListenerErrorHandler;
import org.springframework.amqp.rabbit.support.ListenerExecutionFailedException;
import org.springframework.amqp.support.converter.RemoteInvocationResult;
import org.springframework.dao.DataAccessException;
import org.springframework.messaging.handler.annotation.support.MethodArgumentNotValidException;
import org.springframework.stereotype.Component;

@Component
@Slf4j
public class RabbitErrorHandler implements RabbitListenerErrorHandler {

	@Override
	public Object handleError(Message amqpMessage, org.springframework.messaging.Message<?> message,
			ListenerExecutionFailedException exception) {
		
		log.error(exception.getMessage(), exception);

		if(exception.getCause() instanceof DataAccessException) {
			return new RemoteInvocationResult(new RegistryMVRException(ErrorCode.REGISTRY_ERROR));
		}
		if (exception.getCause() instanceof RetryableException) {
			return new RemoteInvocationResult(exception.getCause().getCause());
		}
		if (exception.getCause() instanceof MethodArgumentNotValidException) {
			return new RemoteInvocationResult(((MethodArgumentNotValidException) exception.getCause()).getRootCause());
		}
		
		return new RemoteInvocationResult(exception.getCause());
	}
}
