package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.common.util.ValidationUtil;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.extgateway.api.v1.CreateEidCenterApplicationApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.List;
import java.util.UUID;
import java.util.function.Function;

@Component
@Slf4j
@RequiredArgsConstructor
public class EidCenterApplicationService implements CreateEidCenterApplicationApiDelegate {

    public static final List<String> APPLICATION_SORT_FIELDS = List.of("applicationNumber", "eidName", "applicationType", "status", "createDate");
    private final EventSender eventSender;

    @Override
    public Mono<EidCenterApplicationResponseDTO> createEidCenterApplication(EidCenterApplicationDTO eidCenterApplicationDTO, ServerWebExchange exchange) {


        return UserContextHolder.getFromReactiveContext()
                .flatMap(context -> {

                    if (eidCenterApplicationDTO.getApplicant() == null) {
                        ContactDTO applicant = new ContactDTO();
                        eidCenterApplicationDTO.setApplicant(applicant);

                    }

                    eidCenterApplicationDTO.getApplicant().setName(context.getGivenNameCyrillic() + " " + context.getMiddleNameCyrillic() + " " + context.getFamillyNameCyrillic());
                    eidCenterApplicationDTO.getApplicant().setNameLatin(context.getGivenName() + " " + context.getMiddleName() + " " + context.getFamillyName());
                    eidCenterApplicationDTO.getApplicant().setEmail(context.getEmail());
                    eidCenterApplicationDTO.getApplicant().citizenIdentifierType(IdentifierTypeDTO.valueOf(context.getCitizenIdentifierType()));
                    eidCenterApplicationDTO.getApplicant().setCitizenIdentifierNumber(context.getCitizenIdentifier());
                    eidCenterApplicationDTO.getApplicant().seteIdentity(UUID.fromString(context.getEidentityId()));

                    EventPayload eventPayloadReq = new EventPayload();
                    eventPayloadReq.setApplicationType(eidCenterApplicationDTO.getApplicationType().name());
                    eventPayloadReq.setTargetUid(eidCenterApplicationDTO.getApplicant().getCitizenIdentifierNumber());
                    eventPayloadReq.setTargetUidType(eidCenterApplicationDTO.getApplicant().getCitizenIdentifierType().name());
                    eventPayloadReq.setTargetName(eidCenterApplicationDTO.getApplicant().getName());
                    eventPayloadReq.setApplicationStatus(eidCenterApplicationDTO.getStatus() == null ? ApplicationStatus.IN_REVIEW.name() : eidCenterApplicationDTO.getStatus().name());
                    eventPayloadReq.setApplicationType(eidCenterApplicationDTO.getApplicationType().name());

                    Function<EidCenterApplicationResponseDTO, EventPayload> auditRespPayload = (appResp -> {
                        EventPayload eventPayloadResp = new EventPayload();
                        eventPayloadResp.setApplicationType(eidCenterApplicationDTO.getApplicationType().name());
                        eventPayloadResp.setTargetUid(eidCenterApplicationDTO.getApplicant().getCitizenIdentifierNumber());
                        eventPayloadResp.setTargetUidType(eidCenterApplicationDTO.getApplicant().getCitizenIdentifierType().name());
                        eventPayloadResp.setTargetName(eidCenterApplicationDTO.getApplicant().getName());
                        eventPayloadResp.setApplicationStatus(eidCenterApplicationDTO.getStatus() == null ? ApplicationStatus.IN_REVIEW.name() : eidCenterApplicationDTO.getStatus().name());
                        eventPayloadResp.setApplicationType(eidCenterApplicationDTO.getApplicationType().name());

                        return eventPayloadResp;
                    });

                    return eventSender.send(exchange, eidCenterApplicationDTO, AuditEventType.CREATE_EXT_ISSUE_APPLICATION, EidCenterApplicationResponseDTO.class, eventPayloadReq, auditRespPayload);
                });
    }

    @Override
    public Mono<EidCenterApplicationResponseDTO> getEidCenterApplicationById(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.GET_APPLICATIONS_BY_ID, EidCenterApplicationResponseDTO.class);
    }

    @Override
    public Mono<Page<EidCenterApplicationShortDTO>> getEidCenterApplicationByFilter(String applicationNumber, String eidName, ApplicationType applicationType, ApplicationStatus status, ServerWebExchange exchange, Pageable pageable) {
        pageable = ValidationUtil.filterAllowedPageableSort(pageable, APPLICATION_SORT_FIELDS);
        EidApplicationFilter filter = new EidApplicationFilter(applicationNumber, eidName, applicationType, status, null, null, null, pageable);
        Mono<Page> result = eventSender.send(exchange, filter, AuditEventType.FIND_EXT_APPLICATION_BY_FILTER, Page.class);
        return result.map(e -> (Page<EidCenterApplicationShortDTO>) e);
    }
}
