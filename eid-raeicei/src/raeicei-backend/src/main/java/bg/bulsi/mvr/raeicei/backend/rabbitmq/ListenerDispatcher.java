package bg.bulsi.mvr.raeicei.backend.rabbitmq;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.dto.AuditData;
import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.audit_logger.model.MessageType;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.rabbitmq.consumer.DefaultRabbitListener;
import bg.bulsi.mvr.raeicei.backend.facade.*;
import bg.bulsi.mvr.raeicei.backend.mapper.*;
import bg.bulsi.mvr.raeicei.backend.service.*;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.Discount;
import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.entity.application.AbstractApplication;
import bg.bulsi.mvr.raeicei.model.entity.application.EidAdministratorApplication;
import bg.bulsi.mvr.raeicei.model.entity.application.EidCenterApplication;
import bg.bulsi.mvr.raeicei.model.entity.tariif.Tariff;
import bg.bulsi.mvr.raeicei.model.enums.Region;
import bg.bulsi.mvr.raeicei.model.repository.AbstractApplicationRepository;
import bg.bulsi.mvr.raeicei.model.repository.EidAdministratorRepository;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerRepository;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.amqp.support.converter.RemoteInvocationResult;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.messaging.handler.annotation.Header;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

import java.util.*;
import java.util.stream.Collectors;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;

@Slf4j
@Component
@RequiredArgsConstructor
public class ListenerDispatcher {

    private final DeviceFacade deviceFacade;
    private final FileUploadFacade fileUploadFacade;
    private final EidAdministratorApplicationFacade eidAdministratorApplicationFacade;
    private final EidCenterApplicationFacade eidCenterApplicationFacade;
    private final DocumentTypeFacade documentTypeFacade;
    private final TariffService tariffService;
    private final TariffMapper tariffMapper;
    private final DiscountService discountService;
    private final ApproveNewCircumstancesService approveNewCircumstancesService;
    private final EmployeeFacade employeeFacade;
    private final AuthorizedPersonFacade authorizedPersonFacade;
    private final CheckCodeService checkCodeService;
    private final ReportService reportService;
    private final EmployeeMapper employeeMapper;
    private final DiscountMapper discountMapper;
    private final RegionMapper regionMapper;
    private final ChangeApplicationStatusService changeApplicationStatusService;
    private final EidManagerRepository eidManagerRepository;
    private final EidAdministratorRepository administratorRepository;
    private final AbstractApplicationRepository applicationRepository;
    private final BaseAuditLogger auditLogger;
    private final EidAdministratorApplicationMapper eidAdministratorApplicationMapper;
    private final EidCenterApplicationMapper eidCenterApplicationMapper;
//    private final DeviceTariffService deviceTariffService;
//    private final DeviceTariffMapper deviceTariffMapper;

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.device.id")
    @Transactional
    public RemoteInvocationResult getDeviceById(@Valid @Payload UUID id) {
        DeviceDTO result = deviceFacade.getDeviceById(id);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.device.aei.aeiid")
    @Transactional
    public RemoteInvocationResult getDevicesByAdministratorId(@Valid @Payload UUID aeiId) {
        List<DeviceDTO> result = deviceFacade.getDevices4AdministratorId(aeiId);
        return new RemoteInvocationResult(result);
    }

    // for Employee of EidManager
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.external.api.v1.employee.device.aei")
    @Transactional
    public RemoteInvocationResult getDevicesForEmployeeByAdministratorId() {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID aeiId = null;

        if (context.isEidAuth() && context.getSystemId() == null) {
            throw new ValidationMVRException(MUST_BE_EMPLOYEE_OF_EID_MANAGER);
        }

        if (context.isEidAuth() && context.getSystemId() != null) {
            aeiId = UUID.fromString(context.getSystemId());
        }

        List<DeviceDTO> result = deviceFacade.getDevices4AdministratorId(aeiId);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.device")
    @Transactional
    public RemoteInvocationResult getDevicesByType(@Valid @Payload String type) {
        List<DeviceDTO> result = deviceFacade.getDevicesByType(type);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.device.getall")
    @Transactional
    public RemoteInvocationResult getAllDevices(@Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        List<DeviceDTO> result;

        if (AuditEventType.GET_EXT_ALL_DEVICES.name().equals(auditEventType.name())) {
            result = deviceFacade.getAllDevices();
        } else {
            result = deviceFacade.getDevices4AdministratorId(UUID.fromString(UserContextHolder.getFromServletContext().getEidAdministratorId()));
        }
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.circs.device.create")
    @Transactional
    public RemoteInvocationResult createDevice(@Valid @Payload DeviceDTO dto) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidAdministratorId = UUID.fromString(context.getSystemId());

        DeviceDTO result = deviceFacade.createDevice(dto, eidAdministratorId);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.external.api.v1.circs.device.create")
    @Transactional
    public RemoteInvocationResult createExtDevice(@Valid @Payload DeviceExtRequestDTO dto) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidAdministratorId = UUID.fromString(context.getSystemId());

        DeviceDTO result = deviceFacade.createExtDevice(dto, eidAdministratorId);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.PUT.raeicei.api.v1.circs.device.update")
    @Transactional
    public RemoteInvocationResult updateDevice(@Valid @Payload DeviceDTO dto) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidAdministratorId = UUID.fromString(context.getSystemId());

        DeviceDTO result = deviceFacade.updateDevice(dto, eidAdministratorId);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.PUT.raeicei.external.api.v1.circs.device.update")
    @Transactional
    public RemoteInvocationResult updateExtDevice(@Valid @Payload DeviceExtRequestDTO dto) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidAdministratorId = UUID.fromString(context.getSystemId());

        DeviceDTO result = deviceFacade.updateExtDevice(dto, eidAdministratorId);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.DELETE.raeicei.external.api.v1.circs.device.delete.id")
    public RemoteInvocationResult deleteDevice(@Valid @Payload UUID id) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidAdministratorId = UUID.fromString(context.getSystemId());

        deviceFacade.deleteDevice(id, eidAdministratorId);
        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.circs.tariff.create")
    @Transactional
    public RemoteInvocationResult createTariff(@Valid @Payload TariffDTO dto, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        TariffResponseDTO responseDTO = buildTariffResponseDTO(dto);
        Tariff entity = tariffService.createTariff(responseDTO);
        TariffResponseDTO returnDto = tariffMapper.mapToTariffDto(entity);
        logAudit((UUID) null, dto, auditEventType, UserContextHolder.getFromServletContext());
        return new RemoteInvocationResult(returnDto);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.tariff.getbydate")
    @Transactional
    public RemoteInvocationResult getTariffByDateAndEidManagerId(@Valid @Payload TariffDateDTO dto) {
        TariffDoubleCurrencyResponseDTO returnDto = tariffService.getTariffDoubleCurrencyByDateAndEidManagerId(dto);
        return new RemoteInvocationResult(returnDto);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.tariff.getall.eidmanagerid")
    @Transactional
    public RemoteInvocationResult getAllTariffsByEidManagerId(@Valid @Payload UUID eidManagerId) {
        List<TariffDoubleCurrencyResponseDTO> dtos = tariffService.getAllTariffsByEidManagerId(eidManagerId);;
        return new RemoteInvocationResult(dtos);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.tariff.id")
    @Transactional
    public RemoteInvocationResult getTariffById(@Valid @Payload UUID id) {
        TariffDoubleCurrencyResponseDTO returnDto = tariffService.getDoubleCurrencyTariffById(id);
        return new RemoteInvocationResult(returnDto);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.PUT.raeicei.api.v1.circs.tariff.update.id")
    @Transactional
    public RemoteInvocationResult updateTariff(@Valid @Payload Map<String, Object> payload, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        UUID id = ((UUID) payload.get("id"));
        TariffDTO tariffDTO = (TariffDTO) payload.get("tariffDTO");
        TariffResponseDTO result = tariffService.updateTariff(id, tariffDTO, eidManagerId);
        logAudit(id, tariffDTO, auditEventType, UserContextHolder.getFromServletContext());
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.DELETE.raeicei.api.v1.circs.tariff.delete.id")
    public RemoteInvocationResult deleteTariff(@Valid @Payload UUID id) {
        tariffService.deleteTariff(id);
        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.circs.discount.create")
    @Transactional
    public RemoteInvocationResult createDiscount(@Valid @Payload DiscountDTO dto) {
        DiscountResponseDTO responseDTO = buildDiscountResponseDTO(dto);
        Discount entity = discountService.createDiscount(responseDTO);
        DiscountResponseDTO returnDto = discountMapper.mapToResponseDto(entity);
        return new RemoteInvocationResult(returnDto);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.discount.getbydate")
    @Transactional
    public RemoteInvocationResult getDiscountByDateAndEidManagerId(@Valid @Payload DiscountDateDTO dto) {
        Discount entity = discountService.getDiscountByDateAndEidManagerId(dto);
        DiscountResponseDTO returnDto = discountMapper.mapToResponseDto(entity);
        return new RemoteInvocationResult(returnDto);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.discount.getall.eidmanagerid")
    @Transactional
    public RemoteInvocationResult getAllDiscountsByEidManagerId(@Valid @Payload UUID eidManagerId) {
        List<Discount> entities = discountService.getAllDiscountsByEidManagerId(eidManagerId);
        List<DiscountResponseDTO> dtos = discountMapper.mapToResponseDtoList(entities);
        return new RemoteInvocationResult(dtos);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.discount.id")
    @Transactional
    public RemoteInvocationResult getDiscountById(@Valid @Payload UUID id) {
        Discount entity = discountService.getDiscountById(id);
        DiscountResponseDTO returnDto = discountMapper.mapToResponseDto(entity);
        return new RemoteInvocationResult(returnDto);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.PUT.raeicei.api.v1.circs.discount.update.id")
    @Transactional
    public RemoteInvocationResult updateDiscount(@Valid @Payload Map<String, Object> payload) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        UUID id = ((UUID) payload.get("id"));
        DiscountDTO discountDTO = (DiscountDTO) payload.get("discountDTO");
        DiscountResponseDTO result = discountService.updateDiscount(id, discountDTO, eidManagerId);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.DELETE.raeicei.api.v1.circs.discount.delete.id")
    public RemoteInvocationResult deleteDiscount(@Valid @Payload UUID id) {
        discountService.deleteDiscount(id);
        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
    }

//    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.devicetariff.create")
//    @Transactional
//    public RemoteInvocationResult createDeviceTariff(@Valid @Payload DeviceTariffDTO dto) {
//        Tariff entity = tariffService.createTariff(dto);
//        DeviceTariffDTO returnDto = deviceTariffMapper.mapToDto(entity);
//        return new RemoteInvocationResult(returnDto);
//    }
//
//    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.devicetariff.getbydate")
//    @Transactional
//    public RemoteInvocationResult getDeviceTariffByDateAndEidManagerId(@Valid @Payload DeviceTariffDTO dto) {
//        Tariff entity = tariffService.getDiscountByDateAndEidAdministratorId(dto);
//        DeviceTariffDTO returnDto = tariffMapper.mapToDto(entity);
//        return new RemoteInvocationResult(returnDto);
//    }
//
//    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.devicetariff.getall.eidadministratorid")
//    @Transactional
//    public RemoteInvocationResult getAllDeviceTariffsByEidManagerId(@Valid @Payload UUID eidAdministratorId) {
//        List<Tariff> entities = tariffService.getAllDiscountsByEidAdministratorId(eidAdministratorId);
//        List<DeviceTariffDTO> dtos = deviceTariffMapper.mapToDtoList(entities);
//        return new RemoteInvocationResult(dtos);
//    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.calculatetariff")
    @Transactional
    public RemoteInvocationResult calculateTariff(@Valid @Payload CalculateTariff dto) {
        CalculateTariffResultDTO result = tariffService.calculateTariff(dto);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.document.get.id")
    @Transactional
    public RemoteInvocationResult downloadDocumentById(@Valid @Payload UUID id) {
        DocumentResponseDTO result = fileUploadFacade.getById(id);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.document.export.technicalcheck")
    @Transactional
    public RemoteInvocationResult downloadTechnicalCheckProtocol() {
        DocumentResponseDTO result = fileUploadFacade.downloadTechnicalCheckProtocol();
        return new RemoteInvocationResult(result);
    }

//    @Transactional
//    @DefaultRabbitListener(queues = "raeicei.rpcqueue.DELETE.raeicei.api.v1.document.delete.id")
//    public RemoteInvocationResult deleteDocument(@Valid @Payload UUID id) {
//        fileUploadFacade.delete(id);
//        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
//    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.application.document.upload.id")
    @Transactional
    public RemoteInvocationResult uploadDocumentForApplication(@Valid @Payload Map<String, Object> payload, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        UserContext context = UserContextHolder.getFromServletContext();

        UUID applicationId = ((UUID) payload.get("id"));
        List<DocumentDTO> dtos =  (List<DocumentDTO>) payload.get("documents");
        boolean isInternal;

        if (context.isEidAuth() && context.getEidentityId() == null) {
            throw new ValidationMVRException(EIDENTITY_REQUIRED);
        }

        if (dtos == null || dtos.isEmpty()) {
            throw new ValidationMVRException(FILE_ATTACHMENT_REQUIRED);
        }

        if (AuditEventType.CREATE_DOCUMENT.name().equals(auditEventType.name())) {
            dtos.forEach(d-> d.setIsOutgoing(true));
            isInternal = true;
        } else {
            isInternal = false;
        }

        return new RemoteInvocationResult(dtos.stream().map(dto -> fileUploadFacade.createForApplication(dto, applicationId, isInternal)).toList());
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.eidmanager.document.upload")
    @Transactional
    public RemoteInvocationResult uploadDocumentForEidManager(@Valid @Payload List<DocumentDTO> dtos, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        UserContext context = UserContextHolder.getFromServletContext();
        boolean isInternal;

        if (context.isEidAuth() && context.getSystemId() == null) {
            throw new ValidationMVRException(MUST_BE_EMPLOYEE_OF_EID_MANAGER);
        }

        if (dtos == null || dtos.isEmpty()) {
            throw new ValidationMVRException(FILE_ATTACHMENT_REQUIRED);
        }

        if (AuditEventType.CREATE_DOCUMENT.name().equals(auditEventType.name())) {
            dtos.forEach(d-> d.setIsOutgoing(true));
            isInternal = true;
        } else {
            isInternal = false;
        }

        UUID eidManagerId = UUID.fromString(context.getSystemId());

        return new RemoteInvocationResult(dtos.stream().map(dto -> fileUploadFacade.createForEidManager(dto, eidManagerId, isInternal)).toList());
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.eidadministratorapplication.create")
    @Transactional
    public RemoteInvocationResult createEidAdministratorApplication(@Valid @Payload EidAdministratorApplicationDTO dto, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        dto.getApplicant().setIsActive(true);

        if (dto.getAuthorizedApplicant() == null) {
            dto.setAuthorizedApplicant(false);
        }

        UserContext context = UserContextHolder.getFromServletContext();

        if (context.isEidAuth() && context.getSystemId() != null) {
            Optional<EidManager> emplManager = eidManagerRepository.findById(UUID.fromString(context.getSystemId()));

            if (emplManager.isPresent() && ApplicationType.REGISTER.equals(dto.getApplicationType()) && !EidManagerStatus.STOP.equals(emplManager.get().getManagerStatus())) {
                throw new ValidationMVRException(EID_MANAGER_ALREADY_EXISTS);
            } else if (emplManager.isPresent() && !dto.getEikNumber().equals(emplManager.get().getEikNumber())) {
                throw new ValidationMVRException(MUST_BE_EMPLOYEE_OF_THIS_EID_MANAGER);
            }
        }

        if (dto.getNotes() != null) {
            dto.getNotes().forEach(note -> note.setAuthorsUsername(context.getUsername()));

            if (AuditEventType.CREATE_ISSUE_APPLICATION.name().equals(auditEventType.name())) {
                dto.getNotes().forEach(note -> note.setIsOutgoing(true));
            }
        }

        if (dto.getAttachments() != null) {
            if (AuditEventType.CREATE_ISSUE_APPLICATION.name().equals(auditEventType.name())) {
                dto.getAttachments().forEach(document -> document.setIsOutgoing(true));
            }
        }

        EidAdministratorApplication application = eidAdministratorApplicationFacade.create(dto);
        logAudit(application, dto, auditEventType, UserContextHolder.getFromServletContext());

        EidAdministratorApplicationResponseDTO result = eidAdministratorApplicationMapper.mapToDto(application);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidadministratorapplication.id")
    @Transactional
    public RemoteInvocationResult getEidAdministratorApplicationByID(@Valid @Payload UUID id) {
        EidAdministratorApplicationResponseDTO result = eidAdministratorApplicationFacade.getByID(id);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidadministratorapplication.list")
    @Transactional
    public RemoteInvocationResult getEidAdministratorApplicationByFilter(EidApplicationFilter filter) {
        Pageable pageable = filter.getPageable();
        UserContext context = UserContextHolder.getFromServletContext();

        if (context.isEidAuth() && context.getEidentityId() != null && context.getSystemId() == null) {
            filter.setApplicantEid(UUID.fromString(context.getEidentityId()));
        } else if (context.isEidAuth() && context.getSystemId() != null) {
            filter.setEidManagerId(UUID.fromString(context.getSystemId()));
        }

        Page<EidAdministratorApplicationShortDTO> result = eidAdministratorApplicationFacade.getByFilter(filter, pageable);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.eidcenterapplication.create")
    @Transactional
    public RemoteInvocationResult createEidCenterApplication(@Valid @Payload EidCenterApplicationDTO dto, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        dto.getApplicant().setIsActive(true);

        if (dto.getAuthorizedApplicant() == null) {
            dto.setAuthorizedApplicant(false);
        }

        UserContext context = UserContextHolder.getFromServletContext();

        if (context.isEidAuth() && context.getSystemId() != null) {
            Optional<EidManager> emplManager = eidManagerRepository.findById(UUID.fromString(context.getSystemId()));

            if (emplManager.isPresent() && ApplicationType.REGISTER.equals(dto.getApplicationType()) && !EidManagerStatus.STOP.equals(emplManager.get().getManagerStatus())) {
                throw new ValidationMVRException(EID_MANAGER_ALREADY_EXISTS);
            } else if (emplManager.isPresent() && !dto.getEikNumber().equals(emplManager.get().getEikNumber())) {
                throw new ValidationMVRException(MUST_BE_EMPLOYEE_OF_THIS_EID_MANAGER);
            }
        }

        if (dto.getNotes() != null) {
            dto.getNotes().forEach(note -> note.setAuthorsUsername(context.getUsername()));

            if (AuditEventType.CREATE_ISSUE_APPLICATION.name().equals(auditEventType.name())) {
                dto.getNotes().forEach(note -> note.setIsOutgoing(true));
            }
        }

        if (dto.getAttachments() != null) {
            if (AuditEventType.CREATE_ISSUE_APPLICATION.name().equals(auditEventType.name())) {
                dto.getAttachments().forEach(document -> document.setIsOutgoing(true));
            }
        }

        EidCenterApplication application = eidCenterApplicationFacade.create(dto);
        logAudit(application, dto, auditEventType, UserContextHolder.getFromServletContext());

        EidCenterApplicationResponseDTO result = eidCenterApplicationMapper.mapToDto(application);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidcenterapplication.id")
    @Transactional
    public RemoteInvocationResult getEidCenterApplicationByID(@Valid @Payload UUID id) {
        EidCenterApplicationResponseDTO result = eidCenterApplicationFacade.getByID(id);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidcenterapplication.list")
    @Transactional
    public RemoteInvocationResult getEidCenterApplicationByFilter(EidApplicationFilter filter) {
        Pageable pageable = filter.getPageable();
        UserContext context = UserContextHolder.getFromServletContext();

        if (context.isEidAuth() && context.getEidentityId() != null && context.getSystemId() == null) {
            filter.setApplicantEid(UUID.fromString(context.getEidentityId()));
        } else if (context.isEidAuth() && context.getSystemId() != null) {
            filter.setEidManagerId(UUID.fromString(context.getSystemId()));
        }

        Page<EidCenterApplicationShortDTO> result = eidCenterApplicationFacade.getByFilter(filter, pageable);
        return new RemoteInvocationResult(result);
    }

	@DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.employee.list.systemid")
	public RemoteInvocationResult getAllEmployees(@Payload Map<String, Object> payload) {
        UUID systemId = (UUID) payload.get("systemId");
        Pageable pageable = (Pageable) payload.get("pageable");
		Page<EmployeeDTO> employees = employeeFacade.getAllEmployeeBySystemId(systemId, pageable);
		return new RemoteInvocationResult(employees);
	}

    // for Employee of EidManager
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.external.api.v1.employee.employee.list")
    public RemoteInvocationResult getAllEmployeesForEidManagerEmployee(@Payload Map<String, Object> payload) {
        Pageable pageable = (Pageable) payload.get("pageable");
        UserContext context = UserContextHolder.getFromServletContext();
        UUID systemId = null;

        if (context.isEidAuth() && context.getSystemId() == null) {
            throw new ValidationMVRException(MUST_BE_EMPLOYEE_OF_EID_MANAGER);
        }

        if (context.isEidAuth() && context.getSystemId() != null) {
            systemId = UUID.fromString(context.getSystemId());
        }

        Page<EmployeeDTO> employees = employeeFacade.getAllEmployeeBySystemId(systemId, pageable);
        return new RemoteInvocationResult(employees);
    }

	@DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidcenter.id.employee")
	public RemoteInvocationResult checkEmployee4CenterId(@Payload Map<String, Object> payload) {
		EmployeeByUidResult emplResult = employeeFacade.checkEmployee(payload,ServiceType.EID_CENTER);
		return new RemoteInvocationResult(emplResult);
	}

	@DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.eidadministrator.id.employee")
	public RemoteInvocationResult checkEmployee4AdministratorId(@Payload Map<String, Object> payload) {
		EmployeeByUidResult emplResult = employeeFacade.checkEmployee(payload,ServiceType.EID_ADMINISTRATOR);
		return new RemoteInvocationResult(emplResult);
	}

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.external.api.v1.circs.employee.create")
    @Transactional
    public RemoteInvocationResult createEmployee(@Valid @Payload EmployeeDTO dto) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        EmployeeDTO returnDto = employeeFacade.createEmployee(dto, eidManagerId);
        return new RemoteInvocationResult(returnDto);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.external.api.v1.employee.id")
    @Transactional
    public RemoteInvocationResult getEmployeeById(@Valid @Payload UUID id) {
        EmployeeDTO returnDto = employeeFacade.getEmployeeById(id);
        return new RemoteInvocationResult(returnDto);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.PUT.raeicei.external.api.v1.circs.employee.update.id")
    @Transactional
    public RemoteInvocationResult updateEmployee(@Valid @Payload Map<String, Object> payload) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        UUID id = ((UUID) payload.get("id"));
        EmployeeDTO employeeDTO = (EmployeeDTO) payload.get("employeeDTO");
        EmployeeDTO result = employeeFacade.updateEmployee(id, employeeDTO, eidManagerId);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.DELETE.raeicei.external.api.v1.circs.employee.delete.id")
    public RemoteInvocationResult deleteEmployee(@Valid @Payload UUID id) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        employeeFacade.deleteEmployee(id, eidManagerId);
        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.external.api.v1.circs.authorizedperson.create")
    @Transactional
    public RemoteInvocationResult createAuthorizedPerson(@Valid @Payload ContactDTO dto) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        ContactDTO returnDto = authorizedPersonFacade.createAuthorizedPerson(dto, eidManagerId);
        return new RemoteInvocationResult(returnDto);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.external.api.v1.authorizedperson.id")
    @Transactional
    public RemoteInvocationResult getAuthorizedPersonById(@Valid @Payload UUID id) {
        ContactDTO returnDto = authorizedPersonFacade.getAuthorizedPersonById(id);
        return new RemoteInvocationResult(returnDto);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.PUT.raeicei.external.api.v1.circs.authorizedperson.update.id")
    @Transactional
    public RemoteInvocationResult updateAuthorizedPerson(@Valid @Payload Map<String, Object> payload) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        UUID id = ((UUID) payload.get("id"));
        ContactDTO contactDTO = (ContactDTO) payload.get("contactDTO");
        ContactDTO result = authorizedPersonFacade.updateAuthorizedPerson(id, contactDTO, eidManagerId);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.DELETE.raeicei.external.api.v1.circs.authorizedperson.delete.id")
    public RemoteInvocationResult deleteAuthorizedPerson(@Valid @Payload UUID id) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        authorizedPersonFacade.deleteAuthorizedPerson(id, eidManagerId);
        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.document-type.create")
    @Transactional
    public RemoteInvocationResult createDocumentType(@Valid @Payload DocumentTypeDTO dto) {
        DocumentTypeResponseDTO result = documentTypeFacade.create(dto);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.PUT.raeicei.api.v1.document-type.update")
    @Transactional
    public RemoteInvocationResult updateDocumentType(@Valid @Payload DocumentTypeResponseDTO dto) {
        DocumentTypeResponseDTO result = documentTypeFacade.update(dto);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.DELETE.raeicei.api.v1.document-type.delete.id")
    public RemoteInvocationResult deleteDocumentType(@Valid @Payload UUID id) {
        documentTypeFacade.delete(id);
        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.document-type.id")
    @Transactional
    public RemoteInvocationResult documentTypeById(@Valid @Payload UUID id) {
        DocumentTypeResponseDTO result = documentTypeFacade.getByID(id);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.document-type.list")
    @Transactional
    public RemoteInvocationResult documentTypeGetAll() {
        List<DocumentTypeResponseDTO> result = documentTypeFacade.getAllDocumentTypes();
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.region.list")
    @Transactional
    public RemoteInvocationResult regionGetAll() {
        List<RegionDTO> result = regionMapper.mapResultList(Arrays.stream(Region.values()).collect(Collectors.toList()));
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.PUT.raeicei.api.v1.application-status.change.id")
    public RemoteInvocationResult changeApplicationStatus(@Valid @Payload Map<String, Object> payload, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        UUID id = ((UUID)payload.get("id"));
        ApplicationStatus applicationStatus = (ApplicationStatus) payload.get("applicationStatus");
        String code = (String) payload.get("code");
        NoteAndDocumentsDTO noteAndDocumentsDTO = (NoteAndDocumentsDTO) payload.get("noteAndDocumentsDTO");
        UserContext context = UserContextHolder.getFromServletContext();
        boolean isInternal = false;

        if (noteAndDocumentsDTO.getNote() == null) {
            throw new ValidationMVRException(NOTE_IS_REQUIRED);
        }

        noteAndDocumentsDTO.getNote().setAuthorsUsername(context.getUsername());

        if (AuditEventType.CHANGE_APPLICATION_STATUS.name().equals(auditEventType.name())) {
            noteAndDocumentsDTO.getNote().setIsOutgoing(true);
            isInternal = true;

            if (noteAndDocumentsDTO.getDocuments() != null) {
                noteAndDocumentsDTO.getDocuments().forEach(d -> {d.setIsOutgoing(true);});
            }
        }

        AbstractApplication application = changeApplicationStatusService.getApplicationById(id);
        logAudit(application, null, auditEventType, UserContextHolder.getFromServletContext());

        changeApplicationStatusService.changeApplicationStatus(id, applicationStatus, code, noteAndDocumentsDTO, isInternal);
        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.application-status.documentsnotes.id")
    public RemoteInvocationResult getDocumentsNotesForApplication(@Valid @Payload UUID id) {
        UserContext context = UserContextHolder.getFromServletContext();

        if (context.isEidAuth() && context.getEidentityId() == null) {
            throw new ValidationMVRException(EIDENTITY_REQUIRED);
        }

        Optional<AbstractApplication> application = applicationRepository.findById(id);

        if (context.isEidAuth() && application.isPresent() && !application.get().getApplicant().getEIdentity().equals(UUID.fromString(context.getEidentityId()))) {
            throw new ValidationMVRException(YOU_MUST_BE_THE_APPLICANT);
        }

        DocumentNoteDTO dto = approveNewCircumstancesService.getDocumentsNotesForApplication(id);
        return new RemoteInvocationResult(dto);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.circs.eidmanager.documentsnotes")
    public RemoteInvocationResult getDocumentsNotesForEidManager() {
        UserContext context = UserContextHolder.getFromServletContext();

        if (context.isEidAuth() && context.getSystemId() == null) {
            throw new ValidationMVRException(MUST_BE_EMPLOYEE_OF_EID_MANAGER);
        }

        UUID eidManagerId = UUID.fromString(context.getSystemId());

        DocumentNoteDTO dto = approveNewCircumstancesService.getDocumentsNotesForEidManager(eidManagerId);
        return new RemoteInvocationResult(dto);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.checkcode.code")
    public RemoteInvocationResult checkCode(@Valid @Payload Map<String, Object> payload) {
        String code = (String) payload.get("code");
        Boolean isOffice = (Boolean) payload.get("isOffice");

        if (code.length() > 3 && !isOffice) {
            throw new ValidationMVRException(CODE_IS_TOO_LONG);
        }

        if (code.length() > 4) {
            throw new ValidationMVRException(CODE_IS_TOO_LONG);
        }

        checkCodeService.checkCode(code, isOffice);
        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.reports.administrators")
    public RemoteInvocationResult getEidAdministratorsReport() {
        List<List<String>> result = reportService.getEidAdministratorsReport();
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.reports.centers")
    public RemoteInvocationResult getEidCentersReport() {
        List<List<String>> result = reportService.getEidCentersReport();
        return new RemoteInvocationResult(result);
    }

    private static TariffResponseDTO buildTariffResponseDTO(TariffDTO dto) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        TariffResponseDTO responseDTO = new TariffResponseDTO();
        responseDTO.setStartDate(dto.getStartDate());
        responseDTO.setPrice(dto.getPrice());
        responseDTO.setEidManagerId(eidManagerId);
        responseDTO.setProvidedServiceId(dto.getProvidedServiceId());
        responseDTO.setCurrency(TariffResponseDTO.CurrencyEnum.valueOf(dto.getCurrency().toString()));
        return responseDTO;
    }

    private static DiscountResponseDTO buildDiscountResponseDTO(DiscountDTO dto) {
        UserContext context = UserContextHolder.getFromServletContext();
        UUID eidManagerId = UUID.fromString(context.getSystemId());

        DiscountResponseDTO responseDTO = new DiscountResponseDTO();
        responseDTO.setStartDate(dto.getStartDate());
        responseDTO.setAgeFrom(dto.getAgeFrom());
        responseDTO.setAgeUntil(dto.getAgeUntil());
        responseDTO.setDiscount(dto.getDiscount());
        responseDTO.setDisability(dto.getDisability());
        responseDTO.setOnlineService(dto.getOnlineService());
        responseDTO.setEidManagerId(eidManagerId);
        responseDTO.setProvidedServiceId(dto.getProvidedServiceId() != null
                ? dto.getProvidedServiceId()
                : null);
        return responseDTO;
    }

    private void logAudit(AbstractApplication application, Object dto, AuditEventType auditEventType, UserContext userContext) {
        EventPayload eventPayloadReq = new EventPayload();
        eventPayloadReq.setRequesterUid(userContext.getCitizenIdentifier());
        eventPayloadReq.setRequesterUidType(userContext.getCitizenIdentifierType());
        eventPayloadReq.setRequesterName(userContext.getName());
        eventPayloadReq.setTargetUid(application.getApplicant().getCitizenIdentifierNumber());
        eventPayloadReq.setTargetUidType(application.getApplicant().getCitizenIdentifierType().name());
        eventPayloadReq.setTargetName(application.getApplicant().getName());
        eventPayloadReq.setApplicationId(application.getId().toString());
        eventPayloadReq.setApplicationStatus(application.getStatus().name());
        eventPayloadReq.setApplicationType(application.getApplicationType().name());

        this.auditLogger.auditEvent(AuditData.builder()
                .correlationId(userContext.getGlobalCorrelationId().toString())
                .eventType(auditEventType)
                .messageType(MessageType.REQUEST)
                .requesterUserId(userContext.getRequesterUserId())
                .requesterSystemId(userContext.getSystemId())
                .requesterSystemName(userContext.getSystemName())
                .targetUserId(userContext.getEidentityId() != null ? userContext.getEidentityId() : null)
                .payload(eventPayloadReq)
                .build());
    }

    private void logAudit(UUID id, Object request, AuditEventType auditEventType, UserContext userContext) {
        EventPayload eventPayloadReq = new EventPayload();
        eventPayloadReq.setRequesterUid(userContext.getCitizenIdentifier());
        eventPayloadReq.setRequesterUidType(userContext.getCitizenIdentifierType());
        eventPayloadReq.setRequesterName(userContext.getName());
        if (id != null) {
            eventPayloadReq.addAdditionalParam("id", id);
        }
        eventPayloadReq.setRequestBody(request);

        this.auditLogger.auditEvent(AuditData.builder()
                .correlationId(userContext.getGlobalCorrelationId().toString())
                .eventType(auditEventType)
                .messageType(MessageType.REQUEST)
                .requesterUserId(userContext.getRequesterUserId())
                .requesterSystemId(userContext.getSystemId())
                .requesterSystemName(userContext.getSystemName())
                .targetUserId(userContext.getEidentityId() != null ? userContext.getEidentityId() : null)
                .payload(eventPayloadReq)
                .build());
    }
}
