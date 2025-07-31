package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.DocumentDTO;
import bg.bulsi.mvr.raeicei.model.entity.Document;

import java.util.UUID;

public interface FileUploadService {

    Document getById(UUID id);

//    List<Document> getDocumentsForApplication(UUID applicationId);

    void delete(UUID id);

    Document createForApplication(DocumentDTO dto, UUID applicationId, Boolean isInternal);

    Document createForEidManager(DocumentDTO dto, UUID eidManagerId, Boolean isInternal);
}
