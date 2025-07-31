package bg.bulsi.mvr.raeicei.backend.rabbitmq;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.rabbitmq.consumer.DefaultRabbitListener;
import bg.bulsi.mvr.raeicei.backend.facade.EidAdministratorFacade;
import bg.bulsi.mvr.raeicei.backend.facade.EidCenterFacade;
import bg.bulsi.mvr.raeicei.backend.service.ApproveNewCircumstancesService;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.amqp.support.converter.RemoteInvocationResult;
import org.springframework.messaging.handler.annotation.Header;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;
import java.util.Map;
import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.NOTE_IS_REQUIRED;

@Slf4j
@Component
@RequiredArgsConstructor
public class EidManagerListenerDispatcher {

    private final EidAdministratorFacade eidAdministratorFacade;
    private final EidCenterFacade eidCenterFacade;
    private final ApproveNewCircumstancesService approveNewCircumstancesService;

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidadministrator.id")
    @Transactional
    public RemoteInvocationResult getEidAdministratorById(@Valid @Payload UUID id) {
        EidAdministratorDTO dto = eidAdministratorFacade.findEidAdministratorById(id);
        return new RemoteInvocationResult(dto);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.external.api.v1.employee.eidadministrator")
    @Transactional
    public RemoteInvocationResult getEidAdministratorWithAuthorizedForEmployeeById() {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        EidAdministratorAuthorizedDTO dto = eidAdministratorFacade.findEidAdministratorWithAuthorizedPersonsById(eidManagerId);
        return new RemoteInvocationResult(dto);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.newcircs.eidadministrator.details.id")
    @Transactional
    public RemoteInvocationResult getFullEidAdministratorById(@Valid @Payload Map<String, Object> payload) {
        UUID id = ((UUID) payload.get("id"));
        Boolean onlyActiveElements = (Boolean) payload.get("onlyActiveElements");

        if (onlyActiveElements == null) {
            onlyActiveElements = false;
        }

        EidAdministratorFullDTO dto = eidAdministratorFacade.findFullEidAdministratorById(id, onlyActiveElements);
        return new RemoteInvocationResult(dto);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidadministrator.getall")
    @Transactional
    public RemoteInvocationResult getAllEidAdministrators() {
        List<EidAdministratorDTO> dtos = eidAdministratorFacade.getAllEidAdministrators();
        return new RemoteInvocationResult(dtos);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidadministrator.history.getall")
    @Transactional
    public RemoteInvocationResult getAllEidAdministratorsHistory(@Valid @Payload Boolean stoppedIncluded) {
        List<EidAdministratorHistoryDTO> dtos = eidAdministratorFacade.getAllEidAdministratorsHistory(stoppedIncluded);
        return new RemoteInvocationResult(dtos);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.newcircs.eidadministrator.findbystatus.status")
    @Transactional
    public RemoteInvocationResult getAllEidAdministratorsByStatus(@Valid @Payload EidManagerStatus eidManagerStatus) {
        List<EidAdministratorDTO> dtos = eidAdministratorFacade.getAllEidAdministratorsByStatus(eidManagerStatus);
        return new RemoteInvocationResult(dtos);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.PUT.raeicei.api.v1.newcircs.eidadministrator.approve.id")
    public RemoteInvocationResult approveEidAdministrator(@Valid @Payload Map<String, Object> payload, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        UUID id = ((UUID) payload.get("id"));
        NewCircumstancesStatus newCircumstancesStatus = (NewCircumstancesStatus) payload.get("newCircumstancesStatus");
        NoteAndDocumentsDTO noteAndDocumentsDTO = (NoteAndDocumentsDTO) payload.get("noteAndDocumentsDTO");
        UserContext context = UserContextHolder.getFromServletContext();

        if (noteAndDocumentsDTO.getNote() == null) {
            throw new ValidationMVRException(NOTE_IS_REQUIRED);
        }

        noteAndDocumentsDTO.getNote().setAuthorsUsername(context.getUsername());

        if (AuditEventType.CHANGE_EID_MANAGER_STATUS.name().equals(auditEventType.name())) {
            noteAndDocumentsDTO.getNote().setIsOutgoing(true);

            if (noteAndDocumentsDTO.getDocuments() != null) {
                noteAndDocumentsDTO.getDocuments().forEach(d -> {d.setIsOutgoing(true);});
            }
        }

        approveNewCircumstancesService.approveEidAdministrator(id, newCircumstancesStatus, noteAndDocumentsDTO);
        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.eidadministrator.create")
    @Transactional
    public RemoteInvocationResult createEidAdministrator(@Valid @Payload EidAdministratorDTO dto) {
        EidAdministratorDTO result = eidAdministratorFacade.createEidAdministrator(dto);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.PUT.raeicei.api.v1.eidadministrator.id")
    @Transactional
    public RemoteInvocationResult updateEidAdministrator(@Valid @Payload EidAdministratorDTO dto) {
        EidAdministratorDTO result = eidAdministratorFacade.updateEidAdministrator(dto);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidcenter.getall")
    @Transactional
    public RemoteInvocationResult getAllEidCenters() {
        List<EidCenterDTO> result = eidCenterFacade.getAllEidCenter();
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidcenter.history.getall")
    @Transactional
    public RemoteInvocationResult getAllEidCentersHistory(@Valid @Payload Boolean stoppedIncluded) {
        List<EidCenterHistoryDTO> result = eidCenterFacade.getAllEidCenterHistory(stoppedIncluded);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.newcircs.eidcenter.findbystatus.status")
    @Transactional
    public RemoteInvocationResult getAllEidCentersByStatus(@Valid @Payload EidManagerStatus eidManagerStatus) {
        List<EidCenterDTO> result = eidCenterFacade.getAllEidCentersByStatus(eidManagerStatus);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.PUT.raeicei.api.v1.newcircs.eidcenter.approve.id")
    public RemoteInvocationResult approveEidCenter(@Valid @Payload Map<String, Object> payload, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        UUID id = ((UUID) payload.get("id"));
        NewCircumstancesStatus newCircumstancesStatus = (NewCircumstancesStatus) payload.get("newCircumstancesStatus");
        NoteAndDocumentsDTO noteAndDocumentsDTO = (NoteAndDocumentsDTO) payload.get("noteAndDocumentsDTO");
        UserContext context = UserContextHolder.getFromServletContext();

        if (noteAndDocumentsDTO.getNote() == null) {
            throw new ValidationMVRException(NOTE_IS_REQUIRED);
        }

        noteAndDocumentsDTO.getNote().setAuthorsUsername(context.getUsername());

        if (AuditEventType.CHANGE_EID_MANAGER_STATUS.name().equals(auditEventType.name())) {
            noteAndDocumentsDTO.getNote().setIsOutgoing(true);

            if (noteAndDocumentsDTO.getDocuments() != null) {
                noteAndDocumentsDTO.getDocuments().forEach(d -> {d.setIsOutgoing(true);});
            }
        }

        approveNewCircumstancesService.approveEidCenter(id, newCircumstancesStatus, noteAndDocumentsDTO);
        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidcenter.id")
    @Transactional
    public RemoteInvocationResult getEidCenterById(@Payload UUID id) {
        EidCenterDTO result = eidCenterFacade.findEidCenterById(id);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.external.api.v1.employee.eidcenter")
    @Transactional
    public RemoteInvocationResult getEidCenterWithAuthorizedForEmployeeById() {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        EidCenterAuthorizedDTO result = eidCenterFacade.findEidCenterWithAuthorizedPersonsById(eidManagerId);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.newcircs.eidcenter.details.id")
    @Transactional
    public RemoteInvocationResult getFullEidCenterById(@Valid @Payload Map<String, Object> payload) {
        UUID id = ((UUID) payload.get("id"));
        Boolean onlyActiveElements = (Boolean) payload.get("onlyActiveElements");

        if (onlyActiveElements == null) {
            onlyActiveElements = false;
        }

        EidCenterFullDTO result = eidCenterFacade.findFullEidCenterById(id, onlyActiveElements);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.eidcenter.create")
    @Transactional
    public RemoteInvocationResult createCenter(@Payload EidCenterDTO dto) {
        EidCenterDTO result = eidCenterFacade.createEidCenter(dto);
        return new RemoteInvocationResult(result);
    }

//    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.eidcenter.create")
//    @Transactional
//    public RemoteInvocationResult updateCenter(@Payload UUID id) {
//        EidCenterDTO result = eidCenterFacade.findEidCenterById(id);
//        return new RemoteInvocationResult(result);
//    }
}
