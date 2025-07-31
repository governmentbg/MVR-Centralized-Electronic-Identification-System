package bg.bulsi.mvr.common.rabbitmq.consumer;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.ObjectInputStream;

import org.springframework.amqp.AmqpException;
import org.springframework.amqp.AmqpRejectAndDontRequeueException;
import org.springframework.amqp.core.Message;
import org.springframework.amqp.core.MessagePostProcessor;
import org.springframework.stereotype.Component;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;

import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Component
public class PreReceiveRabbitPostProcessor implements MessagePostProcessor {

    @Override
    public Message postProcessMessage(Message message) throws AmqpException {
	    try {
	        constructUserContextFromHeader(message);
	        logEventDetails(message);
	        return message;
		} catch(Exception ex) {
	        throw new AmqpRejectAndDontRequeueException("Error processing message: " + ex.getMessage(), ex);
		}
    }

    private void constructUserContextFromHeader(Message message) {
        Object json = message.getMessageProperties().getHeader(UserContext.USER_CONTEXT_KEY);
        UserContext userContext = UserContextHolder.fromJson(json.toString().getBytes());
        UserContextHolder.setToServletContext(userContext);
    }

    private void logEventDetails(Message message) {
        String queue = message.getMessageProperties().getConsumerQueue();
        String replyTo = message.getMessageProperties().getReplyTo();
//        String auditEventType = message.getMessageProperties().getHeader(AuditEventType.AUDIT_EVENT_TYPE_KEY);
        String body = deserializeAndConvertToJSON(message.getBody());
        log.info("[queueName={}, replyTo={}, message={}]", queue, replyTo, body);

//        UserContext userContext = UserContextHolder.getFromServletContext();
        
//        auditLogger.auditEvent(AuditData.builder()
//                .correlationId(userContext.getGlobalCorrelationId().toString())
//                .eventType(AuditEventType.valueOf(auditEventType))
//                .messageType(MessageType.PROCESSING)
//		        .requesterUserId(userContext.getRequesterUserId())
//		        .requesterSystemId(userContext.getSupplierId())
//		        .requesterSystemName(userContext.getSupplierName())
//		        .targetUserId(userContext.getTargetUserId())
//                .payload(body)
//                .build());
    }

    private String deserializeAndConvertToJSON(byte[] bytes)  {
        try (ByteArrayInputStream byteArrayInputStream = new ByteArrayInputStream(bytes);
             ObjectInputStream objectInputStream = new ObjectInputStream(byteArrayInputStream)) {
            ObjectMapper objectMapper = new ObjectMapper();
            objectMapper.registerModule(new JavaTimeModule());
            return objectMapper.writeValueAsString(objectInputStream.readObject());
        } catch (IOException | ClassNotFoundException ex) {
            log.error("Cannot deserialize message body \n" + ex);
        }
        return null;
    }
}
