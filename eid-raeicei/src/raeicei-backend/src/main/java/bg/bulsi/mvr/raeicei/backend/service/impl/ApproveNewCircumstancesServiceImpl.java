package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.pan_client.NotificationSender;
import bg.bulsi.mvr.raeicei.backend.mapper.DocumentMapper;
import bg.bulsi.mvr.raeicei.backend.mapper.NoteMapper;
import bg.bulsi.mvr.raeicei.backend.service.ApproveNewCircumstancesService;
import bg.bulsi.mvr.raeicei.backend.service.FileUploadService;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentNoteDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerStatus;
import bg.bulsi.mvr.raeicei.contract.dto.NewCircumstancesStatus;
import bg.bulsi.mvr.raeicei.contract.dto.NoteAndDocumentsDTO;
import bg.bulsi.mvr.raeicei.model.entity.Document;
import bg.bulsi.mvr.raeicei.model.entity.EidAdministrator;
import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.entity.Note;
import bg.bulsi.mvr.raeicei.model.entity.application.AbstractApplication;
import bg.bulsi.mvr.raeicei.model.pan.DirectEmailFactory;
import bg.bulsi.mvr.raeicei.model.repository.AbstractApplicationRepository;
import bg.bulsi.mvr.raeicei.model.repository.EidAdministratorRepository;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerRepository;
import bg.bulsi.mvr.raeicei.model.repository.NoteRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.time.LocalDate;
import java.util.Comparator;
import java.util.UUID;
import java.util.stream.Collectors;

import static bg.bulsi.mvr.common.exception.ErrorCode.ENTITY_NOT_FOUND;

@Slf4j
@Service
@RequiredArgsConstructor
public class ApproveNewCircumstancesServiceImpl implements ApproveNewCircumstancesService {

    private final EidManagerRepository eidManagerRepository;
    private final EidAdministratorRepository eidAdministratorRepository;
    private final AbstractApplicationRepository applicationRepository;
    private final NoteRepository noteRepository;
    private final DocumentMapper documentMapper;
    private final NoteMapper noteMapper;
    private final NotificationSender notificationSender;
    private final DirectEmailFactory directEmailFactory;
    private final FileUploadService fileUploadService;

    public void approveEidAdministrator(UUID id, NewCircumstancesStatus status, NoteAndDocumentsDTO noteAndDocumentsDTO) {
        approveEidManager(getEidAdministratorById(id), status, noteAndDocumentsDTO);
    }

    public void approveEidCenter(UUID id, NewCircumstancesStatus status, NoteAndDocumentsDTO noteAndDocumentsDTO) {
        approveEidManager(getEidManagerById(id), status, noteAndDocumentsDTO);
    }

    private void approveEidManager(EidManager eidManager, NewCircumstancesStatus newCircumstancesStatus, NoteAndDocumentsDTO noteAndDocumentsDTO) {

        if (NewCircumstancesStatus.APPROVED.equals(newCircumstancesStatus)) {

            if (!(EidManagerStatus.IN_REVIEW.equals(eidManager.getManagerStatus()) || EidManagerStatus.TECHNICAL_CHECK.equals(eidManager.getManagerStatus()))) {
                throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
            }

            eidManager.setManagerStatus(EidManagerStatus.ACTIVE);

            setIsActiveForEidManagersCollections(eidManager);

            if (eidManager instanceof EidAdministrator admin) {
                setIsActiveForEidAdministratorDevices(admin);
            }
        } else if (NewCircumstancesStatus.FOR_CORRECTION.equals(newCircumstancesStatus)) {

            if (!(EidManagerStatus.IN_REVIEW.equals(eidManager.getManagerStatus()) || EidManagerStatus.TECHNICAL_CHECK.equals(eidManager.getManagerStatus()))) {
                throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
            }

            eidManager.setManagerStatus(EidManagerStatus.PENDING_ATTACHMENTS);
        } else if (NewCircumstancesStatus.SUSPENDED.equals(newCircumstancesStatus)) {

            if (!EidManagerStatus.ACTIVE.equals(eidManager.getManagerStatus())) {
                throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
            }

            eidManager.setManagerStatus(EidManagerStatus.SUSPENDED);
        } else if (NewCircumstancesStatus.STOPPED.equals(newCircumstancesStatus)) {

            if (!(EidManagerStatus.SUSPENDED.equals(eidManager.getManagerStatus()) || EidManagerStatus.ACTIVE.equals(eidManager.getManagerStatus()))) {
                throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
            }

            eidManager.setManagerStatus(EidManagerStatus.STOP);
        } else if (NewCircumstancesStatus.ACTIVE.equals(newCircumstancesStatus)) {

            if (!EidManagerStatus.SUSPENDED.equals(eidManager.getManagerStatus())) {
                throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
            }

            eidManager.setManagerStatus(EidManagerStatus.ACTIVE);

            setIsActiveForEidManagersCollections(eidManager);

            if (eidManager instanceof EidAdministrator admin) {
                setIsActiveForEidAdministratorDevices(admin);
            }
        }

        Note note = noteMapper.mapToEntity(noteAndDocumentsDTO.getNote());
        note.setNewStatus(newCircumstancesStatus.toString());
        eidManager.getNotes().add(note);

        if (noteAndDocumentsDTO.getDocuments() != null) {
            noteAndDocumentsDTO.getDocuments().forEach(d -> {
                fileUploadService.createForEidManager(d, eidManager.getId(), true);
                note.getAttachmentsNames().add(d.getFileName());
            });
        }

        noteRepository.save(note);

        if (eidManager instanceof EidAdministrator admin) {
            this.notificationSender.sendDirectEmail(
                    directEmailFactory.approveEidAdministrator(eidManager.getEmail(), new Object[]{LocalDate.now(), eidManager.getCode(), eidManager.getManagerStatus()})
            );

            eidAdministratorRepository.save(admin);
        } else {
            this.notificationSender.sendDirectEmail(
                    directEmailFactory.approveEidCenter(eidManager.getEmail(), new Object[]{LocalDate.now(), eidManager.getCode(), eidManager.getManagerStatus()})
            );

            eidManagerRepository.save(eidManager);
        }
    }

    private static void setIsActiveForEidManagersCollections(EidManager eidManager) {
        if (eidManager.getAuthorizedPersons() != null && !eidManager.getAuthorizedPersons().isEmpty()) {
            eidManager.getAuthorizedPersons().forEach(person -> person.setIsActive(true));
        }

        if (eidManager.getProvidedServices() != null && !eidManager.getProvidedServices().isEmpty()) {
            eidManager.getProvidedServices().forEach(providedService -> providedService.setIsActive(true));
        }

        if (eidManager.getEidManagerFrontOffices() != null && !eidManager.getEidManagerFrontOffices().isEmpty()) {
            eidManager.getEidManagerFrontOffices().forEach(frontOffice -> frontOffice.setIsActive(true));
        }

        if (eidManager.getEmployees() != null && !eidManager.getEmployees().isEmpty()) {
            eidManager.getEmployees().forEach(employee -> employee.setIsActive(true));
        }
    }

    private static void setIsActiveForEidAdministratorDevices(EidAdministrator admin) {
        if (admin.getDevices() != null && !admin.getDevices().isEmpty()) {
            admin.getDevices().forEach(device -> device.setIsActive(true));
        }
    }

    public DocumentNoteDTO getDocumentsNotesForApplication(UUID applicationId) {
        AbstractApplication application = applicationRepository.findById(applicationId).orElseThrow(() ->
                new EntityNotFoundException(ENTITY_NOT_FOUND, "Application", applicationId.toString()));

        DocumentNoteDTO dto = new DocumentNoteDTO();
        dto.setAttachments(documentMapper.mapToDtoList(application.getAttachments().stream()
                .sorted(Comparator.comparing(Document::getCreateDate).reversed())
                .collect(Collectors.toList())));
        dto.setNotes(noteMapper.mapToDtoList(application.getNotes().stream()
                .sorted(Comparator.comparing(Note::getCreateDate).reversed())
                .collect(Collectors.toList())));
        return dto;
    }

    public DocumentNoteDTO getDocumentsNotesForEidManager(UUID eidManagerId) {
        EidManager eidManager = getEidManagerById(eidManagerId);

        DocumentNoteDTO dto = new DocumentNoteDTO();
        dto.setAttachments(documentMapper.mapToDtoList(eidManager.getAttachments().stream()
                .sorted(Comparator.comparing(Document::getCreateDate).reversed())
                .collect(Collectors.toList())));
        dto.setNotes(noteMapper.mapToDtoList(eidManager.getNotes().stream()
                .sorted(Comparator.comparing(Note::getCreateDate).reversed())
                .collect(Collectors.toList())));
        return dto;
    }

    public EidManager getEidManagerById(UUID id) {
        return eidManagerRepository.findById(id).orElseThrow(() -> new EntityNotFoundException(ENTITY_NOT_FOUND, "EidManager", id.toString()));
    }

    public EidAdministrator getEidAdministratorById(UUID id) {
        return eidAdministratorRepository.findById(id).orElseThrow(() -> new EntityNotFoundException(ENTITY_NOT_FOUND, "EidAdministrator", id.toString()));
    }
}
