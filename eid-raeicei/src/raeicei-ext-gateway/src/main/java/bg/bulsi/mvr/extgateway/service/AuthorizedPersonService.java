package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.ContactDTO;
import bg.bulsi.mvr.raeicei.extgateway.api.v1.AuthorizedPersonServiceApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

@Component
@Slf4j
@RequiredArgsConstructor
public class AuthorizedPersonService implements AuthorizedPersonServiceApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<ContactDTO> getAuthorizedPersonById(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.GET_AUTHORIZED_PERSON_BY_ID, ContactDTO.class);
    }
}
