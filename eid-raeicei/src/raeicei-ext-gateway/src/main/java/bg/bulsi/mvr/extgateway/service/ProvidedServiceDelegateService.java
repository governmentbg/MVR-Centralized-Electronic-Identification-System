package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceResponseDTO;
import bg.bulsi.mvr.raeicei.extgateway.api.v1.ProvidedServiceApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.HashMap;
import java.util.List;
import java.util.UUID;

@Component
@Slf4j
@RequiredArgsConstructor
public class ProvidedServiceDelegateService implements ProvidedServiceApiDelegate{
    private final EventSender eventSender;

	@Override
	public Mono<List<ProvidedServiceResponseDTO>> getAllProvidedServices(ServerWebExchange exchange) {
		
        Mono<List> result = eventSender.send(
                exchange,
                new HashMap<>(),
                AuditEventType.GET_ALL_PROVIDED_SERVICE,
                List.class);
        return result.map(e -> (List<ProvidedServiceResponseDTO>) e);
	}

	@Override
	public Mono<ProvidedServiceResponseDTO> getProvidedServiceById(UUID id, ServerWebExchange exchange) {
		return eventSender.send(
				exchange,
				id,
				AuditEventType.GET_PROVIDED_SERVICE_BY_ID,
				ProvidedServiceResponseDTO.class);
	}
}
