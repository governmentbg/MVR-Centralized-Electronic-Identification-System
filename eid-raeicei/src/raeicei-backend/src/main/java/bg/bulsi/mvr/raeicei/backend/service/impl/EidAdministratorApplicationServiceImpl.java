package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.raeicei.backend.mapper.EidAdministratorApplicationMapper;
import bg.bulsi.mvr.raeicei.backend.service.EidAdministratorApplicationService;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.*;
import bg.bulsi.mvr.raeicei.model.entity.application.EidAdministratorApplication;
import bg.bulsi.mvr.raeicei.model.enums.ApplicationType;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;
import bg.bulsi.mvr.raeicei.model.repository.*;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Objects;
import java.util.Optional;
import java.util.UUID;
import java.util.stream.Collectors;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;

@Slf4j
@Service
@RequiredArgsConstructor
public class EidAdministratorApplicationServiceImpl implements EidAdministratorApplicationService {

    private final EidAdministratorApplicationRepository repository;
    private final AbstractApplicationRepository abstractApplicationRepository;
    private final DocumentTypeRepository documentTypeRepository;
    private final EidManagerRepository eidManagerRepository;
    private final EidManagerFrontOfficeRepository eidManagerFrontOfficeRepository;
    private final EidAdministratorApplicationMapper mapper;

    @Value("${latitude.min-latitude}")
    private Double minLatitude;

    @Value("${latitude.max-latitude}")
    private Double maxLatitude;

    @Value("${longitude.min-longitude}")
    private Double minLongitude;

    @Value("${longitude.max-longitude}")
    private Double maxLongitude;

    @Override
    public EidAdministratorApplication create(EidAdministratorApplicationDTO dto) {
        if (bg.bulsi.mvr.raeicei.contract.dto.ApplicationType.REGISTER.equals(dto.getApplicationType())) {
            validateDTO(dto);
            validateDevices(dto);
        }

        Optional<EidManager> administrator = eidManagerRepository.findByEikNumberAndServiceType(dto.getEikNumber(), ManagerType.EID_ADMINISTRATOR);

        if (administrator.isPresent() && EidManagerStatus.STOP.equals(administrator.get().getManagerStatus())) {
            throw new ValidationMVRException(WRONG_APPLICATION_TYPE);
        }

        EidAdministratorApplication entity = mapper.mapToEntity(dto);

        if (dto.getAuthorizedApplicant()) {
            Contact applicant = mapper.mapToEntity(dto.getApplicant());
            entity.getAuthorizedPersons().add(applicant);
        }

        if ((dto.getAttachments() == null || dto.getAttachments().isEmpty()) && (dto.getNotes() == null || dto.getNotes().isEmpty())) {
            return repository.save(entity);
        }

        List<Document> documents = entity.getAttachments();

        for (Document document : documents) {
            if (document.getDocumentType().getId() != null) {
                UUID documentTypeId = document.getDocumentType().getId();
                DocumentType documentType = documentTypeRepository.findById(documentTypeId).orElseThrow(() -> new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, "DocumentType", documentTypeId.toString()));
                document.setDocumentType(documentType);
            }
        }

        List<Note> notes = entity.getNotes();
        for (Note note : notes) {
            note.setNewStatus(entity.getApplicationType().toString());
            note.getAttachmentsNames().addAll(documents.stream().map(Document::getFileName).collect(Collectors.toSet()));
        }

        return repository.save(entity);
    }

    @Override
    public EidAdministratorApplication getById(UUID id) {
        return repository.findById(id).orElseThrow(() -> new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, "EidAdministratorApplication", id.toString()));
    }

    @Override
    public Page<EidAdministratorApplicationShortDTO> getByFilter(EidApplicationFilter filter, Pageable pageable) {
        if (Objects.nonNull(pageable) && pageable.getSort().isEmpty()) {
            Sort sort = Sort.by(Sort.Direction.DESC, "createDate");
            pageable = PageRequest.of(pageable.getPageNumber(), pageable.getPageSize(), sort);
        }

        if (filter.getEidManagerId() != null) {
            EidManager eidManager = eidManagerRepository.findById(filter.getEidManagerId()).orElseThrow(() ->
                    new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, filter.getEidManagerId().toString(), filter.getEidManagerId().toString()));
            filter.setEikNumber(eidManager.getEikNumber());
        }

        return mapper.mapToDtoPage(repository.findByFilter(filter, filter.getApplicationType() != null ? ApplicationType.valueOf(filter.getApplicationType().name()) : null, pageable));
    }

    private void validateDTO(EidAdministratorApplicationDTO dto) {
        if (!dto.getAuthorizedApplicant() && (dto.getAuthorizedPersons() == null || dto.getAuthorizedPersons().isEmpty())) {
            throw new ValidationMVRException(AUTHORIZED_PERSON_LIST_IS_EMPTY);
        }

        if (dto.getEmployees() == null || dto.getEmployees().isEmpty()) {
            throw new ValidationMVRException(EMPLOYEE_LIST_IS_EMPTY);
        }

        if (dto.getEmployees().stream().noneMatch(e -> e.getRoles().contains("ADMINISTRATOR"))) {
            throw new ValidationMVRException(EMPLOYEE_MUST_BE_ADMIN);
        }

        Optional<EidManager> administrator = eidManagerRepository.findByEikNumberAndServiceType(dto.getEikNumber(), ManagerType.EID_ADMINISTRATOR);

        if (administrator.isPresent() && !EidManagerStatus.STOP.equals(administrator.get().getManagerStatus())) {
            throw new ValidationMVRException(ErrorCode.EID_MANAGER_ALREADY_EXISTS);
        }

        if (dto.getEidManagerFrontOffices() == null || dto.getEidManagerFrontOffices().isEmpty()) {
            throw new ValidationMVRException(FRONT_OFFICE_LIST_IS_EMPTY);
        }

        for (EidManagerFrontOfficeDTO office : dto.getEidManagerFrontOffices()) {
            if (office.getCode() != null && eidManagerFrontOfficeRepository.existsByCode(office.getCode())) {
                throw new ValidationMVRException(ErrorCode.EID_MANAGER_FRONT_OFFICE_WITH_THIS_CODE_ALREADY_EXISTS);
            }

            double latitude = 0.0;
            double longitude = 0.0;

//            try {
//                latitude = Double.parseDouble(office.getLatitude());
//                longitude = Double.parseDouble(office.getLongitude());
//            } catch (Exception e) {
//                throw new ValidationMVRException(ErrorCode.COORDINATES_MUST_BE_DECIMAL_NUMBER);
//            }
//
//            if (latitude < minLatitude || latitude > maxLatitude || longitude < minLongitude || longitude > maxLongitude) {
//                throw new ValidationMVRException(ErrorCode.COORDINATES_MUST_BE_WITHIN_BULGARIA);
//            }
        }

        if (dto.getDownloadUrl() == null || dto.getDownloadUrl().isBlank()) {
            throw new ValidationMVRException(ErrorCode.DOWNLOAD_URL_REQUIRED);
        }
    }

    private void validateDevices(EidAdministratorApplicationDTO dto) {
        if (dto.getDevices() == null || dto.getDevices().isEmpty()) {
            throw new ValidationMVRException(DEVICE_LIST_IS_EMPTY);
        }

        for (DeviceDTO device : dto.getDevices()) {
            if ((device.getAuthorizationLink() == null || device.getAuthorizationLink().isBlank()) && (device.getBackchannelAuthorizationLink() == null || device.getBackchannelAuthorizationLink().isBlank())) {
                throw new ValidationMVRException(LINK_REQUIRED);
            }
        }
    }
}
