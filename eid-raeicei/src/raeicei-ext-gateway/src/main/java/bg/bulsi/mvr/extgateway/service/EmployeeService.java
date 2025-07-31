package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.EmployeeDTO;
import bg.bulsi.mvr.raeicei.extgateway.api.v1.EmployeeServiceApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

@Component
@Slf4j
@RequiredArgsConstructor
public class EmployeeService implements EmployeeServiceApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<EmployeeDTO> getEmployeeById(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.GET_EMPLOYEE_BY_ID, EmployeeDTO.class);
    }
}
