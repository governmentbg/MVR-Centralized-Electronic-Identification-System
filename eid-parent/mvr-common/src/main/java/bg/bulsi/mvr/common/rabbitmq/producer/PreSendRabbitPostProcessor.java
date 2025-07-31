package bg.bulsi.mvr.common.rabbitmq.producer;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.extern.slf4j.Slf4j;
import org.springframework.amqp.AmqpException;
import org.springframework.amqp.core.Message;
import org.springframework.amqp.core.MessagePostProcessor;
import reactor.core.publisher.Mono;

import java.util.Objects;

@Getter
@AllArgsConstructor
@Slf4j
public class PreSendRabbitPostProcessor implements MessagePostProcessor {
    private UserContext userContext;
    private AuditEventType eventType;

    public PreSendRabbitPostProcessor(UserContext userContext) {
        this.userContext = userContext;
    }

    public static Mono<PreSendRabbitPostProcessor> create(AuditEventType eventType) {
        return UserContextHolder.getFromReactiveContext()
                .map(user -> new PreSendRabbitPostProcessor(user, eventType));
    }

    public static Mono<PreSendRabbitPostProcessor> create() {
        return UserContextHolder.getFromReactiveContext()
                .map(PreSendRabbitPostProcessor::new);
    }

    @Override
    public Message postProcessMessage(Message message) throws AmqpException {
        setUserContextHeader(message);
        if (Objects.nonNull(eventType)) {
            setAuditEventTypeHeader(message);
        }
        return message;
    }

    private void setAuditEventTypeHeader(Message message) {
        message.getMessageProperties().setHeader(AuditEventType.AUDIT_EVENT_TYPE_KEY, eventType.name());
    }

    private void setUserContextHeader(Message message) {
        try {
            String json = new ObjectMapper().writeValueAsString(this.userContext);
            message.getMessageProperties().setHeader(UserContext.USER_CONTEXT_KEY, json);
        } catch (JsonProcessingException e) {
            log.error("Cannot convert UserContext to json \n" + e);
        }
    }


}
