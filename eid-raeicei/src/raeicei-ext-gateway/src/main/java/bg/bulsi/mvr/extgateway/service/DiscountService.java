package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.DiscountDateDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DiscountDoubleCurrencyResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DiscountResponseDTO;
import bg.bulsi.mvr.raeicei.extgateway.api.v1.DiscountApiDelegate;
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
public class DiscountService implements DiscountApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<List<DiscountResponseDTO>> getAllDiscounts(UUID eidManagerId, ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                eidManagerId,
                AuditEventType.GET_ALL_DISCOUNTS_BY_EID_ADMINISTRATOR_ID,
                List.class);
        return result.map(e -> (List<DiscountResponseDTO>) e);
    }

    @Override
    public Mono<DiscountResponseDTO> getDiscountByDate(DiscountDateDTO discountDateDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, discountDateDTO, AuditEventType.GET_DISCOUNT_BY_DATE, DiscountResponseDTO.class);
    }

    @Override
    public Mono<DiscountResponseDTO> getDiscountById(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.GET_DISCOUNT_BY_ID, DiscountResponseDTO.class);
    }
}
