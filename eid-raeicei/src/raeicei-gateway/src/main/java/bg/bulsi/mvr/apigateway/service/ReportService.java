package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.gateway.api.v1.ReportApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.List;

@Component
@Slf4j
@RequiredArgsConstructor
public class ReportService implements ReportApiDelegate {
    private final EventSender eventSender;

    @Override
    public Mono<List<Object>> getEidAdministratorsReport(ServerWebExchange exchange) {
        return eventSender.send(exchange, "", AuditEventType.REPORT_OF_EID_ADMINISTRATORS, List.class).map(e -> List.copyOf((List<List<String>>) e));
    }

    @Override
    public Mono<List<Object>> getEidCentersReport(ServerWebExchange exchange) {
        return eventSender.send(exchange, "", AuditEventType.REPORT_OF_EID_CENTERS, List.class).map(e -> List.copyOf((List<List<String>>) e));
    }
}
