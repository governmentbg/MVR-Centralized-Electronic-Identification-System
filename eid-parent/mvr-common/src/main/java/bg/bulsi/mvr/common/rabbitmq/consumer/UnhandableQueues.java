//package bg.bulsi.mvr.common.rabbitmq.consumer;
//
//
//import org.springframework.amqp.rabbit.annotation.RabbitListener;
//import org.springframework.amqp.support.AmqpHeaders;
//import org.springframework.amqp.support.converter.RemoteInvocationResult;
//import org.springframework.context.annotation.DependsOn;
//import org.springframework.messaging.handler.annotation.Header;
//import org.springframework.stereotype.Component;
//
//import bg.bulsi.mvr.common.exception.FaultMVRException;
//import lombok.extern.slf4j.Slf4j;
//
//@Slf4j
//@Component
//@DependsOn(value = {"rabbitMqManagementPropertySource","rabbitConsumerConfig"})
//public class UnhandableQueues {
//
//	@RabbitListener(queues = "#{'${queuesToExclude}'.split(',')}")
//	public RemoteInvocationResult dropMessage(Object message, @Header(AmqpHeaders.CONSUMER_QUEUE) String queueName,
//			@Header(AmqpHeaders.REPLY_TO) String replyTo) {
//
//		//TODO: Should not log the whole message
//		log.warn(".dropMessage() [queueName={}, replyTo={}, Message will not be handled={}]", queueName, replyTo, message);
//
//		FaultMVRException faultMVRException = new FaultMVRException("Message will not be handled = " + message);
//
//		return new RemoteInvocationResult(faultMVRException);
//	}
//}
