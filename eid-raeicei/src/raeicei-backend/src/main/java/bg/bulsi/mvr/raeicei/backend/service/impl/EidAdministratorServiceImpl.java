package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.raeicei.backend.mapper.*;
import bg.bulsi.mvr.raeicei.backend.service.DeviceService;
import bg.bulsi.mvr.raeicei.backend.service.EidAdministratorService;
import bg.bulsi.mvr.raeicei.backend.service.EidManagerService;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import bg.bulsi.mvr.raeicei.model.entity.Document;
import bg.bulsi.mvr.raeicei.model.entity.EidAdministrator;
import bg.bulsi.mvr.raeicei.model.entity.Note;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;
import bg.bulsi.mvr.raeicei.model.repository.EidAdministratorRepository;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerRepository;
import bg.bulsi.mvr.raeicei.model.repository.NoteRepository;
import bg.bulsi.mvr.raeicei.model.repository.view.EidAdministratorAuthorizedView;
import bg.bulsi.mvr.raeicei.model.repository.view.EidAdministratorView;
import bg.bulsi.mvr.raeicei.model.repository.view.ReportOfEidManagers;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.util.StringUtils;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.EID_ADMINISTRATOR_NOT_FOUND;

@Slf4j
@Service
@RequiredArgsConstructor
public class EidAdministratorServiceImpl implements EidAdministratorService {

    private final EidManagerRepository repository;
    private final EidAdministratorRepository administratorRepository;
    private final EidManagerMapper mapper;
    private final DeviceMapper deviceMapper;
    private final AuthorizedPersonMapper authorizedPersonMapper;
    private final ProvidedServiceMapper providedServiceMapper;
    private final EidManagerFrontOfficeMapper eidManagerFrontOfficeMapper;
    private final EmployeeMapper employeeMapper;
    private final EidManagerService eidManagerService;
    private final DeviceService deviceService;
    private final FileUploadServiceImpl fileUploadService;
    private final NoteRepository noteRepository;

    @Override
    public EidAdministrator getEidAdministratorById(UUID id) {
        return (EidAdministrator) repository.findById(id).orElseThrow(() -> new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, id.toString(), id.toString()));
    }

    public EidAdministrator getEidAdministratorByEikNumber(String eikNumber) {
        return (EidAdministrator) repository.findByEikNumber(eikNumber).orElseThrow(() -> new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, eikNumber.toString(), eikNumber.toString()));
    }

    @Override
    public EidAdministratorDTO findEidAdministratorById(UUID id) {
        EidAdministratorView administrator = repository.getEidAdministratorById(id).orElseThrow(() -> new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, id.toString(), id.toString()));
        EidAdministratorDTO administratorDto = mapper.map(administrator);
        log.debug("Administrator Code mapping: {} - {}", administrator.getCode(), administratorDto.getCode());
        return administratorDto;
    }

    @Override
    public EidAdministratorAuthorizedDTO findEidAdministratorWithAuthorizedPersonsById(UUID id) {
        EidAdministratorAuthorizedView administrator = repository.getEidAdministratorAuthorizedById(id).orElseThrow(() -> new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, id.toString(), id.toString()));
        EidAdministratorAuthorizedDTO administratorAuthorizedDto = mapper.map(administrator);
        log.debug("Administrator Code mapping: {} - {}", administrator.getCode(), administratorAuthorizedDto.getCode());
        return administratorAuthorizedDto;
    }

    @Override
    public EidAdministratorFullDTO findFullEidAdministratorById(UUID id, Boolean onlyActiveElements) {
        EidAdministrator administrator = administratorRepository.findById(id).orElseThrow(() -> new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, id.toString(), id.toString()));
        EidAdministratorFullDTO administratorFullDto = mapper.mapToAdministratorFullDto(administrator);

        if (onlyActiveElements) {
            List<DeviceDTO> devices = administrator.getDevices().stream().filter(d -> Boolean.TRUE.equals(d.getIsActive())).map(deviceMapper::mapToDto).toList();
            administratorFullDto.setDevices(devices);

            List<ContactDTO> authorizedPersons = administrator.getAuthorizedPersons().stream().filter(ap -> Boolean.TRUE.equals(ap.getIsActive())).map(authorizedPersonMapper::mapToAuthorizedPersonDto).toList();
            administratorFullDto.setAuthorizedPersons(authorizedPersons);

            List<ProvidedServiceResponseDTO> providedServices = administrator.getProvidedServices().stream().filter(ps -> Boolean.TRUE.equals(ps.getIsActive())).map(providedServiceMapper::toResponseDto).toList();
            administratorFullDto.setProvidedServices(providedServices);

            List<EidManagerFrontOfficeResponseDTO> eidManagerFrontOffices = administrator.getEidManagerFrontOffices().stream().filter(o -> Boolean.TRUE.equals(o.getIsActive())).map(eidManagerFrontOfficeMapper::mapToResponseDto).toList();
            administratorFullDto.setEidManagerFrontOffices(eidManagerFrontOffices);

            List<EmployeeDTO> employees = administrator.getEmployees().stream().filter(e -> Boolean.TRUE.equals(e.getIsActive())).map(employeeMapper::mapToEmployeeDto).toList();
            administratorFullDto.setEmployees(employees);
        }

        log.debug("Administrator Code mapping: {} - {}", administrator.getCode(), administratorFullDto.getCode());
        return administratorFullDto;
    }

    @Override
    public List<EidAdministratorDTO> getAllEidAdministrators() {
        // List<EidAdministratorView> administrators = repository.getAllEidAdministratorsByQuery();
//        List<EidAdministratorView> administrators = repository.findAllEidAdministrators();
        List<EidAdministratorView> administrators = repository.getAllActiveEidAdministratorsFromAuditByQuery();
        return mapper.map(administrators);
    }

    @Override
    public List<EidAdministratorHistoryDTO> getAllEidAdministratorsHistory(Boolean stoppedIncluded) {
        List<EidAdministratorView> administrators;
        if (stoppedIncluded) {
            administrators = repository.getAllEidAdministratorsHistoryByQuery();
        } else {
            administrators = repository.getAllActiveOrSuspendedEidAdministratorsFromAuditByQuery();
        }
        return mapper.mapToHistoryDtoList(administrators);
    }

    @Override
    public List<EidAdministratorDTO> getAllEidAdministratorsByStatus(EidManagerStatus eidManagerStatus) {
        List<EidAdministratorView> administrators = repository.getAllEidAdministratorsByStatus(eidManagerStatus.toString());
        return mapper.map(administrators);
    }

    @Override
    public EidAdministrator createEidAdministrator(EidAdministratorDTO dto) {
        validateEidAdministratorDTO(dto, true);

        EidAdministrator entity = mapper.mapToEntity(dto);

        this.eidManagerService.addFrontOfficesToEidManager(entity, dto.getEidManagerFrontOfficeIds());

        this.eidManagerService.addAuthorizedPersonsToEidManager(entity, dto.getAuthorizedPersonsIds());

        this.eidManagerService.addEmployeesToEidManager(entity, dto.getEmployeesIds());

        this.eidManagerService.addProvidedServicesToEidManager(entity);

        this.addDevicesToEidAdministrator(dto, entity);

        this.addAttachmentsToEidAdministrator(dto, entity);

        this.addNotesToEidAdministrator(dto, entity);

        return repository.save(entity);
    }

    @Override
    @Transactional
    public EidAdministrator updateEidAdministrator(EidAdministratorDTO dto) {
        validateEidAdministratorDTO(dto, false);

        EidAdministrator entity = getEidAdministratorById(dto.getId());
        mapper.mapToEntity(entity, dto);

        this.

                addDevicesToEidAdministrator(dto, entity);

        this.eidManagerService.addAuthorizedPersonsToEidManager(entity, dto.getAuthorizedPersonsIds());

        this.eidManagerService.addEmployeesToEidManager(entity, dto.getEmployeesIds());

        this.eidManagerService.addProvidedServicesToEidManager(entity);

        this.addAttachmentsToEidAdministrator(dto, entity);

        this.addNotesToEidAdministrator(dto, entity);

        return repository.save(entity);
    }

    @Transactional
    @Override
    public EidAdministrator updateEidAdministratorFlatData(EidAdministratorDTO dto) {
        validateEidAdministratorDTO(dto, false);

        EidAdministrator entity = getEidAdministratorByEikNumber(dto.getEikNumber());

        updateEidAdministratorEntityFlatData(entity, dto);

        return repository.save(entity);
    }

    @Override
    @Transactional
    public List<ReportOfEidManagers> getEidAdministratorsReport() {
        return repository.getEidAdministratorsReport();
    }

    private void validateEidAdministratorDTO(EidAdministratorDTO dto, boolean isCreate) {
        if (!StringUtils.hasText(dto.getName())) {
            throw new ValidationMVRException(ErrorCode.EID_ADMINISTRATOR_NAME_REQUIRED);
        }
        if (!StringUtils.hasText(dto.getNameLatin())) {
            throw new ValidationMVRException(ErrorCode.EID_ADMINISTRATOR_NAME_LATIN_REQUIRED);
        }
        if (!StringUtils.hasText(dto.getEikNumber())) {
            throw new ValidationMVRException(ErrorCode.EID_ADMINISTRATOR_EIK_NUMBER_REQUIRED);
        }
        if (isCreate && !StringUtils.hasText(dto.getDownloadUrl())) {
            throw new ValidationMVRException(ErrorCode.DOWNLOAD_URL_REQUIRED);
        }
    }

    private void addDevicesToEidAdministrator(EidAdministratorDTO dto, EidAdministrator entity) {
        if (dto.getDeviceIds() != null) {
            List<Device> devices = new ArrayList<>();
            for (UUID deviceId : dto.getDeviceIds()) {
                Device device = deviceService.getDeviceById(deviceId);
                devices.add(device);
            }
            entity.setDevices(devices);
        }
    }

    private void addAttachmentsToEidAdministrator(EidAdministratorDTO dto, EidAdministrator entity) {
        if (dto.getAttachmentIds() != null) {
            List<Document> attachments = new ArrayList<>();
            for (UUID attachmentId : dto.getAttachmentIds()) {
                Document document = fileUploadService.getById(attachmentId);
                attachments.add(document);
            }
            entity.setAttachments(attachments);
        }
    }

    private void addNotesToEidAdministrator(EidAdministratorDTO dto, EidAdministrator entity) {
        if (dto.getNoteIds() != null) {
            List<Note> notes = new ArrayList<>();
            for (UUID noteId : dto.getNoteIds()) {
                Note note = noteRepository.getReferenceById(noteId);
                notes.add(note);
            }
            entity.setNotes(notes);
        }
    }

    public void updateEidAdministratorEntityFlatData(EidAdministrator entity, EidAdministratorDTO dto) {
        if (dto.getName() != null) {
            entity.setName(dto.getName());
        }
        if (dto.getNameLatin() != null) {
            entity.setNameLatin(dto.getNameLatin());
        }
        if (dto.getEikNumber() != null) {
            entity.setEikNumber(dto.getEikNumber());
        }
        if (dto.getCode() != null) {
            entity.setCode(dto.getCode());
        }
        if (dto.getAddress() != null) {
            entity.setAddress(dto.getAddress());
        }
        if (dto.getEmail() != null) {
            entity.setEmail(dto.getEmail());
        }
        if (dto.getHomePage() != null) {
            entity.setHomePage(dto.getHomePage());
        }
        if (dto.getServiceType() != null) {
            entity.setServiceType(ManagerType.valueOf(dto.getServiceType().toString()));
        }
        if (dto.getManagerStatus() != null) {
            entity.setManagerStatus(dto.getManagerStatus());
        }
        if (dto.getLogoUrl() != null) {
            entity.setLogoUrl(dto.getLogoUrl());
        }
        if (dto.getDownloadUrl() != null) {
            entity.setDownloadUrl(dto.getDownloadUrl());
        }
    }
}