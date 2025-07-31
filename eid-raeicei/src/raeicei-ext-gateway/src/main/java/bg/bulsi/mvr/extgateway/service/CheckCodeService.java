package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.HttpResponse;
import bg.bulsi.mvr.raeicei.extgateway.api.v1.CheckCodeApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.HashMap;

@Component
@Slf4j
@RequiredArgsConstructor
public class CheckCodeService implements CheckCodeApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<HttpResponse> checkCode(String code, Boolean isOffice, ServerWebExchange exchange) {
        return eventSender.send(exchange, buildPayload(code, isOffice), AuditEventType.CHECK_CODE, HttpResponse.class);
    }

    public HashMap<String, Object> buildPayload(String code, Boolean isOffice) {
        HashMap<String, Object> payload = new HashMap<>(2);
        payload.put("code", code);
        payload.put("isOffice", isOffice);

        return payload;
    }
}
