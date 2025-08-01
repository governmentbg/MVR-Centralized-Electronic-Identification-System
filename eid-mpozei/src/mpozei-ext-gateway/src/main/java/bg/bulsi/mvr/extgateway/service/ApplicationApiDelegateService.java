package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.common.service.FileFormatService;
import bg.bulsi.mvr.common.util.ValidationUtil;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.contract.dto.mapper.ApplicationToAuditEventTypeMapper;
import bg.bulsi.mvr.mpozei.contract.dto.xml.EidApplicationXml;
import bg.bulsi.mvr.mpozei.extgateway.api.v1.ApplicationApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.time.LocalDate;
import java.time.OffsetDateTime;
import java.util.Base64;
import java.util.List;
import java.util.Objects;
import java.util.UUID;
import java.util.function.Function;

import static bg.bulsi.mvr.common.exception.ErrorCode.ONLY_DESK_APPLICATIONS_CAN_BE_REVOKED;
import static bg.bulsi.mvr.common.exception.ErrorCode.USER_ID_OR_CITIZEN_PROFILE_ID_MUST_BE_PROVIDED;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertFalse;

@Component
@Slf4j
@RequiredArgsConstructor
public class ApplicationApiDelegateService implements ApplicationApiDelegate {
    public static final List<String> APPLICATION_SORT_FIELDS = List.of("id", "status", "createDate", "deviceId", "applicationType", "eidAdministratorId");

    private final EventSender eventSender;

    private final FileFormatService fileFormatService;
    
    private final ApplicationAuditMapper mapper;
    
    @Override
    public Mono<ApplicationXmlResponse> generateXmlSignature(ApplicationXmlRequest request, ServerWebExchange exchange) {
       // assertFalse(Objects.equals(request.getApplicationType(), ApplicationType.REVOKE_EID), ONLY_DESK_APPLICATIONS_CAN_BE_REVOKED);
        return eventSender.send(
                exchange,
                request,
                AuditEventType.GENERATE_APPLICATION_SIGNATURE,
                ApplicationXmlResponse.class);
    }

	@Override
	public Mono<OnlineApplicationResponse> createOnlineApplication(OnlineApplicationRequest request,
			ServerWebExchange exchange) {
    	return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {
	
			String originalXml = new String(Base64.getDecoder().decode(request.getXml()));
	
			EidApplicationXml xml = fileFormatService.createObjectFromXmlString(originalXml, EidApplicationXml.class);
	
			EventPayload eventPayloadReq = new EventPayload();
			eventPayloadReq.setTargetUid(userContext.getCitizenIdentifier());
			eventPayloadReq.setTargetUidType(userContext.getCitizenIdentifierType());
			eventPayloadReq.setTargetName(userContext.getName());
			eventPayloadReq.setApplicationId(xml.getApplicationId());
			eventPayloadReq.setApplicationType(xml.getApplicationType());
			eventPayloadReq.setRequestBody(originalXml);
	
			Function<OnlineApplicationResponse, EventPayload> auditRespPayload = (appResp -> {
				EventPayload eventPayloadResp = new EventPayload();
				eventPayloadResp.setTargetUid(userContext.getCitizenIdentifier());
				eventPayloadResp.setTargetUidType(userContext.getCitizenIdentifierType());
				eventPayloadResp.setTargetName(userContext.getName());
				eventPayloadResp.setApplicationId(appResp.getId().toString());
				eventPayloadResp.setApplicationType(xml.getApplicationType());
				eventPayloadResp.setApplicationStatus(appResp.getStatus().toString());
	
				return eventPayloadResp;
			});
	
			AuditEventType auditEventType = ApplicationToAuditEventTypeMapper
					.mapToAuditEventType(ApplicationType.valueOf(xml.getApplicationType()), false);
	
			return eventSender.send(exchange, request, auditEventType, OnlineApplicationResponse.class, eventPayloadReq,
					auditRespPayload);
    	});
	}
    
	@Override
	public Mono<OnlineApplicationResponse> createOnlineApplicationForCertStatusPlain(OnlineCertStatusApplicationRequest request,
			ServerWebExchange exchange) {
    	return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {
	
			EventPayload eventPayloadReq = new EventPayload();
			eventPayloadReq.setTargetUid(userContext.getCitizenIdentifier());
			eventPayloadReq.setTargetUidType(userContext.getCitizenIdentifierType());
			eventPayloadReq.setTargetName(userContext.getName());
			eventPayloadReq.setApplicationType(request.getApplicationType().toString());
			eventPayloadReq.setCertificateId(request.getCertificateId().toString());
			eventPayloadReq.setRequestBody(request);
			
			Function<OnlineApplicationResponse, EventPayload> auditRespPayload = (appResp -> {
				EventPayload eventPayloadResp = new EventPayload();
				eventPayloadResp.setTargetUid(userContext.getCitizenIdentifier());
				eventPayloadResp.setTargetUidType(userContext.getCitizenIdentifierType());
				eventPayloadResp.setTargetName(userContext.getName());
				eventPayloadResp.setApplicationId(appResp.getId().toString());
				eventPayloadResp.setApplicationType(request.getApplicationType().toString());
				eventPayloadResp.setApplicationStatus(appResp.getStatus().toString());
				eventPayloadResp.setCertificateId(request.getCertificateId().toString());
				
				return eventPayloadResp;
			});
	
			AuditEventType auditEventType = ApplicationToAuditEventTypeMapper
					.mapToAuditEventType(request.getApplicationType(), false);
	
			return eventSender.send(
					exchange, 
					request, 
					auditEventType, 
					OnlineApplicationResponse.class, 
					eventPayloadReq,
					auditRespPayload);
    	});
	}
	
    @Override
    public Mono<OnlineApplicationResponse> createOnlineApplicationForCertStatusSigned(OnlineApplicationRequest request, ServerWebExchange exchange) {
    	return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {
	    	String originalXml = new String(Base64.getDecoder().decode(request.getXml()));
	
			EidApplicationXml xml = fileFormatService.createObjectFromXmlString(originalXml, EidApplicationXml.class);
	
			EventPayload eventPayloadReq = new EventPayload();
			eventPayloadReq.setTargetUid(userContext.getCitizenIdentifier());
			eventPayloadReq.setTargetUidType(userContext.getCitizenIdentifierType());
			eventPayloadReq.setTargetName(userContext.getName());
			eventPayloadReq.setApplicationId(xml.getApplicationId());
			eventPayloadReq.setApplicationType(xml.getApplicationType());
			eventPayloadReq.setCertificateId(xml.getCertificateId());
			eventPayloadReq.setRequestBody(originalXml);
	
			Function<OnlineApplicationResponse, EventPayload> auditRespPayload = (appResp -> {
				EventPayload eventPayloadResp = new EventPayload();
				eventPayloadResp.setTargetUid(userContext.getCitizenIdentifier());
				eventPayloadResp.setTargetUidType(userContext.getCitizenIdentifierType());
				eventPayloadResp.setTargetName(userContext.getName());
				eventPayloadResp.setApplicationId(xml.getApplicationId());
				eventPayloadResp.setApplicationType(xml.getApplicationType());
				eventPayloadResp.setApplicationStatus(appResp.getStatus().toString());
				eventPayloadResp.setCertificateId(xml.getCertificateId());
				
				return eventPayloadResp;
			});
	    	
	        return eventSender.send(
	                exchange,
	                request,
	                AuditEventType.CREATE_CERT_STATUS_ONLINE_APPLICATION,
	                OnlineApplicationResponse.class,
	                eventPayloadReq,
	                auditRespPayload);
    	});
    }
	
	
//    @Override
//    public Mono<OnlineApplicationResponse> createOnlineApplication(OnlineApplicationRequest request, ServerWebExchange exchange) {
//        return eventSender.send(
//                exchange,
//                request,
//                AuditEventType.CREATE_ONLINE_APPLICATION,
//                OnlineApplicationResponse.class);
//    }

    @Override
    public Mono<Page<ApplicationDTO>> findApplicationsByFilter(List<ApplicationStatus> statuses,
    											               List<ApplicationSubmissionType> submissionTypes,
                                                               String id,
                                                               String applicationNumber,
                                                               List<UUID> deviceIds,
                                                               OffsetDateTime createDateFrom,
                                                               OffsetDateTime createDateTo,
                                                               List<ApplicationType> applicationTypes,
                                                               UUID eidAdministratorId,
                                                               List<UUID> eidAdministratorFrontOfficeId,
                                                               ServerWebExchange exchange,
                                                               Pageable pageable) {
        Pageable allowedPageable = ValidationUtil.filterAllowedPageableSort(pageable, APPLICATION_SORT_FIELDS);
        return UserContextHolder.getFromReactiveContext()
                .flatMap(context -> {
                    assertFalse(context.getEidentityId() == null && context.getCitizenProfileId() == null, USER_ID_OR_CITIZEN_PROFILE_ID_MUST_BE_PROVIDED);

                    UUID eidentityId = Objects.isNull(context.getEidentityId()) ? null : UUID.fromString(context.getEidentityId());
                    UUID citizenProfileId = Objects.isNull(context.getCitizenProfileId()) ? null : UUID.fromString(context.getCitizenProfileId());
                    ApplicationFilter filter = new ApplicationFilter(eidentityId, citizenProfileId, id, applicationNumber, createDateFrom, createDateTo, deviceIds, statuses, applicationTypes, eidAdministratorId, eidAdministratorFrontOfficeId, submissionTypes,  allowedPageable);
                    Mono<Page> result = eventSender.send(
                            exchange,
                            filter,
                            AuditEventType.GET_APPLICATIONS_BY_EIDENTITY_ID,
                            Page.class);
                    return result.map(e -> (Page<ApplicationDTO>) e);
                });
    }

    @Override
    public Mono<ApplicationDetailsExternalResponse> getApplicationById(UUID id, ServerWebExchange exchange) {
		return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {
			
		EventPayload eventPayloadReq = new EventPayload();
		eventPayloadReq.setTargetUid(userContext.getCitizenIdentifier());
		eventPayloadReq.setTargetUidType(userContext.getCitizenIdentifierType());
		eventPayloadReq.setTargetName(userContext.getName());

    	Function<ApplicationDetailsExternalResponse, EventPayload> auditRespPayload = (appResp -> {
			EventPayload eventPayloadResp = new EventPayload();
			eventPayloadResp.setTargetUid(userContext.getCitizenIdentifier());
			eventPayloadResp.setTargetUidType(userContext.getCitizenIdentifierType());
			eventPayloadResp.setTargetName(userContext.getName());
			eventPayloadResp.setApplicationId(appResp.getId().toString());
			eventPayloadResp.setApplicationType(appResp.getApplicationType().toString());
			eventPayloadResp.setApplicationStatus(appResp.getStatus().toString());
			eventPayloadResp.setCertificateId(String.valueOf(appResp.getCertificateId()));
			
			return eventPayloadResp;
		});
		
    	return eventSender.send(
                exchange,
                id,
                AuditEventType.GET_APPLICATIONS_BY_ID,
                ApplicationDetailsExternalResponse.class,
                eventPayloadReq,
                auditRespPayload
                );
		});
    }
//
//    @Override
//    public Mono<ApplicationStatus> completeIssueEidApplication(UUID id, ServerWebExchange exchange) {
//        return eventSender.send(
//                exchange,
//                id,
//                AuditEventType.COMPLETE_ISSUE_EID,
//                ApplicationStatus.class);
//    }

    @Override
    public Mono<ApplicationStatus> signOnlineApplication(SignOnlineApplicationRequest request, ServerWebExchange exchange) {
		return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {
			
		EventPayload eventPayloadReq = new EventPayload();
		eventPayloadReq.setTargetUid(userContext.getCitizenIdentifier());
		eventPayloadReq.setTargetUidType(userContext.getCitizenIdentifierType());
		eventPayloadReq.setTargetName(userContext.getName());
    	
    	Function<ApplicationStatus, EventPayload> auditRespPayload = (appResp -> {
			EventPayload eventPayloadResp = new EventPayload();
			eventPayloadResp.setTargetUid(userContext.getCitizenIdentifier());
			eventPayloadResp.setTargetUidType(userContext.getCitizenIdentifierType());
			eventPayloadResp.setTargetName(userContext.getName());
			eventPayloadResp.setApplicationStatus(appResp.toString());

			return eventPayloadResp;
		});
		
    	return eventSender.send(
                exchange,
                request,
                AuditEventType.SIGN_APPLICATION,
                ApplicationStatus.class,
                eventPayloadReq,
                auditRespPayload
                );
		});
    }

	@Override
	public Mono<ApplicationStatus> completeCertStatusApplication(UUID applicationId, ServerWebExchange exchange) {
		return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {
			
		EventPayload eventPayloadReq = new EventPayload();
		eventPayloadReq.setTargetUid(userContext.getCitizenIdentifier());
		eventPayloadReq.setTargetUidType(userContext.getCitizenIdentifierType());
		eventPayloadReq.setTargetName(userContext.getName());
		eventPayloadReq.setApplicationId(applicationId.toString());
    	
    	Function<ApplicationStatus, EventPayload> auditRespPayload = (appResp -> {
			EventPayload eventPayloadResp = new EventPayload();
			eventPayloadResp.setTargetUid(userContext.getCitizenIdentifier());
			eventPayloadResp.setTargetUidType(userContext.getCitizenIdentifierType());
			eventPayloadResp.setTargetName(userContext.getName());
			eventPayloadResp.setApplicationId(applicationId.toString());

			return eventPayloadResp;
		});
		
		return eventSender.send(
                exchange,
                applicationId,
                AuditEventType.COMPLETE_ONLINE_APPLICATION,
                ApplicationStatus.class,
                eventPayloadReq,
                auditRespPayload);
		});
	}
}
