package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.EidCenterDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidCenterHistoryDTO;
import bg.bulsi.mvr.raeicei.extgateway.api.v1.EidCenterApiDelegate;
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
public class EidCenterService implements EidCenterApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<List<EidCenterDTO>> getAllEidCenters(ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                new HashMap<>(),
                AuditEventType.GET_ALL_EID_CENTERS,
                List.class);
        return result.map(e -> new ArrayList<EidCenterDTO>(e));
    }

    @Override
    public Mono<List<EidCenterHistoryDTO>> getAllEidCentersHistory(Boolean stoppedIncluded, ServerWebExchange exchange) {

        if (stoppedIncluded == null) {
            stoppedIncluded = false;
        }

        Mono<List> result = eventSender.send(
                exchange,
                stoppedIncluded,
                AuditEventType.GET_ALL_EID_CENTERS_HISTORY,
                List.class);
        return result.map(e -> new ArrayList<EidCenterHistoryDTO>(e));
    }

    @Override
    public Mono<EidCenterDTO> getEidCenter(UUID id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.GET_EID_CENTER_BY_ID, EidCenterDTO.class);

    }
}
