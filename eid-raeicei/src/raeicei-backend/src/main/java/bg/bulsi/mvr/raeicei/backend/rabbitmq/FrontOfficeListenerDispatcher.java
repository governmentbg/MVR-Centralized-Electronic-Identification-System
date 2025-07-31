package bg.bulsi.mvr.raeicei.backend.rabbitmq;

import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.rabbitmq.consumer.DefaultRabbitListener;
import bg.bulsi.mvr.raeicei.backend.facade.EidManagerFrontOfficeFacade;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.HttpResponse;
import bg.bulsi.mvr.raeicei.contract.dto.OfficesByRegionDTO;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.amqp.support.converter.RemoteInvocationResult;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;
import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.MUST_BE_EMPLOYEE_OF_EID_MANAGER;

@Slf4j
@Component
@RequiredArgsConstructor
public class FrontOfficeListenerDispatcher {

    private final EidManagerFrontOfficeFacade eidManagerFrontOfficeFacade;

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidmanagerfrontoffice")
    @Transactional
    public RemoteInvocationResult getEidManagerFrontOfficeByName(@Valid @Payload String name) {
        EidManagerFrontOfficeDTO result = eidManagerFrontOfficeFacade.getEidManagerFrontOfficeByName(name);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidmanagerfrontoffice.getall.eidmanagerid")
    @Transactional
    public RemoteInvocationResult getAllEidManagerFrontOfficesByEidManagerId(@Valid @Payload UUID eidManagerId) {
        List<EidManagerFrontOfficeDTO> result = eidManagerFrontOfficeFacade.getAllEidManagerFrontOfficesByEidManagerId(eidManagerId);
        return new RemoteInvocationResult(result);
    }

    // for Employee of EidManager
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.external.api.v1.employee.eidmanagerfrontoffice.getall")
    @Transactional
    public RemoteInvocationResult getAllEidManagerFrontOfficesForEmployee() {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = null;

        if (context.isEidAuth() && context.getSystemId() == null) {
            throw new ValidationMVRException(MUST_BE_EMPLOYEE_OF_EID_MANAGER);
        }

        if (context.isEidAuth() && context.getSystemId() != null) {
            eidManagerId = UUID.fromString(context.getSystemId());
        }

        List<EidManagerFrontOfficeDTO> result = eidManagerFrontOfficeFacade.getAllEidManagerFrontOfficesByEidManagerId(eidManagerId);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidmanagerfrontoffice.getall.region.region")
    @Transactional
    public RemoteInvocationResult getAllEidManagerFrontOfficesByRegion(@Valid @Payload OfficesByRegionDTO dto) {
        List<EidManagerFrontOfficeDTO> result = eidManagerFrontOfficeFacade.getAllEidManagerFrontOfficesByRegion(dto);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.circs.eidmanagerfrontoffice.create")
    @Transactional
    public RemoteInvocationResult createEidManagerFrontOffice(@Valid @Payload EidManagerFrontOfficeDTO dto) {
        UserContext context = UserContextHolder.getFromServletContext();

        if (!context.isEidAuth() && dto.getIsActive() == null) {
            throw new ValidationMVRException(ErrorCode.IS_ACTIVE_CANNOT_BE_NULL);
        }

        EidManagerFrontOfficeResponseDTO responseDTO = buildEidManagerFrontOfficeResponseDTO(dto);
        EidManagerFrontOfficeResponseDTO result = eidManagerFrontOfficeFacade.createEidManagerFrontOffice(responseDTO);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.PUT.raeicei.api.v1.circs.eidmanagerfrontoffice.update")
    @Transactional
    public RemoteInvocationResult updateEidManagerFrontOffice(@Valid @Payload EidManagerFrontOfficeDTO dto) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        EidManagerFrontOfficeResponseDTO result = eidManagerFrontOfficeFacade.updateEidManagerFrontOffice(dto, eidManagerId);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidmanagerfrontoffice.id")
    @Transactional
    public RemoteInvocationResult getEidManagerFrontOfficeById(@Valid @Payload UUID id) {
        EidManagerFrontOfficeResponseDTO result = eidManagerFrontOfficeFacade.getEidManagerFrontOfficeById(id);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.DELETE.raeicei.external.api.v1.circs.eidmanagerfrontoffice.delete.id")
    public RemoteInvocationResult deleteEidManagerFrontOffice(@Valid @Payload UUID id) {
        eidManagerFrontOfficeFacade.deleteEidManagerFrontOffice(id);
        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
    }

    private static EidManagerFrontOfficeResponseDTO buildEidManagerFrontOfficeResponseDTO(EidManagerFrontOfficeDTO dto) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        EidManagerFrontOfficeResponseDTO responseDTO = new EidManagerFrontOfficeResponseDTO();
        responseDTO.setEidManagerId(eidManagerId);
        responseDTO.setName(dto.getName());
        responseDTO.setLocation(dto.getLocation());
        responseDTO.setRegion(dto.getRegion());
        responseDTO.setContact(dto.getContact());
        responseDTO.setCode(dto.getCode());
        responseDTO.setIsActive(dto.getIsActive());
        responseDTO.setLongitude(dto.getLongitude());
        responseDTO.setLatitude(dto.getLatitude());
        responseDTO.setWorkingHours(dto.getWorkingHours());
        responseDTO.setEmail(dto.getEmail());
        responseDTO.setDescription(dto.getDescription());
        return responseDTO;
    }
}
