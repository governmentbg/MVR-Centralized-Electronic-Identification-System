package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.apigateway.api.v1.EIdentityApiDelegate;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.common.rabbitmq.producer.RabbitControllerAdvice;
import bg.bulsi.mvr.reicontract.dto.*;
import lombok.extern.slf4j.Slf4j;

import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;

@Slf4j
@Service
public class EIdentityApiDelegateService implements EIdentityApiDelegate {
	@Autowired
	private EventSender eventSender;

	@Override
	public Mono<EidentityDTO> getEidentityById(UUID id, ServerWebExchange exchange) {
		log.info(".eidentityIdGet() [RequestID={}, id={}]",exchange.getRequest().getId(), id);
		return eventSender.send(
				exchange,
				id,
				AuditEventType.GET_EIDENTITY_BY_ID,
				EidentityDTO.class);
	}

	@Override
	public Mono<EidentityDTO> updateEidentity(UUID id, CitizenDataDTO citizenDataDTO, ServerWebExchange exchange) {
		log.info(".eidentityIdPut() [RequestID={}, id={}, citizenDataDTO={}]", exchange.getRequest().getId(), id, citizenDataDTO);

		Map<String, Object> messageBody = new HashMap<>();
		messageBody.put("id", id);
		messageBody.put("citizenDataDTO", citizenDataDTO);

		return eventSender.send(
				exchange,
				messageBody,
				id.toString(),
				AuditEventType.UPDATE_EIDENTITY,
				EidentityDTO.class);
	}

	@Override
	public Mono<EidentityDTO> updateEidentityActive(UUID id, Boolean isActive, ServerWebExchange exchange) {
		log.info(".updateEidentityActive() [RequestID={}, id={}, isActive={}]", exchange.getRequest().getId(), id, isActive);

		Map<String, Object> messageBody = new HashMap<>();
		messageBody.put("id", id);
		messageBody.put("isActive", isActive);

		return eventSender.send(
				exchange,
				messageBody,
				id.toString(),
				AuditEventType.UPDATE_EIDENTITY_ACTIVE,
				EidentityDTO.class);
	}

	@Override
	public Mono<UUID> createEidentity(CitizenDataDTO citizenDataDTO, ServerWebExchange exchange) {
		log.info(".eidentityPost() [RequestID={}, citizenDataDTO={}]",exchange.getRequest().getId(), citizenDataDTO);

		Mono<UUID> result = eventSender.send(
				exchange,
				citizenDataDTO,
				AuditEventType.CREATE_EIDENTITY,
				UUID.class);
				
		return result;
	}

	@Override
	public Mono<EidentityDTO> findEidentityByNumberAndType(String number, IdentifierTypeDTO type, ServerWebExchange exchange) {
		log.info(".findEidentityByNumberAndType() [RequestID={}]",exchange.getRequest().getId());
		Map<String, String> payload = new HashMap<>();
		payload.put("citizenIdentifierNumber", number);
		payload.put("citizenIdentifierType", type.name());
		return eventSender.send(
				exchange,
				payload,
				AuditEventType.CREATE_EIDENTITY,
				EidentityDTO.class);
	}

	@Override
	public Mono<List<EidentityDTO>> getEidentitiesByNumberAndType(FindEidentitiesRequestDTO request, ServerWebExchange exchange) {
		log.info(".getEidentitiesByIds() [RequestID={}]",exchange.getRequest().getId());
		Mono<List> list = eventSender.send(
				exchange,
				request,
				AuditEventType.GET_EIDENTITIES_BY_NUMBER_AND_TYPE,
				List.class);
		return list.map(e -> (List<EidentityDTO>) e);
	}

	@Override
	public Mono<EidentityDTO> findEidentityByNumberAndTypeInternal(String number, IdentifierTypeDTO type, ServerWebExchange exchange) {
		log.info(".findEidentityByNumberAndTypeInternal() [RequestID={}]",exchange.getRequest().getId());
		Map<String, String> payload = new HashMap<>();
		payload.put("citizenIdentifierNumber", number);
		payload.put("citizenIdentifierType", type.name());
		return eventSender.send(
				exchange,
				payload,
				AuditEventType.CREATE_EIDENTITY,
				EidentityDTO.class);
	}

	@Override
	public Mono<EidentityDTO> getEidentityByIdInternal(UUID id, ServerWebExchange exchange) {
		log.info(".eidentityIdGetInternal() [RequestID={}, id={}]",exchange.getRequest().getId(), id);
		return eventSender.send(
				exchange,
				id,
				AuditEventType.GET_EIDENTITY_BY_ID,
				EidentityDTO.class);
	}

	@Override
	public Mono<HttpResponseDTO> updateCitizenIdentifier(UpdateCitizenIdentifierDTO dto, ServerWebExchange exchange) {
		return eventSender.send(
				exchange,
				dto,
				AuditEventType.UPDATE_CITIZEN_IDENTIFIER_BY_NAIF,
				HttpResponseDTO.class);
	}
}
