package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.CalculateTariff;
import bg.bulsi.mvr.raeicei.contract.dto.CalculateTariffDTO;
import bg.bulsi.mvr.raeicei.contract.dto.CalculateTariffResultDTO;
import bg.bulsi.mvr.raeicei.gateway.api.v1.CalculateTariffApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

@Component
@Slf4j
@RequiredArgsConstructor
public class CalculateTariffService implements CalculateTariffApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<CalculateTariffResultDTO> calculateTariff(CalculateTariffDTO calculateTariffDTO, ServerWebExchange exchange) {
    	CalculateTariff message = new CalculateTariff(calculateTariffDTO, false);
        return eventSender.send(exchange, message, AuditEventType.CALCULATE_TARIFF, CalculateTariffResultDTO.class);
    }
}
