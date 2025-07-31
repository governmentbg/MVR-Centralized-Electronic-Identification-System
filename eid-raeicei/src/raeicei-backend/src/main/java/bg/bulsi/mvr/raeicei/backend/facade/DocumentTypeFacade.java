package bg.bulsi.mvr.raeicei.backend.facade;


import bg.bulsi.mvr.raeicei.contract.dto.DocumentTypeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentTypeResponseDTO;

import java.util.List;
import java.util.UUID;

public interface DocumentTypeFacade {

    DocumentTypeResponseDTO create(DocumentTypeDTO dto);

    DocumentTypeResponseDTO update(DocumentTypeResponseDTO dto);

    void delete(UUID id);

    DocumentTypeResponseDTO getByID(UUID id);

    List<DocumentTypeResponseDTO> getAllDocumentTypes();
}
