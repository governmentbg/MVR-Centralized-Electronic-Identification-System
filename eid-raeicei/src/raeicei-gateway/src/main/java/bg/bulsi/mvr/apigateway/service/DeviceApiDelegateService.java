package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.gateway.api.v1.DeviceApiDelegate;
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
public class DeviceApiDelegateService implements DeviceApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<DeviceDTO> getDevice(UUID id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.GET_DEVICE_BY_ID, DeviceDTO.class);
    }

    @Override
    public Mono<List<DeviceDTO>> getDevices4AdministratorId(UUID aeiId, ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                aeiId,
                AuditEventType.GET_AEI_DEVICES,
                List.class);
        return result.map(e -> new ArrayList<DeviceDTO>(e));
    }

    @Override
    public Mono<List<DeviceDTO>> getAllDevices(ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                new HashMap<>(),
                AuditEventType.GET_ALL_DEVICES,
                List.class);
        return result.map(e -> new ArrayList<DeviceDTO>(e));
    }

    @Override
    public Mono<List<DeviceDTO>> getDevicesByType(String type, ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                type,
                AuditEventType.GET_DEVICES_BY_TYPE,
                List.class);
        return result.map(e -> new ArrayList<DeviceDTO>(e));
    }
}
