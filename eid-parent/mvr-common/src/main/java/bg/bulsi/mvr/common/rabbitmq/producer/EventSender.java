package bg.bulsi.mvr.common.rabbitmq.producer;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.dto.AuditData;
import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.audit_logger.model.MessageType;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import lombok.extern.slf4j.Slf4j;

import java.util.Collections;
import java.util.Objects;
import java.util.function.Function;

import org.springframework.amqp.core.DirectExchange;
import org.springframework.amqp.rabbit.AsyncRabbitTemplate;
import org.springframework.amqp.rabbit.RabbitConverterFuture;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;

import reactor.core.publisher.Mono;

@Component
@Slf4j
public class EventSender {
	@Autowired
	private AsyncRabbitTemplate asyncRabbitTemplate;
	@Autowired
	private DirectExchange rpcExchange;
	@Autowired
	private BaseAuditLogger auditLogger;

	public <T> Mono<T> send(ServerWebExchange exchange, Object payload, AuditEventType eventType, Class<T> returnType) {
		return PreSendRabbitPostProcessor.create(eventType).flatMap(postProcessor -> this.processEvent(exchange,
				payload, postProcessor.getUserContext().getTargetUserId(), eventType, returnType, postProcessor, null, null));
	}

	public <T> Mono<T> send(ServerWebExchange exchange, Object payload, AuditEventType eventType, Class<T> returnType, EventPayload requestPayload, Function<T, EventPayload> responsePayload) {
		return PreSendRabbitPostProcessor.create(eventType).flatMap(postProcessor -> this.processEvent(exchange,
				payload, postProcessor.getUserContext().getTargetUserId(), eventType, returnType, postProcessor, requestPayload, responsePayload));
	}
	
	// TODO: check if this is needed in the future
	public <T> Mono<T> send(ServerWebExchange exchange, Object payload, String targetUserId, AuditEventType eventType,
			Class<T> returnType) {
		return PreSendRabbitPostProcessor.create(eventType).flatMap(
				postProcessor -> processEvent(exchange, payload, targetUserId, eventType, returnType, postProcessor, null, null));
	}

	private <T> Mono<? extends T> processEvent(ServerWebExchange exchange, Object payload, String targetUserId,
			AuditEventType eventType, Class<T> returnType, PreSendRabbitPostProcessor postProcessor,
			EventPayload requestPayload, Function<T, EventPayload> responsePayload) {
		RabbitConverterFuture<ExtendedRemoteInvocationResult> rabbitFuture = asyncRabbitTemplate.convertSendAndReceive(
				rpcExchange.getName(), exchange.getAttribute(RabbitControllerAdvice.ROUTING_KEY_NAME), payload,
				postProcessor);

		UserContext userContext = postProcessor.getUserContext();
		if(requestPayload != null) {
			requestPayload.setTargetUid(userContext.getCitizenIdentifier());
			requestPayload.setTargetUidType(userContext.getCitizenIdentifierType());
			requestPayload.setTargetName(userContext.getName());
		}
		
		AuditData requestAuditEvent = AuditData.builder()
					.correlationId(userContext.getGlobalCorrelationId().toString())
					.eventType(eventType)
					.messageType(MessageType.REQUEST)
					.requesterUserId(userContext.getRequesterUserId())
					.requesterSystemId(userContext.getSystemId())
					.requesterSystemName(userContext.getSystemName())
					.payload(requestPayload)
					.build();
					//.targetUserId(targetUserId != null ? targetUserId : userContext.getTargetUserId()).build();
			auditLogger.auditEvent(requestAuditEvent);

		//this has to be set in case of RabbitMQ inaccessibility
		exchange.getResponse().getHeaders().set(UserContext.USER_CONTEXT_KEY,
				UserContextHolder.toJson(userContext));
		
		//use separate field for store {@link AuditEventType} (i.e. not stored in {@link UserContext}), as it is only used for the current gateway
		exchange.getAttributes().put(AuditEventType.AUDIT_EVENT_TYPE_KEY, eventType);
		
		return Mono.fromFuture(rabbitFuture).flatMap(m -> {
			var rabbitUserContextJson = m.getMessageProperties().getHeader(UserContext.USER_CONTEXT_KEY);
			
			exchange.getResponse().getHeaders().set(UserContext.USER_CONTEXT_KEY,
					rabbitUserContextJson.toString());
			
			Mono<T> mono = Mono.empty();
			try {
				mono = Mono.just((returnType.cast(m.recreate())));
				auditLogger.auditEvent(AuditData.builder()
							.correlationId(userContext.getGlobalCorrelationId().toString())
							.eventType(eventType)
							.messageType(MessageType.SUCCESS)
							.requesterUserId(userContext.getRequesterUserId())
							.requesterSystemId(userContext.getSystemId())
							.requesterSystemName(userContext.getSystemName())
							//.targetUserId(requestAuditEvent.getTargetUserId() != null ? requestAuditEvent.getTargetUserId() : UserContextHolder.fromJson(rabbitUserContextJson.toString()).getTargetUserId())
							// TODO: to check the line bellow when exception is thrown ClassCastException
							.payload(responsePayload != null ? responsePayload.apply(returnType.cast(m.recreate())) : null)
							.build());
			} catch (Throwable e) {
				log.error(e.toString());
				mono = Mono.error(m.getException() == null ? e : m.getException());
			}
			
			return mono;
		});
	}
}
