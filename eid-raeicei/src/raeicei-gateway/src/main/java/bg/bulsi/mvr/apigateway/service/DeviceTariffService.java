package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceTariffDTO;
import bg.bulsi.mvr.raeicei.gateway.api.v1.DeviceTariffApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.List;
import java.util.UUID;

@Component
@Slf4j
@RequiredArgsConstructor
public class DeviceTariffService implements DeviceTariffApiDelegate {

    private final EventSender eventSender;
    @Override
    public Mono<DeviceTariffDTO> createDeviceTariff(DeviceTariffDTO deviceTariffDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, deviceTariffDTO, AuditEventType.CREATE_DEVICE_TARIFF,DeviceTariffDTO.class);
    }

    @Override
    public Mono<DeviceTariffDTO> getDeviceTariffByDate(DeviceTariffDTO deviceTariffDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, deviceTariffDTO, AuditEventType.GET_DEVICE_TARIFF_BY_DATE,DeviceTariffDTO.class);
    }

    @Override
    public Mono<List<DeviceTariffDTO>> getAllDeviceTariffs(UUID eidAdministratorId, ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                eidAdministratorId,
                AuditEventType.GET_ALL_DEVICE_TARIFFS_BY_EID_ADMINISTRATOR_ID,
                List.class);
        return result.map(e -> (List<DeviceTariffDTO>) e);
    }
}
