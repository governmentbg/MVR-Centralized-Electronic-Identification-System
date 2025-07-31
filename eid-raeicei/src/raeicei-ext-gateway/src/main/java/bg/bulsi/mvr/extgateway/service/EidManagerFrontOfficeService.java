package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeResponseDTO;
import bg.bulsi.mvr.raeicei.extgateway.api.v1.EidManagerFrontOfficeApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Component
@Slf4j
@RequiredArgsConstructor
public class EidManagerFrontOfficeService implements EidManagerFrontOfficeApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<EidManagerFrontOfficeResponseDTO> getEidManagerFrontOffice(UUID id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.GET_EID_MANAGER_FRONT_OFFICE_BY_ID, EidManagerFrontOfficeResponseDTO.class);
    }

    @Override
    public Mono<List<EidManagerFrontOfficeDTO>> getAllEidManagerFrontOffices(UUID eidAdministratorId, ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                eidAdministratorId,
                AuditEventType.GET_ALL_EID_MANAGER_FRONT_OFFICES_BY_MANAGER_ID,
                List.class);
        return result.map(e -> new ArrayList<EidManagerFrontOfficeDTO>(e));
    }

    @Override
    public Mono<EidManagerFrontOfficeDTO> getEidManagerFrontOfficeByName(String name, ServerWebExchange exchange) {
        return eventSender.send(exchange, name, AuditEventType.GET_EID_MANAGER_FRONT_OFFICE_BY_NAME, EidManagerFrontOfficeDTO.class);
    }
}
