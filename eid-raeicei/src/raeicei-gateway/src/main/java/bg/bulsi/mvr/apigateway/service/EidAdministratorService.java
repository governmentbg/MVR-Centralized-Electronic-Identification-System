package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorHistoryDTO;
import bg.bulsi.mvr.raeicei.gateway.api.v1.EidAdministratorApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.UUID;

@Component
@Slf4j
@RequiredArgsConstructor
public class EidAdministratorService implements EidAdministratorApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<List<EidAdministratorDTO>> getAllEidAdministrators(ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                new HashMap<>(),
                AuditEventType.GET_ALL_EID_ADMINISTRATORS,
                List.class);
        return result.map(e -> new ArrayList<EidAdministratorDTO>(e));
    }

    @Override
    public Mono<List<EidAdministratorHistoryDTO>> getAllEidAdministratorsHistory(Boolean stoppedIncluded, ServerWebExchange exchange) {

        if (stoppedIncluded == null) {
            stoppedIncluded = false;
        }

        Mono<List> result = eventSender.send(
                exchange,
                stoppedIncluded,
                AuditEventType.GET_ALL_EID_ADMINISTRATORS_HISTORY,
                List.class);
        return result.map(e -> new ArrayList<EidAdministratorHistoryDTO>(e));
    }

    @Override
    public Mono<EidAdministratorDTO> getEidAdministrator(UUID id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.GET_EID_ADMINISTRATOR_BY_ID, EidAdministratorDTO.class);
    }

    @Override
    public Mono<EidAdministratorDTO> createEidAdministrator(EidAdministratorDTO eidAdministratorDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, eidAdministratorDTO, AuditEventType.CREATE_EID_ADMINISTRATOR, EidAdministratorDTO.class);
    }

    @Override
    public Mono<EidAdministratorDTO> updateEidAdministrator(String id, EidAdministratorDTO eidAdministratorDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, eidAdministratorDTO, AuditEventType.UPDATE_EID_ADMINISTRATOR, EidAdministratorDTO.class);
    }

}
