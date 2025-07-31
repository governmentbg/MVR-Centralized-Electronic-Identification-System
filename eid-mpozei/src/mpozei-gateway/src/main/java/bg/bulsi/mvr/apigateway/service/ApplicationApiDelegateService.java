package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.common.util.ValidationUtil;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.contract.dto.mapper.ApplicationToAuditEventTypeMapper;
import bg.bulsi.mvr.mpozei.gateway.api.v1.ApplicationApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.time.LocalDate;
import java.time.OffsetDateTime;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.function.Function;
import java.util.function.UnaryOperator;

@Component
@Slf4j
@RequiredArgsConstructor
public class ApplicationApiDelegateService implements ApplicationApiDelegate {
    public static final List<String> APPLICATION_SORT_FIELDS = List.of("id", "status", "createDate", "deviceId");

    private final EventSender eventSender;

    @Override
    @Transactional
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
	public Mono<ApplicationResponse> createDeskApplication(DeskApplicationRequest request, Boolean regixAvailability,
                                                           ServerWebExchange exchange) {
    	EventPayload eventPayloadReq = new EventPayload();
    	//TODO: Requester properties should be set from JWT
//		eventPayloadReq.setRequesterUid(request.getCitizenIdentifierNumber());
//		eventPayloadReq.setRequesterUidType(request.getCitizenIdentifierType().name());
//		eventPayloadReq.setRequesterName(request.getFirstName(), request.getSecondName(), request.getLastName());
//		eventPayloadReq.setIdCardNumber(request.getPersonalIdentityDocument().getIdentityNumber());
		eventPayloadReq.setApplicationType(request.getApplicationType().name());
		eventPayloadReq.setTargetUid(request.getCitizenIdentifierNumber());
		eventPayloadReq.setTargetUidType(request.getCitizenIdentifierType().name());
		eventPayloadReq.setTargetName(request.getFirstName(), request.getSecondName(), request.getLastName());
		eventPayloadReq.setRequestBody(request.toString());
		
		Function<ApplicationResponse, EventPayload> auditRespPayload = (appResp -> {
			EventPayload eventPayloadResp = new EventPayload();
			eventPayloadResp.setApplicationType(request.getApplicationType().name());
			eventPayloadResp.setTargetUid(request.getCitizenIdentifierNumber());
			eventPayloadResp.setTargetUidType(request.getCitizenIdentifierType().name());
			eventPayloadResp.setTargetName(request.getFirstName(), request.getSecondName(), request.getLastName());
			
			return eventPayloadResp;
		});
		
		AuditEventType auditEventType = ApplicationToAuditEventTypeMapper.mapToAuditEventType(request.getApplicationType(), true);
    	return UserContextHolder.getFromReactiveContext().map(userContext -> {
			userContext.setRegixAvailability(regixAvailability);
			return userContext;
		})
		.then(eventSender.send(exchange, request, auditEventType, ApplicationResponse.class, eventPayloadReq, auditRespPayload));
	}

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
    public Mono<byte[]> exportApplication(UUID applicationId, ServerWebExchange exchange) {
    	EventPayload eventPayloadReq = new EventPayload();
		eventPayloadReq.setRequestBody(applicationId.toString());
    	
        return eventSender.send(
                exchange,
                applicationId,
                AuditEventType.EXPORT_APPLICATION,
                byte[].class, 
                eventPayloadReq,
                null);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
    public Mono<ApplicationStatus> updateApplicationStatus(UUID id, ApplicationStatus status, ServerWebExchange exchange) {
        Map<String, String> payload = new HashMap<>();
        payload.put("id", id.toString());
        payload.put("status", status.name());
        
    	EventPayload eventPayloadReq = new EventPayload();
    	eventPayloadReq.setApplicationId(id.toString());
    	eventPayloadReq.setApplicationStatus(status.name());
    	
        return eventSender.send(
                exchange,
                payload,
                AuditEventType.UPDATE_CITIZEN_CERTIFICATE_STATUS,
                ApplicationStatus.class,
                eventPayloadReq,
                null);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
    public Mono<ApplicationStatus> signApplication(UUID id, ApplicationSignRequest applicationSignRequest, ServerWebExchange exchange) {
        Map<String, Object> payload = new HashMap<>();
        payload.put("applicationId", id);
        payload.put("applicationSignRequest", applicationSignRequest);
    	
    	EventPayload eventPayloadReq = new EventPayload();
    	eventPayloadReq.setApplicationId(id.toString());
    	eventPayloadReq.setRequestBody(applicationSignRequest);
    	
    	return eventSender.send(
                exchange,
                payload,
                AuditEventType.SIGN_APPLICATION,
                ApplicationStatus.class,
                eventPayloadReq,
                null);    
    	}

    /**
     * Used by external Eid Administrators/MVR
     */
    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin', 'ROLE_EXT_AEI_operator')")
    public Mono<Page<ApplicationDTO>> findApplicationsByFilter(UUID eidentityId,
                                                               List<ApplicationStatus> statuses,
                                                               List<ApplicationSubmissionType> submissionTypes,
                                                               String id,
                                                               String applicationNumber,
                                                               List<UUID> deviceIds,
                                                               List<UUID> eidAdministratorFrontOfficeId,
                                                               List<ApplicationType> applicationType,
                                                               OffsetDateTime createDateFrom,
                                                               OffsetDateTime createDateTo,
                                                               ServerWebExchange exchange,
                                                               Pageable pageable) {
        pageable = ValidationUtil.filterAllowedPageableSort(pageable, APPLICATION_SORT_FIELDS);
        ApplicationFilter filter = new ApplicationFilter(eidentityId, id, applicationNumber, createDateFrom, createDateTo, deviceIds, eidAdministratorFrontOfficeId, statuses, submissionTypes, applicationType, pageable);
        Mono<Page> result = eventSender.send(
                exchange,
                filter,
                AuditEventType.FIND_APPLICATION_BY_FILTER,
                Page.class);
        return result.map(e -> (Page<ApplicationDTO>) e);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin', 'ROLE_EXT_AEI_operator')")
    public Mono<ApplicationDetailsResponse> getApplicationById(UUID id, ServerWebExchange exchange) {
    	EventPayload eventPayloadReq = new EventPayload();
    	eventPayloadReq.setApplicationId(id.toString());
    	
		Function<ApplicationDetailsResponse, EventPayload> auditRespPayload = (appResp -> {
			EventPayload eventPayloadResp = new EventPayload();
	    	eventPayloadReq.setApplicationId(id.toString());
			
			return eventPayloadResp;
		});
    	
        return eventSender.send(
                exchange,
                id,
                AuditEventType.GET_APPLICATIONS_BY_ID,
                ApplicationDetailsResponse.class,
                eventPayloadReq,
                auditRespPayload);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin', 'ROLE_EXT_AEI_m2m')")
    public Mono<ApplicationStatus> completeIssueEidApplication(UUID id, ServerWebExchange exchange) {
    	EventPayload eventPayloadReq = new EventPayload();
    	eventPayloadReq.setApplicationId(id.toString());
    	
		Function<ApplicationStatus, EventPayload> auditRespPayload = (appResp -> {
			EventPayload eventPayloadResp = new EventPayload();
	    	eventPayloadReq.setApplicationId(id.toString());
	    	eventPayloadReq.setApplicationStatus(appResp.name());

			
			return eventPayloadResp;
		});
    	
        return eventSender.send(
                exchange,
                id,
                AuditEventType.COMPLETE_ISSUE_EID,
                ApplicationStatus.class,
                eventPayloadReq,
                auditRespPayload);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
    public Mono<ApplicationStatus> denyApplication(UUID applicationId, UUID reasonId, String reasonText, ServerWebExchange exchange) {
    	EventPayload eventPayloadReq = new EventPayload();
    	eventPayloadReq.setApplicationId(applicationId.toString());
    	
		Function<ApplicationStatus, EventPayload> auditRespPayload = (appResp -> {
			EventPayload eventPayloadResp = new EventPayload();
	    	eventPayloadReq.setApplicationId(applicationId.toString());
	    	eventPayloadReq.setApplicationStatus(appResp.name());

			
			return eventPayloadResp;
		});
    	
        return eventSender.send(
                exchange,
                new DenyApplicationDTO(applicationId, reasonId, reasonText),
                AuditEventType.DENY_APPLICATION,
                ApplicationStatus.class,
                eventPayloadReq,
                auditRespPayload);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
    public Mono<String> addGuardiansToApplication(UUID applicationId, ApplicationAddGuardiansRequest request, ServerWebExchange exchange) {
        HashMap<String, Object> body = new HashMap<>();
        body.put("request", request);
        body.put("applicationId", applicationId);
        
    	EventPayload eventPayloadReq = new EventPayload();
    	eventPayloadReq.setApplicationId(applicationId.toString());
    	
		Function<String, EventPayload> auditRespPayload = (appResp -> {
			EventPayload eventPayloadResp = new EventPayload();
	    	eventPayloadReq.setApplicationId(applicationId.toString());
	    	eventPayloadReq.setRequestBody(request);

			
			return eventPayloadResp;
		});
        
        return eventSender.send(
                exchange,
                body,
                AuditEventType.ADD_GUARDIANS_TO_APPLICATION,
                String.class,
                eventPayloadReq,
                auditRespPayload);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
    public Mono<byte[]> exportApplicationConfirmation(UUID applicationId, ServerWebExchange exchange) {
    	EventPayload eventPayloadReq = new EventPayload();
    	eventPayloadReq.setApplicationId(applicationId.toString());
    	
		Function<byte[], EventPayload> auditRespPayload = (appResp -> {
			EventPayload eventPayloadResp = new EventPayload();
	    	eventPayloadReq.setApplicationId(applicationId.toString());
			
			return eventPayloadResp;
		});

        return eventSender.send(
                exchange,
                applicationId,
                AuditEventType.EXPORT_APPLICATION,
                byte[].class,
                eventPayloadReq,
                auditRespPayload);
    }

    /**
     * Used by MVR - superuser
     */
    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_super_admin')")
    public Mono<Page<ApplicationDTO>> findApplicationsByFilterAdmin(UUID eidentityId, List<ApplicationStatus> statuses, List<ApplicationSubmissionType> submissionTypes, String id, String applicationNumber, List<UUID> deviceIds, UUID administratorId, List<UUID> eidAdministratorFrontOfficeId, List<ApplicationType> applicationType, OffsetDateTime createdDateFrom, OffsetDateTime createdDateTo, ServerWebExchange exchange, Pageable pageable) {
        pageable = ValidationUtil.filterAllowedPageableSort(pageable, APPLICATION_SORT_FIELDS);
        ApplicationFilter filter = new ApplicationFilter(eidentityId, id, applicationNumber, createdDateFrom, createdDateTo, deviceIds, administratorId, eidAdministratorFrontOfficeId, statuses, submissionTypes, applicationType, pageable);
        Mono<Page> result = eventSender.send(
                exchange,
                filter,
                AuditEventType.GET_APPLICATIONS_ADMIN,
                Page.class);
        return result.map(e -> (Page<ApplicationDTO>) e);
    }
}
