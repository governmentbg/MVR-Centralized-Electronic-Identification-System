package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.DocumentTypeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentTypeResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.DocumentType;

import java.util.List;
import java.util.UUID;

public interface DocumentTypeService {

    DocumentType create(DocumentTypeDTO dto);

    DocumentType update(DocumentTypeResponseDTO dto);

    void delete(UUID id);

    DocumentType getByID(UUID id);

    List<DocumentType> getAllDocumentTypes();
}
