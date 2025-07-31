package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.common.util.ValidationUtil;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.extgateway.api.v1.CertificateApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.time.LocalDate;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.UUID;
import java.util.function.Function;

import static bg.bulsi.mvr.common.exception.ErrorCode.USER_ID_OR_CITIZEN_PROFILE_ID_MUST_BE_PROVIDED;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertFalse;

@Component
@Slf4j
@RequiredArgsConstructor
public class CertificateApiDelegateService implements CertificateApiDelegate {
    public static final List<String> CERTIFICATE_SORT_FIELDS = List.of("id", "validityFrom", "validityUntil", "deviceId", "serialNumber", "status", "alias");

    private final EventSender eventSender;

    @Override
    public Mono<CitizenCertificateSummaryResponse> getCitizenCertificateById(UUID id, ServerWebExchange exchange) {
    	return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {

			EventPayload eventPayloadReq = new EventPayload();
			eventPayloadReq.setTargetUid(userContext.getCitizenIdentifier());
			eventPayloadReq.setTargetUidType(userContext.getCitizenIdentifierType());
			eventPayloadReq.setTargetName(userContext.getName());
			eventPayloadReq.setCertificateId(id.toString());
	
			Function<CitizenCertificateSummaryResponse, EventPayload> auditRespPayload = (appResp -> {
				EventPayload eventPayloadResp = new EventPayload();
				eventPayloadResp.setTargetUid(userContext.getCitizenIdentifier());
				eventPayloadResp.setTargetUidType(userContext.getCitizenIdentifierType());
				eventPayloadResp.setTargetName(userContext.getName());
				eventPayloadResp.setCertificateId(id.toString());
	
				return eventPayloadResp;
			});
    	
    	return eventSender.send(
                exchange,
                id,
                AuditEventType.GET_CITIZEN_CERTIFICATE_BY_ID,
                CitizenCertificateSummaryResponse.class,
                eventPayloadReq,
                auditRespPayload);
    	});
    }

    @Override
    public Mono<Page<FindCertificateResponse>> findCitizenCertificates(String id, String serialNumber, List<CertificateStatusExternal> statuses, LocalDate validityFrom, LocalDate validityUntil, List<UUID> deviceIds, UUID eidAdministratorId, String alias, ServerWebExchange exchange, Pageable pageable) {
        Pageable allowedPageable = ValidationUtil.filterAllowedPageableSort(pageable, CERTIFICATE_SORT_FIELDS);
        return UserContextHolder.getFromReactiveContext()
                .flatMap(context -> {
                    assertFalse(context.getEidentityId() == null && context.getCitizenProfileId() == null, USER_ID_OR_CITIZEN_PROFILE_ID_MUST_BE_PROVIDED);

                    UUID eidentityId = Objects.isNull(context.getEidentityId()) ? null : UUID.fromString(context.getEidentityId());
                    UUID citizenProfileId = Objects.isNull(context.getCitizenProfileId()) ? null : UUID.fromString(context.getCitizenProfileId());

                    List<CertificateStatus> internalStatuses = statuses == null ? null : statuses.stream().map(e -> CertificateStatus.valueOf(e.name())).toList();

                    CitizenCertificateFilter filter = new CitizenCertificateFilter(eidentityId, citizenProfileId, id, serialNumber, internalStatuses, validityFrom, validityUntil, deviceIds, alias, true, allowedPageable);

                    Mono<Page> result = eventSender.send(
                            exchange,
                            filter,
                            AuditEventType.FIND_CITIZEN_CERTIFICATES,
                            Page.class);
                    return result.map(e -> (Page<FindCertificateResponse>) e);
                });
    }

    @Override
    public Mono<CertificateResponse> certificateEnrollWithBasicProfile(MobileCertificateBasicProfileRequest request, ServerWebExchange exchange) {
    	return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {

			EventPayload eventPayloadReq = new EventPayload();
			eventPayloadReq.setTargetUid(userContext.getCitizenIdentifier());
			eventPayloadReq.setTargetUidType(userContext.getCitizenIdentifierType());
			eventPayloadReq.setTargetName(userContext.getName());
			eventPayloadReq.setRequestBody(request);
	
			Function<CertificateResponse, EventPayload> auditRespPayload = (appResp -> {
				EventPayload eventPayloadResp = new EventPayload();
				eventPayloadResp.setTargetUid(userContext.getCitizenIdentifier());
				eventPayloadResp.setTargetUidType(userContext.getCitizenIdentifierType());
				eventPayloadResp.setTargetName(userContext.getName());
				eventPayloadResp.setCertificateId(appResp.getId().toString());
				
				return eventPayloadResp;
			});
    	
    	return eventSender.send(
                exchange,
                request,
                AuditEventType.CREATE_CITIZEN_CERTIFICATE,
                CertificateResponse.class,
                eventPayloadReq,
                auditRespPayload);
    	});
    }

    @Override
    public Mono<CertificateResponse> certificateEnrollWithEid(MobileCertificateEidRequest request, ServerWebExchange exchange) {
    	return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {

			EventPayload eventPayloadReq = new EventPayload();
			eventPayloadReq.setTargetUid(userContext.getCitizenIdentifier());
			eventPayloadReq.setTargetUidType(userContext.getCitizenIdentifierType());
			eventPayloadReq.setTargetName(userContext.getName());
			eventPayloadReq.setRequestBody(request);
	
			Function<CertificateResponse, EventPayload> auditRespPayload = (appResp -> {
				EventPayload eventPayloadResp = new EventPayload();
				eventPayloadResp.setTargetUid(userContext.getCitizenIdentifier());
				eventPayloadResp.setTargetUidType(userContext.getCitizenIdentifierType());
				eventPayloadResp.setTargetName(userContext.getName());
				eventPayloadResp.setCertificateId(appResp.getId().toString());
				
				return eventPayloadResp;
			});
    	
    	return eventSender.send(
                exchange,
                request,
                AuditEventType.CREATE_CITIZEN_CERTIFICATE,
                CertificateResponse.class,
                eventPayloadReq,
                auditRespPayload);
    	});
    }

    @Override
    public Mono<ApplicationStatus> confirmCertificateStoredWithBasicProfile(CertificateBasicProfileConfirmationRequest request, ServerWebExchange exchange) {
    	return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {

			EventPayload eventPayloadReq = new EventPayload();
			eventPayloadReq.setTargetUid(userContext.getCitizenIdentifier());
			eventPayloadReq.setTargetUidType(userContext.getCitizenIdentifierType());
			eventPayloadReq.setTargetName(userContext.getName());
			eventPayloadReq.setApplicationStatus(request.getStatus().toString());
			eventPayloadReq.setRequestBody(request);
	
			Function<ApplicationStatus, EventPayload> auditRespPayload = (appResp -> {
				EventPayload eventPayloadResp = new EventPayload();
				eventPayloadResp.setTargetUid(userContext.getCitizenIdentifier());
				eventPayloadResp.setTargetUidType(userContext.getCitizenIdentifierType());
				eventPayloadResp.setTargetName(userContext.getName());
				eventPayloadResp.setApplicationStatus(request.getStatus().toString());
				
				return eventPayloadResp;
			});
    	
    	return eventSender.send(
                exchange,
                request,
                AuditEventType.CONFIRM_MOBILE_APPLICATION,
                ApplicationStatus.class,
                eventPayloadReq,
                auditRespPayload);
    	});
    }

    @Override
    public Mono<ApplicationStatus> confirmCertificateStoredWithEid(CertificateEidConfirmationRequest request, ServerWebExchange exchange) {
    	return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {

			EventPayload eventPayloadReq = new EventPayload();
			eventPayloadReq.setTargetUid(userContext.getCitizenIdentifier());
			eventPayloadReq.setTargetUidType(userContext.getCitizenIdentifierType());
			eventPayloadReq.setTargetName(userContext.getName());
			eventPayloadReq.setApplicationStatus(request.getStatus().toString());
			eventPayloadReq.setRequestBody(request);
	
			Function<ApplicationStatus, EventPayload> auditRespPayload = (appResp -> {
				EventPayload eventPayloadResp = new EventPayload();
				eventPayloadResp.setTargetUid(userContext.getCitizenIdentifier());
				eventPayloadResp.setTargetUidType(userContext.getCitizenIdentifierType());
				eventPayloadResp.setTargetName(userContext.getName());
				eventPayloadResp.setApplicationStatus(request.getStatus().toString());
				
				return eventPayloadResp;
			});
    	
    	return eventSender.send(
                exchange,
                request,
                AuditEventType.CONFIRM_MOBILE_APPLICATION,
                ApplicationStatus.class,
                eventPayloadReq,
                auditRespPayload);
    	});
    }

    @Override
    public Mono<List<CertificateHistoryDTO>> getCitizenCertificateHistoryById(UUID id, ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                id,
                AuditEventType.GET_CERTIFICATE_HISTORY,
                List.class);
        return result.map(e -> (List<CertificateHistoryDTO>) e);
    }

    @Override
    public Mono<HttpResponse> updateCertificateAlias(CertificateAlias certificateAlias, UUID certificateId, ServerWebExchange exchange) {
    	return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {

			EventPayload eventPayloadReq = new EventPayload();
			eventPayloadReq.setTargetUid(userContext.getCitizenIdentifier());
			eventPayloadReq.setTargetUidType(userContext.getCitizenIdentifierType());
			eventPayloadReq.setTargetName(userContext.getName());
			eventPayloadReq.setCertificateId(certificateId.toString());
			eventPayloadReq.setRequestBody(certificateAlias);
	
			Function<HttpResponse, EventPayload> auditRespPayload = (resp -> {
				EventPayload eventPayloadResp = new EventPayload();
				eventPayloadResp.setTargetUid(userContext.getCitizenIdentifier());
				eventPayloadResp.setTargetUidType(userContext.getCitizenIdentifierType());
				eventPayloadResp.setTargetName(userContext.getName());
				eventPayloadResp.setCertificateId(certificateId.toString());
				
				return eventPayloadResp;
			});
    	
    	return eventSender.send(
                exchange,
                Map.of("certificateId", certificateId.toString(), "alias", certificateAlias.getAlias()),
                AuditEventType.UPDATE_CERTIFICATE_ALIAS,
                HttpResponse.class,
                eventPayloadReq,
                auditRespPayload);
    	});
    }

    @Override
    public Mono<Page<PublicCertificateInfo>> findCertificatesInfo(String keyword, ServerWebExchange exchange, Pageable pageable) {
        return eventSender.send(
                exchange,
                Map.of("keyword", keyword, "pageable", pageable),
                AuditEventType.PUBLIC_FIND_CERTIFICATES_INFO,
                Page.class).map(e -> (Page<PublicCertificateInfo>) e);
    }
}
