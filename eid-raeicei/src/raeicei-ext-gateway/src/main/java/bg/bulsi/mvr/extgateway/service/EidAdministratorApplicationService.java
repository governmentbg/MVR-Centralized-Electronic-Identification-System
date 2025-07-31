package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.common.util.ValidationUtil;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.extgateway.api.v1.CreateEidAdministratorApplicationApiDelegate;
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
public class EidAdministratorApplicationService implements CreateEidAdministratorApplicationApiDelegate {

    public static final List<String> APPLICATION_SORT_FIELDS = List.of("applicationNumber", "eidName", "applicationType", "status", "createDate");
    private final EventSender eventSender;

    @Override
    public Mono<EidAdministratorApplicationResponseDTO> createEidAdministratorApplication(EidAdministratorApplicationDTO eidAdministratorApplicationDTO, ServerWebExchange exchange) {
        return UserContextHolder.getFromReactiveContext()
                .flatMap(context -> {

                    if (eidAdministratorApplicationDTO.getApplicant() == null) {
                        ContactDTO applicant = new ContactDTO();
                        eidAdministratorApplicationDTO.setApplicant(applicant);
                    }

                    eidAdministratorApplicationDTO.getApplicant().setName(context.getGivenNameCyrillic() + " " + context.getMiddleNameCyrillic() + " " + context.getFamillyNameCyrillic());
                    eidAdministratorApplicationDTO.getApplicant().setNameLatin(context.getGivenName() + " " + context.getMiddleName() + " " + context.getFamillyName());
                    eidAdministratorApplicationDTO.getApplicant().setEmail(context.getEmail());
                    eidAdministratorApplicationDTO.getApplicant().citizenIdentifierType(IdentifierTypeDTO.valueOf(context.getCitizenIdentifierType()));
                    eidAdministratorApplicationDTO.getApplicant().setCitizenIdentifierNumber(context.getCitizenIdentifier());
                    eidAdministratorApplicationDTO.getApplicant().seteIdentity(UUID.fromString(context.getEidentityId()));

                    EventPayload eventPayloadReq = new EventPayload();
                    eventPayloadReq.setApplicationType(eidAdministratorApplicationDTO.getApplicationType().name());
                    eventPayloadReq.setTargetUid(eidAdministratorApplicationDTO.getApplicant().getCitizenIdentifierNumber());
                    eventPayloadReq.setTargetUidType(eidAdministratorApplicationDTO.getApplicant().getCitizenIdentifierType().name());
                    eventPayloadReq.setTargetName(eidAdministratorApplicationDTO.getApplicant().getName());
                    eventPayloadReq.setApplicationStatus(eidAdministratorApplicationDTO.getStatus() == null ? ApplicationStatus.IN_REVIEW.name() : eidAdministratorApplicationDTO.getStatus().name());
                    eventPayloadReq.setApplicationType(eidAdministratorApplicationDTO.getApplicationType().name());

                    Function<EidAdministratorApplicationResponseDTO, EventPayload> auditRespPayload = (appResp -> {
                        EventPayload eventPayloadResp = new EventPayload();
                        eventPayloadResp.setApplicationType(eidAdministratorApplicationDTO.getApplicationType().name());
                        eventPayloadResp.setTargetUid(eidAdministratorApplicationDTO.getApplicant().getCitizenIdentifierNumber());
                        eventPayloadResp.setTargetUidType(eidAdministratorApplicationDTO.getApplicant().getCitizenIdentifierType().name());
                        eventPayloadResp.setTargetName(eidAdministratorApplicationDTO.getApplicant().getName());
                        eventPayloadResp.setApplicationStatus(eidAdministratorApplicationDTO.getStatus() == null ? ApplicationStatus.IN_REVIEW.name() : eidAdministratorApplicationDTO.getStatus().name());
                        eventPayloadResp.setApplicationType(eidAdministratorApplicationDTO.getApplicationType().name());

                        return eventPayloadResp;
                    });

                    return eventSender.send(exchange, eidAdministratorApplicationDTO, AuditEventType.CREATE_EXT_ISSUE_APPLICATION, EidAdministratorApplicationResponseDTO.class, eventPayloadReq, auditRespPayload);
                });
    }

    @Override
    public Mono<EidAdministratorApplicationResponseDTO> getEidAdministratorApplicationById(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.GET_APPLICATIONS_BY_ID, EidAdministratorApplicationResponseDTO.class);
    }

    @Override
    public Mono<Page<EidAdministratorApplicationShortDTO>> getEidAdministratorApplicationByFilter(String applicationNumber, String eidName, ApplicationType applicationType, ApplicationStatus status, ServerWebExchange exchange, Pageable pageable) {
        pageable = ValidationUtil.filterAllowedPageableSort(pageable, APPLICATION_SORT_FIELDS);
        EidApplicationFilter filter = new EidApplicationFilter(applicationNumber, eidName, applicationType, status, null, null, null, pageable);
        Mono<Page> result = eventSender.send(exchange, filter, AuditEventType.FIND_EXT_APPLICATION_BY_FILTER, Page.class);
        return result.map(e -> (Page<EidAdministratorApplicationShortDTO>) e);
    }
}
