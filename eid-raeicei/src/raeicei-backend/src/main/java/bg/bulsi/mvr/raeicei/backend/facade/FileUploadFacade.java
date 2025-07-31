package bg.bulsi.mvr.raeicei.backend.facade;


import bg.bulsi.mvr.raeicei.contract.dto.DocumentDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentResponseDTO;

import java.util.UUID;

public interface FileUploadFacade {

    DocumentResponseDTO getById(UUID id);

    DocumentResponseDTO downloadTechnicalCheckProtocol();

//    List<DocumentDTO> getDocumentsForApplication(UUID applicationId);

    void delete(UUID id);

    DocumentDTO createForApplication(DocumentDTO dto, UUID applicationId, Boolean isInternal);

    DocumentDTO createForEidManager(DocumentDTO dto, UUID eidManagerId, Boolean isInternal);
}
