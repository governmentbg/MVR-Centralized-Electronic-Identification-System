package bg.bulsi.backend.rabbitmq;

import bg.bulsi.backend.service.CitizenIdentifierService;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.rabbitmq.consumer.DefaultRabbitListener;
import bg.bulsi.mvr.common.util.ValidationUtil;
import bg.bulsi.mvr.reicontract.dto.*;
import bg.bulsi.reimodel.model.IdentifierType;
import bg.bulsi.reimodel.repository.view.CitizenIdentifierView;
import jakarta.validation.Valid;
import lombok.AllArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.amqp.support.converter.RemoteInvocationResult;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.stereotype.Component;

import java.util.List;
import java.util.Map;
import java.util.UUID;

@Slf4j
@Component
@AllArgsConstructor
public class ListenerDispatcher {
	private final ValidationUtil validationUtil;
	private final CitizenIdentifierService citizenIdentifierService;

	@DefaultRabbitListener(queues = "rei.rpcqueue.POST.api.v1.eidentity")
	public RemoteInvocationResult createCitizenIdentifier(@Valid @Payload CitizenDataDTO citizenDataDTO) {
		UUID eid = citizenIdentifierService.create(citizenDataDTO).getEidentityId();

		RemoteInvocationResult result = new RemoteInvocationResult(eid);
		
		UserContextHolder.getFromServletContext().setTargetUserId(eid);
		
		log.info(".createCitizenIdentifier() [eid={}]", eid);
		
		return result;
	}

	@DefaultRabbitListener(queues = "rei.rpcqueue.PUT.api.v1.eidentity.id")
	public RemoteInvocationResult updateCitizenIdentifier(Map<String, Object> inputMessage) {
		UUID id = (UUID) inputMessage.get("id");
		CitizenDataDTO citizenDataDTO = (CitizenDataDTO) inputMessage.get("citizenDataDTO");
		validationUtil.checkValid(citizenDataDTO);

		CitizenIdentifierView citizenIdentifierView = citizenIdentifierService.update(id, citizenDataDTO);

		EidentityDTO eIdentityDTO = new EidentityDTO(
				citizenIdentifierView.getEidentityId(),
				citizenIdentifierView.getActive(),
				citizenIdentifierView.getFirstName(),
				citizenIdentifierView.getSecondName(),
				citizenIdentifierView.getLastName(),
				citizenIdentifierView.getNumber(),
				IdentifierTypeDTO.fromValue(citizenIdentifierView.getType().name()));

		RemoteInvocationResult result = new RemoteInvocationResult(eIdentityDTO);

		log.info(".updateCitizenIdentifier() [result={}]", result);
		return result;
	}

	@DefaultRabbitListener(queues = "rei.rpcqueue.GET.api.v1.eidentity.id")
	public RemoteInvocationResult getCitizenIdentifier(UUID id) {
		CitizenIdentifierView citizenIdentifierView = citizenIdentifierService.getByEidentityId(id);

		EidentityDTO eIdentityDTO = new EidentityDTO(
				citizenIdentifierView.getEidentityId(),
				citizenIdentifierView.getActive(),
				citizenIdentifierView.getFirstName(),
				citizenIdentifierView.getSecondName(),
				citizenIdentifierView.getLastName(),
				citizenIdentifierView.getNumber(),
				IdentifierTypeDTO.fromValue(citizenIdentifierView.getType().name()));

		RemoteInvocationResult result = new RemoteInvocationResult(eIdentityDTO);

		UserContextHolder.getFromServletContext().setTargetUserId(eIdentityDTO.getId());
		
		log.info(".getCitizenIdentifier() [result={}]", result);
		return result;
	}

	@DefaultRabbitListener(queues = "rei.rpcqueue.GET.api.v1.eidentity.id.internal")
	public RemoteInvocationResult getCitizenIdentifierInternal(UUID id) {
		CitizenIdentifierView citizenIdentifierView = citizenIdentifierService.getByEidentityIdInternal(id);

		EidentityDTO eIdentityDTO = new EidentityDTO(
				citizenIdentifierView.getEidentityId(),
				citizenIdentifierView.getActive(),
				citizenIdentifierView.getFirstName(),
				citizenIdentifierView.getSecondName(),
				citizenIdentifierView.getLastName(),
				citizenIdentifierView.getNumber(),
				IdentifierTypeDTO.fromValue(citizenIdentifierView.getType().name()));

		RemoteInvocationResult result = new RemoteInvocationResult(eIdentityDTO);

		UserContextHolder.getFromServletContext().setTargetUserId(eIdentityDTO.getId());

		log.info(".getCitizenIdentifier() [result={}]", result);
		return result;
	}

	@DefaultRabbitListener(queues = "rei.rpcqueue.PUT.api.v1.eidentity.id.status")
	public RemoteInvocationResult updateCitizenIdentifierActive(Map<String, Object> inputMessage) {
		UUID id = (UUID) inputMessage.get("id");
		Boolean isActive = (Boolean) inputMessage.get("isActive");

		EidentityDTO eidentityDTO = citizenIdentifierService.updateActiveByEidentityId(id, isActive);


		RemoteInvocationResult result = new RemoteInvocationResult(eidentityDTO);

		log.info(".updateCitizenIdentifierActive() [result={}]", result);
		return result;
	}


	@DefaultRabbitListener(queues = "rei.rpcqueue.GET.api.v1.eidentity")
	public RemoteInvocationResult findCitizenIdentityByNumberAndType(Map<String, String> payload) {
		String number = payload.get("citizenIdentifierNumber");
		IdentifierType type = IdentifierType.valueOf(payload.get("citizenIdentifierType"));
		EidentityDTO result = citizenIdentifierService.findByNumberAndType(number, type);
		
		UserContextHolder.getFromServletContext().setTargetUserId(result.getId());
		
		return new RemoteInvocationResult(result);
	}

	@DefaultRabbitListener(queues = "rei.rpcqueue.GET.api.v1.eidentity.internal")
	public RemoteInvocationResult findCitizenIdentityByNumberAndTypeInternal(Map<String, String> payload) {
		String number = payload.get("citizenIdentifierNumber");
		IdentifierType type = IdentifierType.valueOf(payload.get("citizenIdentifierType"));
		EidentityDTO result = citizenIdentifierService.findByNumberAndTypeInternal(number, type);

		UserContextHolder.getFromServletContext().setTargetUserId(result.getId());

		return new RemoteInvocationResult(result);
	}

	@DefaultRabbitListener(queues = "rei.rpcqueue.POST.api.v1.eidentity.list")
	public RemoteInvocationResult getEidentitiesByNumberAndType(@Payload FindEidentitiesRequestDTO request) {
		List<EidentityDTO> eidentities = citizenIdentifierService.findAllByNumberAndType(request.getEidentities());
		return new RemoteInvocationResult(eidentities);
	}

	@DefaultRabbitListener(queues = "rei.rpcqueue.PUT.api.v1.eidentity.update-citizen-identifier")
	public RemoteInvocationResult updateCitizenIdentifier(@Payload UpdateCitizenIdentifierDTO dto) {
		citizenIdentifierService.updateCitizenIdentifier(dto);
		return new RemoteInvocationResult(HttpResponseDTO.builder().message("OK").statusCode(200).build());
	}
}
