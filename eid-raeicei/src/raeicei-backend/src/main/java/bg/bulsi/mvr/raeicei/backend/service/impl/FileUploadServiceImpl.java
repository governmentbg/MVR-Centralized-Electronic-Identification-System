package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.raeicei.backend.mapper.DocumentMapper;
import bg.bulsi.mvr.raeicei.backend.service.FileUploadService;
import bg.bulsi.mvr.raeicei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerStatus;
import bg.bulsi.mvr.raeicei.model.entity.Document;
import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.entity.application.AbstractApplication;
import bg.bulsi.mvr.raeicei.model.repository.AbstractApplicationRepository;
import bg.bulsi.mvr.raeicei.model.repository.DocumentRepository;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.EID_ADMINISTRATOR_NOT_FOUND;
import static bg.bulsi.mvr.common.exception.ErrorCode.ENTITY_NOT_FOUND;

@Slf4j
@Service
@RequiredArgsConstructor
public class FileUploadServiceImpl implements FileUploadService {

    private final DocumentRepository documentRepository;
    private final AbstractApplicationRepository applicationRepository;
    private final EidManagerRepository managerRepository;
    private final DocumentMapper mapper;

    @Override
    public Document getById(UUID id) {
        return documentRepository.findById(id).orElseThrow(() -> new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, "Document", id.toString()));
    }

//    @Override
//    public List<Document> getDocumentsForApplication(UUID applicationId) {
//        return repository.findDocumentByApplicationId(applicationId);
//    }

    @Override
    public void delete(UUID id) {
        if (!documentRepository.existsById(id)) {
            throw new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, "Document", id.toString());
        }
        documentRepository.deleteById(id);
    }

    @Override
    public Document createForApplication(DocumentDTO dto, UUID applicationId, Boolean isInternal) {
        Document entity = mapper.mapToEntity(dto);

        AbstractApplication application = applicationRepository.findById(applicationId).orElseThrow(() ->
                new EntityNotFoundException(ENTITY_NOT_FOUND, "Application", applicationId.toString()));

        application.getAttachments().add(entity);

        if (!isInternal) {
            application.setStatus(ApplicationStatus.IN_REVIEW);
        }

        return documentRepository.save(entity);
    }

    @Override
    public Document createForEidManager(DocumentDTO dto, UUID eidManagerId, Boolean isInternal) {
        Document entity = mapper.mapToEntity(dto);

        EidManager eidManager = managerRepository.findById(eidManagerId).orElseThrow(() ->
                new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, eidManagerId.toString(), eidManagerId.toString()));

        if (!isInternal && !EidManagerStatus.PENDING_ATTACHMENTS.equals(eidManager.getManagerStatus())) {
            throw new ValidationMVRException(ErrorCode.EID_MANAGER_PENDING_FOR_ATTACHMENTS_STATUS_REQUIRED);
        }

        eidManager.getAttachments().add(entity);

        if (!isInternal) {
            eidManager.setManagerStatus(EidManagerStatus.IN_REVIEW);
        }

        return documentRepository.save(entity);
    }
}
