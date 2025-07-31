package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.TariffDateDTO;
import bg.bulsi.mvr.raeicei.contract.dto.TariffDoubleCurrencyResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.TariffResponseDTO;
import bg.bulsi.mvr.raeicei.extgateway.api.v1.TariffApiDelegate;
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
public class TariffService implements TariffApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<List<TariffDoubleCurrencyResponseDTO>> getAllTariffs(UUID eidAdministratorId, ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                eidAdministratorId,
                AuditEventType.GET_ALL_TARIFFS_BY_EID_ADMINISTRATOR_ID,
                List.class);
        return result.map(e -> (List<TariffDoubleCurrencyResponseDTO>) e);
    }

    @Override
    public Mono<TariffDoubleCurrencyResponseDTO> getTariffByDate(TariffDateDTO tariffDateDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, tariffDateDTO, AuditEventType.GET_TARIFF_BY_DATE, TariffDoubleCurrencyResponseDTO.class);
    }

    @Override
    public Mono<TariffDoubleCurrencyResponseDTO> getTariffById(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.GET_TARIFF_BY_ID, TariffDoubleCurrencyResponseDTO.class);
    }
}
