package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.contract.dto.mapper.ApplicationToAuditEventTypeMapper;
import bg.bulsi.mvr.mpozei.gateway.api.v1.NaifApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.UUID;
import java.util.function.Function;

@Slf4j
@RequiredArgsConstructor
@Service
public class NaifApiDelegateService implements NaifApiDelegate {
    private final EventSender eventSender;

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_naif')")
    public Mono<HttpResponse> confirmPersoCentreApplication(PersoCentreConfirmApplicationRequest request, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                request,
                AuditEventType.CONFIRM_PERSO_APPLICATION,
                HttpResponse.class);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_naif')")
    public Mono<PersoCentreApplicationResponse> createPersoCentreApplication(PersoCentreApplicationRequest request, ServerWebExchange exchange) {
    	EventPayload eventPayloadReq = new EventPayload();
    	//TODO: Requester properties should be set from JWT
//		eventPayloadReq.setRequesterUid(request.getCitizenIdentifierNumber());
//		eventPayloadReq.setRequesterUidType(request.getCitizenIdentifierType().name());
//		eventPayloadReq.setRequesterName(request.getFirstName(), request.getSecondName(), request.getLastName());
//		eventPayloadReq.setIdCardNumber(request.getPersonalIdentityDocument().getIdentityNumber());
		eventPayloadReq.setApplicationType(request.getApplicationType().name());
		eventPayloadReq.setTargetUid(request.getCitizenIdentifierNumber());
		eventPayloadReq.setTargetName(request.getFirstName(), request.getSecondName(), request.getLastName());
		eventPayloadReq.setRequestBody(request.toString());
		
		Function<PersoCentreApplicationResponse, EventPayload> auditRespPayload = (appResp -> {
			EventPayload eventPayloadResp = new EventPayload();
			eventPayloadResp.setApplicationType(request.getApplicationType().name());
			eventPayloadResp.setTargetUid(request.getCitizenIdentifierNumber());
			eventPayloadResp.setTargetName(request.getFirstName(), request.getSecondName(), request.getLastName());
			
			return eventPayloadResp;
		});
		
		AuditEventType auditEventType = ApplicationToAuditEventTypeMapper.mapToAuditEventType(request.getApplicationType(), true);
    	
        return eventSender.send(
                exchange,
                request,
                auditEventType,
                PersoCentreApplicationResponse.class,
                eventPayloadReq,
                auditRespPayload);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_naif')")
    public Mono<NaifUpdateCertificateStatusResponse> updateCertificateStatusByNaif(NaifUpdateCertificateStatusDTO dto, ServerWebExchange exchange) {
        //dto.setMessageRefID(UUID.randomUUID());
        return eventSender.send(
                exchange,
                dto,
                AuditEventType.NAIF_UPDATE_CERTIFICATE_STATUS,
                NaifUpdateCertificateStatusResponse.class);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_naif')")
    public Mono<NaifDeliveredCertificateResponse> activateDeliveredCertificateByNaif(NaifDeliveredCertificateDTO dto, ServerWebExchange exchange) {
       // dto.setMessageRefID(UUID.randomUUID());
        return eventSender.send(
                exchange,
                dto,
                AuditEventType.ACTIVATE_DELIVERED_CERTIFICATE_BY_NAIF,
                NaifDeliveredCertificateResponse.class);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_naif')")
    public Mono<NaifInvalidateEidResponse> invalidateCitizenEidByNaif(InvalidateCitizenEidDTO dto, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                dto,
                AuditEventType.INVALIDATE_CITIZEN_EID_BY_NAIF,
                NaifInvalidateEidResponse.class);
    }

	/*
	 * @Override
	 * // @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_naif')") public
	 * Mono<NaifUpdateCitizenIdentifierResponse>
	 * updateCitizenIdentifierByNaif(UpdateCitizenIdentifierDTO dto,
	 * ServerWebExchange exchange) { return eventSender.send( exchange, dto,
	 * AuditEventType.UPDATE_CITIZEN_IDENTIFIER_BY_NAIF,
	 * NaifUpdateCitizenIdentifierResponse.class); }
	 */

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_naif')")
    public Mono<NaifDeviceHistoryResponse> getDeviceHistory(NaifDeviceHistoryRequest request, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                request,
                AuditEventType.NAIF_GET_DEVICE_HISTORY,
                NaifDeviceHistoryResponse.class);
    }
}
