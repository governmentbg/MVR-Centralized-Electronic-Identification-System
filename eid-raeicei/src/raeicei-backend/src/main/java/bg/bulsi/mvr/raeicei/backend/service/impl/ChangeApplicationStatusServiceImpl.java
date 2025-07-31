package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.pan_client.NotificationSender;
import bg.bulsi.mvr.raeicei.backend.mapper.EidManagerMapper;
import bg.bulsi.mvr.raeicei.backend.mapper.NoteMapper;
import bg.bulsi.mvr.raeicei.backend.service.ChangeApplicationStatusService;
import bg.bulsi.mvr.raeicei.backend.service.EidAdministratorService;
import bg.bulsi.mvr.raeicei.backend.service.EidCenterService;
import bg.bulsi.mvr.raeicei.backend.service.FileUploadService;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.EidAdministrator;
import bg.bulsi.mvr.raeicei.model.entity.EidCenter;
import bg.bulsi.mvr.raeicei.model.entity.EidManagerFrontOffice;
import bg.bulsi.mvr.raeicei.model.entity.Note;
import bg.bulsi.mvr.raeicei.model.entity.application.AbstractApplication;
import bg.bulsi.mvr.raeicei.model.entity.application.EidAdministratorApplication;
import bg.bulsi.mvr.raeicei.model.enums.ApplicationType;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;
import bg.bulsi.mvr.raeicei.model.pan.DirectEmailFactory;
import bg.bulsi.mvr.raeicei.model.repository.AbstractApplicationRepository;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerFrontOfficeRepository;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.security.SecureRandom;
import java.time.LocalDate;
import java.util.*;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;

@Slf4j
@Service
@RequiredArgsConstructor
public class ChangeApplicationStatusServiceImpl implements ChangeApplicationStatusService {

    private final AbstractApplicationRepository repository;
    private final EidManagerRepository eidManagerRepository;
    private final EidManagerFrontOfficeRepository eidManagerFrontOfficeRepository;
    private final EidAdministratorService eidAdministratorService;
    private final EidCenterService eidCenterService;
    private final FileUploadService fileUploadService;
    private final NoteMapper noteMapper;
    private final EidManagerMapper eidManagerMapper;
    private final NotificationSender notificationSender;
    private final DirectEmailFactory directEmailFactory;

    @Override
    public void changeApplicationStatus(UUID id, ApplicationStatus status, String eidManagerCode, NoteAndDocumentsDTO noteAndDocumentsDTO, boolean isInternal) {

        AbstractApplication application = getApplicationById(id);

        ApplicationStatus currentStatus = application.getStatus();

        if (!isStatusAllowed(currentStatus, status)) {
            throw new ValidationMVRException(STATUS_NOT_ALLOWED);
        }

        application.setStatus(status);

        Note note = noteMapper.mapToEntity(noteAndDocumentsDTO.getNote());

        note.setNewStatus(status.toString());

        if (noteAndDocumentsDTO.getDocuments() != null) {
            noteAndDocumentsDTO.getDocuments().forEach(d -> {
                fileUploadService.createForApplication(d, id, isInternal);
                note.getAttachmentsNames().add(d.getFileName());
            });
        }

        application.getNotes().add(note);

        if (ApplicationStatus.ACCEPTED.equals(application.getStatus())) {
            selectService(application, eidManagerCode);
        }

        this.notificationSender.sendDirectEmail(
                directEmailFactory.changeApplicationStatus(application.getApplicant().getEmail(), new Object[]{LocalDate.now(), application.getApplicationNumber().getId(), application.getStatus()})
        );
    }

    private void selectService(AbstractApplication application, String eidManagerCode) {
        if (ManagerType.EID_ADMINISTRATOR.equals(application.getManagerType()) && ApplicationType.REGISTER.equals(application.getApplicationType())) {
            checkEidManagerCode(eidManagerCode);

            for (EidManagerFrontOffice office : application.getEidManagerFrontOffices()) {
                if (office.getCode() == null) {
                    String officeCode = generateOfficeCode();
                    officeCode = checkGeneratedOfficeCode(officeCode);
                    office.setCode(officeCode);
                }
            }

            EidAdministratorDTO dto = eidManagerMapper.mapApplicationToAdministratorDto((EidAdministratorApplication) application);
            dto.setManagerStatus(EidManagerStatus.TECHNICAL_CHECK);
            dto.setCode(eidManagerCode);

            eidAdministratorService.createEidAdministrator(dto);

        } else if (ManagerType.EID_ADMINISTRATOR.equals(application.getManagerType())) {
            EidAdministrator administrator = (EidAdministrator) eidManagerRepository.findByEikNumberAndServiceType(application.getEikNumber(), ManagerType.EID_ADMINISTRATOR).orElseThrow(() ->
                    new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, application.getEikNumber(), application.getEikNumber()));

            EidAdministratorDTO dto = eidManagerMapper.mapApplicationToAdministratorDto((EidAdministratorApplication) application);

            if (ApplicationType.RESUME.equals(application.getApplicationType())) {

                if (!EidManagerStatus.SUSPENDED.equals(administrator.getManagerStatus())) {
                    throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
                }

                dto.setManagerStatus(EidManagerStatus.ACTIVE);

            } else if (ApplicationType.REVOKE.equals(application.getApplicationType())) {

                if (!EidManagerStatus.ACTIVE.equals(administrator.getManagerStatus())) {
                    throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
                }

                dto.setManagerStatus(EidManagerStatus.STOP);

            } else if (ApplicationType.STOP.equals(application.getApplicationType())) {

                if (!(EidManagerStatus.ACTIVE.equals(administrator.getManagerStatus()) || EidManagerStatus.SUSPENDED.equals(administrator.getManagerStatus()))) {
                    throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
                }

                dto.setManagerStatus(EidManagerStatus.SUSPENDED);
            }

            eidAdministratorService.updateEidAdministratorFlatData(dto);

        } else if (ManagerType.EID_CENTER.equals(application.getManagerType()) && ApplicationType.REGISTER.equals(application.getApplicationType())) {
            checkEidManagerCode(eidManagerCode);

            for (EidManagerFrontOffice office : application.getEidManagerFrontOffices()) {
                if (office.getCode() == null) {
                    String officeCode = generateOfficeCode();
                    officeCode = checkGeneratedOfficeCode(officeCode);
                    office.setCode(officeCode);
                }
            }

            EidCenterDTO dto = eidManagerMapper.mapApplicationToCenterDto(application);
            dto.setManagerStatus(EidManagerStatus.TECHNICAL_CHECK);
            dto.setCode(eidManagerCode);

            eidCenterService.createEidCenter(dto);

        } else if (ManagerType.EID_CENTER.equals(application.getManagerType())) {
            EidCenter center = (EidCenter) eidManagerRepository.findByEikNumberAndServiceType(application.getEikNumber(), ManagerType.EID_CENTER).orElseThrow(() ->
                    new EntityNotFoundException(EID_CENTER_NOT_FOUND, application.getEikNumber(), application.getEikNumber()));

            EidCenterDTO dto = eidManagerMapper.mapApplicationToCenterDto(application);

            if (ApplicationType.RESUME.equals(application.getApplicationType())) {

                if (!EidManagerStatus.SUSPENDED.equals(center.getManagerStatus())) {
                    throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
                }

                dto.setManagerStatus(EidManagerStatus.ACTIVE);

            } else if (ApplicationType.REVOKE.equals(application.getApplicationType())) {

                if (!EidManagerStatus.ACTIVE.equals(center.getManagerStatus())) {
                    throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
                }

                dto.setManagerStatus(EidManagerStatus.STOP);

            } else if (ApplicationType.STOP.equals(application.getApplicationType())) {

                if (!(EidManagerStatus.ACTIVE.equals(center.getManagerStatus()) || EidManagerStatus.SUSPENDED.equals(center.getManagerStatus()))) {
                    throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
                }

                dto.setManagerStatus(EidManagerStatus.SUSPENDED);
            }

            eidCenterService.updateEidCenterFlatData(dto);
        }
    }

    public AbstractApplication getApplicationById(UUID id) {
        return repository.findById(id).orElseThrow(() -> new EntityNotFoundException(ENTITY_NOT_FOUND, "Application", id.toString()));
    }

    public boolean isStatusAllowed(ApplicationStatus currentStatus, ApplicationStatus status) {
        Map<ApplicationStatus, List<ApplicationStatus>> validStatusesMap = new HashMap<>();
        validStatusesMap.put(ApplicationStatus.IN_REVIEW, List.of(ApplicationStatus.RETURNED_FOR_CORRECTION, ApplicationStatus.DENIED, ApplicationStatus.ACCEPTED));
        validStatusesMap.put(ApplicationStatus.ACTIVE, Collections.emptyList());
        validStatusesMap.put(ApplicationStatus.DENIED, Collections.emptyList());
        validStatusesMap.put(ApplicationStatus.RETURNED_FOR_CORRECTION, List.of(ApplicationStatus.IN_REVIEW));
        validStatusesMap.put(ApplicationStatus.ACCEPTED, List.of(ApplicationStatus.ACTIVE, ApplicationStatus.DENIED));

        List<ApplicationStatus> validStatusesList = validStatusesMap.get(currentStatus);
        return validStatusesList.contains(status);
    }

    private void checkEidManagerCode(String eidManagerCode) {
        if (eidManagerCode == null) {
            throw new ValidationMVRException(CODE_IS_REQUIRED);
        }
        if (eidManagerCode.length() > 3) {
            throw new ValidationMVRException(CODE_IS_TOO_LONG);
        }
        if (eidManagerRepository.existsByCode(eidManagerCode)) {
            throw new ValidationMVRException(ErrorCode.EID_MANAGER_WITH_THIS_CODE_ALREADY_EXISTS);
        }
    }

    public String generateOfficeCode() {
        String CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        SecureRandom RANDOM = new SecureRandom();

        StringBuilder generatedOfficeCode = new StringBuilder(4);
        for (int i = 0; i < 4; i++) {
            int index = RANDOM.nextInt(CHARACTERS.length());
            generatedOfficeCode.append(CHARACTERS.charAt(index));
        }
        return generatedOfficeCode.toString();
    }

    public String checkGeneratedOfficeCode(String generatedOfficeCode) {
        while (eidManagerFrontOfficeRepository.existsByCode(generatedOfficeCode)) {
            generatedOfficeCode = generateOfficeCode();
        }
        return generatedOfficeCode;
    }
}
