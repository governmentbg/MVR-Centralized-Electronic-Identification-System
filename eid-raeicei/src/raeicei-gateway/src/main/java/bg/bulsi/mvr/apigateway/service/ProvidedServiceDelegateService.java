package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.EidServiceTypeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.HttpResponse;
import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceRequestDTO;
import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceResponseDTO;
import bg.bulsi.mvr.raeicei.gateway.api.v1.ProvidedServiceApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;

@Component
@Slf4j
@RequiredArgsConstructor
public class ProvidedServiceDelegateService implements ProvidedServiceApiDelegate{
    private final EventSender eventSender;

	@Override
	public Mono<ProvidedServiceResponseDTO> createProvidedService(ProvidedServiceRequestDTO providedServiceRequestDTO,
			ServerWebExchange exchange) {
		return eventSender.send(
				exchange,
				providedServiceRequestDTO,
				AuditEventType.CREATE_APROVIDED_SERVICE,
				ProvidedServiceResponseDTO.class);
	}

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

	@Override
	public Mono<ProvidedServiceResponseDTO> updateProvidedService(
			UUID id,
			ProvidedServiceRequestDTO providedServiceRequestDTO, 
			ServerWebExchange exchange) {
		log.info(".updateProvidedService() [RequestID={}, id={}, providedServiceRequestDTO={}]",exchange.getRequest().getId(), id, providedServiceRequestDTO);

		Map<String, Object> messageBody = new HashMap<>();
		messageBody.put("id", id);
		messageBody.put("providedServiceRequestDTO", providedServiceRequestDTO);
		
		return eventSender.send(
				exchange,
				messageBody,
				AuditEventType.UPDATE_PROVIDED_SERVICE,
				ProvidedServiceResponseDTO.class);
	   }

	 @Override
	 public Mono<ProvidedServiceResponseDTO> getProvidedServiceByApplicationType(EidServiceTypeDTO applicationType,
			ServerWebExchange exchange) {
			return eventSender.send(
					exchange,
					applicationType,
					AuditEventType.GET_PROVIDED_SERVICE_BY_APPLICATION_TYPE,
					ProvidedServiceResponseDTO.class);
	 }

	@Override
	public Mono<HttpResponse> deleteProvidedService(String id, ServerWebExchange exchange) {
		return eventSender.send(exchange, id, AuditEventType.DELETE_PROVIDED_SERVICE, HttpResponse.class);
	}
	
//    GET_PROVIDED_SERVICE_BY_ID,
//    GET_ALL_PROVIDED_SERVICE,
//    CREATE_APROVIDED_SERVICE,
//    UPDATE_PROVIDED_SERVICE,
	
//    @Override
//    public Mono<List<TariffDTO>> getAllTariffs(UUID eidAdministratorId, ServerWebExchange exchange) {
//            Mono<List> result = eventSender.send(
//                    exchange,
//                    eidAdministratorId,
//                    AuditEventType.GET_ALL_TARIFFS_BY_EID_ADMINISTRATOR_ID,
//                    List.class);
//            return result.map(e -> (List<TariffDTO>) e);
//        }
//
//    @Override
//    public Mono<TariffDTO> getTariffByDate(TariffDateDTO tariffDateDTO, ServerWebExchange exchange) {
//        return eventSender.send(exchange, tariffDateDTO, AuditEventType.GET_TARIFF_BY_DATE, TariffDTO.class);
//    }
    
}
