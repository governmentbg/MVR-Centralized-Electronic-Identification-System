package bg.bulsi.mvr.raeicei.backend.facade.impl;

import bg.bulsi.mvr.raeicei.backend.facade.DocumentTypeFacade;
import bg.bulsi.mvr.raeicei.backend.mapper.DocumentTypeMapper;
import bg.bulsi.mvr.raeicei.backend.service.DocumentTypeService;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentTypeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentTypeResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.DocumentType;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class DocumentTypeFacadeImpl implements DocumentTypeFacade {

    private final DocumentTypeService service;
    private final DocumentTypeMapper mapper;

    @Override
    public DocumentTypeResponseDTO create(DocumentTypeDTO dto) {
        DocumentType entity = service.create(dto);
        return mapper.mapToResponseDto(entity);
    }

    @Override
    public DocumentTypeResponseDTO update(DocumentTypeResponseDTO dto) {
        DocumentType entity = service.update(dto);
        return mapper.mapToResponseDto(entity);
    }

    @Override
    public void delete(UUID id) {
        service.delete(id);
    }

    @Override
    public DocumentTypeResponseDTO getByID(UUID id) {
        DocumentType entity = service.getByID(id);
        return mapper.mapToResponseDto(entity);
    }

    @Override
    public List<DocumentTypeResponseDTO> getAllDocumentTypes() {
        List<DocumentType> entities = service.getAllDocumentTypes();
        return mapper.mapResultList(entities);
    }
}
