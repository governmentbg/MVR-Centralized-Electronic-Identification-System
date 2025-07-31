package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.raeicei.backend.mapper.*;
import bg.bulsi.mvr.raeicei.backend.service.EidCenterService;
import bg.bulsi.mvr.raeicei.backend.service.EidManagerService;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.Document;
import bg.bulsi.mvr.raeicei.model.entity.EidCenter;
import bg.bulsi.mvr.raeicei.model.entity.Note;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerRepository;
import bg.bulsi.mvr.raeicei.model.repository.NoteRepository;
import bg.bulsi.mvr.raeicei.model.repository.view.EidCenterAuthorizedView;
import bg.bulsi.mvr.raeicei.model.repository.view.EidCenterView;
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
import static bg.bulsi.mvr.common.exception.ErrorCode.EID_CENTER_NOT_FOUND;

@Slf4j
@Service
@RequiredArgsConstructor
public class EidCenterServiceImpl implements EidCenterService {

    private final EidManagerRepository eidManagerRepository;

    private final EidManagerMapper eidManagerMapper;

    private final AuthorizedPersonMapper authorizedPersonMapper;

    private final ProvidedServiceMapper providedServiceMapper;

    private final EidManagerFrontOfficeMapper eidManagerFrontOfficeMapper;

    private final EmployeeMapper employeeMapper;

    private final EidManagerService eidManagerService;

    private final FileUploadServiceImpl fileUploadService;

    private final NoteRepository noteRepository;

    @Override
    public EidCenterDTO getEidCenterById(UUID id) {
        EidCenterView eidCenterView = eidManagerRepository.getEidCenterByIdWithQuery(id).orElseThrow(() -> new EntityNotFoundException(EID_CENTER_NOT_FOUND, id.toString(), id.toString()));

        return this.eidManagerMapper.mapViewToEidCenterDto(eidCenterView);
    }

    @Override
    public EidCenterAuthorizedDTO getCenterWithAuthorizedPersonsById(UUID id) {
        EidCenterAuthorizedView eidCenterView = eidManagerRepository.getEidCenterAuthorizedById(id).orElseThrow(() -> new EntityNotFoundException(EID_CENTER_NOT_FOUND, id.toString(), id.toString()));

        return this.eidManagerMapper.mapViewToEidCenterAuthorizedDto(eidCenterView);
    }

    @Override
    public EidCenterFullDTO getFullEidCenterById(UUID id, Boolean onlyActiveElements) {
        EidCenter eidCenter = eidManagerRepository.findEidCenterById(id).orElseThrow(() -> new EntityNotFoundException(EID_CENTER_NOT_FOUND, id.toString(), id.toString()));
        EidCenterFullDTO centerFullDTO = eidManagerMapper.mapToEidCenterFullDto(eidCenter);

        if (onlyActiveElements) {
            List<ContactDTO> authorizedPersons = eidCenter.getAuthorizedPersons().stream().filter(ap -> Boolean.TRUE.equals(ap.getIsActive())).map(authorizedPersonMapper::mapToAuthorizedPersonDto).toList();
            centerFullDTO.setAuthorizedPersons(authorizedPersons);

            List<ProvidedServiceResponseDTO> providedServices = eidCenter.getProvidedServices().stream().filter(ps -> Boolean.TRUE.equals(ps.getIsActive())).map(providedServiceMapper::toResponseDto).toList();
            centerFullDTO.setProvidedServices(providedServices);

            List<EidManagerFrontOfficeResponseDTO> eidManagerFrontOffices = eidCenter.getEidManagerFrontOffices().stream().filter(o -> Boolean.TRUE.equals(o.getIsActive())).map(eidManagerFrontOfficeMapper::mapToResponseDto).toList();
            centerFullDTO.setEidManagerFrontOffices(eidManagerFrontOffices);

            List<EmployeeDTO> employees = eidCenter.getEmployees().stream().filter(e -> Boolean.TRUE.equals(e.getIsActive())).map(employeeMapper::mapToEmployeeDto).toList();
            centerFullDTO.setEmployees(employees);
        }

        return centerFullDTO;
    }

    public EidCenter getEidCenterByEikNumber(String eikNumber) {
        return (EidCenter) eidManagerRepository.findByEikNumber(eikNumber).orElseThrow(() -> new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, eikNumber.toString(), eikNumber.toString()));
    }

    @Override
    public EidCenter getEidCenterEntityById(UUID id) {
        return eidManagerRepository.findEidCenterById(id).orElseThrow(() -> new EntityNotFoundException(EID_CENTER_NOT_FOUND, id.toString(), id.toString()));
    }

    @Override
    public List<EidCenterDTO> getAllEidCenters() {
//    	List<EidCenterView> eidCenters = this.eidManagerRepository.getAllEidCentersByQuery();
        List<EidCenterView> eidCenters = this.eidManagerRepository.getAllActiveEidCentersFromAuditByQuery();

        return eidManagerMapper.mapViewToEidCenterDtoList(eidCenters);
    }

    @Override
    public List<EidCenterHistoryDTO> getAllEidCentersHistory(Boolean stoppedIncluded) {
        List<EidCenterView> eidCenters;

        if (stoppedIncluded) {
            eidCenters = this.eidManagerRepository.getAllEidCentersHistoryByQuery();
        } else {
            eidCenters = this.eidManagerRepository.getAllActiveOrSuspendedEidCentersFromAuditByQuery();
        }

        return eidManagerMapper.mapToHistoryDto(eidCenters);
    }

    @Override
    public List<EidCenterDTO> getAllEidCentersByStatus(EidManagerStatus eidManagerStatus) {
        List<EidCenterView> eidCenters = this.eidManagerRepository.getAllEidCentersByStatus(eidManagerStatus.toString());

        return eidManagerMapper.mapViewToEidCenterDtoList(eidCenters);
    }

    @Override
    public EidCenter createEidCenter(EidCenterDTO dto) {
        validateEidCenterDTO(dto);

        EidCenter entity = eidManagerMapper.mapToEidCenterEntity(dto);

        this.eidManagerService.addFrontOfficesToEidManager(entity, dto.getEidManagerFrontOfficeIds());

        this.eidManagerService.addAuthorizedPersonsToEidManager(entity, dto.getAuthorizedPersonsIds());

        this.eidManagerService.addEmployeesToEidManager(entity, dto.getEmployeesIds());

        this.eidManagerService.addProvidedServicesToEidManager(entity);

        this.addAttachmentsToEidAdministrator(dto, entity);

        this.addNotesToEidAdministrator(dto, entity);

        return eidManagerRepository.save(entity);
    }

    @Override
    public EidCenter updateEidCenter(EidCenterDTO dto) {
        validateEidCenterDTO(dto);

        EidCenter entity = getEidCenterEntityById(dto.getId());
        //eidManagerMapper.mapToEidCenterEntity(entity, dto);

        this.eidManagerService.addFrontOfficesToEidManager(entity, dto.getEidManagerFrontOfficeIds());

        this.eidManagerService.addAuthorizedPersonsToEidManager(entity, dto.getAuthorizedPersonsIds());

        this.eidManagerService.addEmployeesToEidManager(entity, dto.getEmployeesIds());

        this.eidManagerService.addProvidedServicesToEidManager(entity);

        this.addAttachmentsToEidAdministrator(dto, entity);

        this.addNotesToEidAdministrator(dto, entity);

        return eidManagerRepository.save(entity);
    }

    @Override
    public EidCenter updateEidCenterFlatData(EidCenterDTO dto) {
        validateEidCenterDTO(dto);

        EidCenter entity = getEidCenterByEikNumber(dto.getEikNumber());

        updateEidCenterEntityFlatData(entity, dto);

        return eidManagerRepository.save(entity);
    }

    @Override
    @Transactional
    public List<ReportOfEidManagers> getEidCentersReport() {
        return eidManagerRepository.getEidCentersReport();
    }

    private void validateEidCenterDTO(EidCenterDTO dto) {
        if (!StringUtils.hasText(dto.getName())) {
            throw new ValidationMVRException(ErrorCode.EID_CENTER_NAME_REQUIRED);
        }
        if (!StringUtils.hasText(dto.getNameLatin())) {
            throw new ValidationMVRException(ErrorCode.EID_CENTER_NAME_LATIN_REQUIRED);
        }
        if (!StringUtils.hasText(dto.getEikNumber())) {
            throw new ValidationMVRException(ErrorCode.EID_CENTER_EIK_NUMBER_REQUIRED);
        }
    }

    public void updateEidCenterEntityFlatData(EidCenter entity, EidCenterDTO dto) {
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
    }

    private void addAttachmentsToEidAdministrator(EidCenterDTO dto, EidCenter entity) {
        if (dto.getAttachmentIds() != null) {
            List<Document> attachments = new ArrayList<>();
            for (UUID attachmentId : dto.getAttachmentIds()) {
                Document document = fileUploadService.getById(attachmentId);
                attachments.add(document);
            }
            entity.setAttachments(attachments);
        }
    }

    private void addNotesToEidAdministrator(EidCenterDTO dto, EidCenter entity) {
        if (dto.getNoteIds() != null) {
            List<Note> notes = new ArrayList<>();
            for (UUID noteId : dto.getNoteIds()) {
                Note note = noteRepository.getReferenceById(noteId);
                notes.add(note);
            }
            entity.setNotes(notes);
        }
    }
}
