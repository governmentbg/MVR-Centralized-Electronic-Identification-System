package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.mpozei.contract.dto.FindHelpPagesFilter;
import bg.bulsi.mvr.mpozei.contract.dto.HelpPageDTO;
import bg.bulsi.mvr.mpozei.contract.dto.MisepPaymentDTO;
//import bg.bulsi.mvr.mpozei.contract.dto.MisepPaymentStatusDTO;
import bg.bulsi.mvr.mpozei.extgateway.api.v1.MisepApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

@Slf4j
@Service
@RequiredArgsConstructor
public class PaymentApiDelegateService implements MisepApiDelegate {
    private final EventSender eventSender;

    @Override
    public Mono<List<MisepPaymentDTO>> getAllPayments(ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                new ArrayList<>(),
                AuditEventType.GET_ALL_PAYMENTS,
                List.class).map(e -> (List<MisepPaymentDTO>) e);
    }

	/*
	 * //TODO: this is wrong
	 * 
	 * @Override public Mono<MisepPaymentStatusDTO> getPaymentById(String
	 * paymentRequestId, String citizenProfileId, ServerWebExchange exchange) {
	 * return eventSender.send( exchange, Map.of("paymentRequestId",
	 * paymentRequestId, "citizenProfileId", citizenProfileId),
	 * AuditEventType.GET_PAYMENT_BY_ID, MisepPaymentStatusDTO.class); }
	 */
}
