package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.RegionDTO;
import bg.bulsi.mvr.raeicei.gateway.api.v1.RegionApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.HashMap;
import java.util.List;

@Component
@Slf4j
@RequiredArgsConstructor
public class RegionService implements RegionApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<List<RegionDTO>> regionGetAll(ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                new HashMap<>(),
                AuditEventType.GET_ALL_REGIONS,
                List.class);
        return result.map(e -> (List<RegionDTO>) e);
    }
}