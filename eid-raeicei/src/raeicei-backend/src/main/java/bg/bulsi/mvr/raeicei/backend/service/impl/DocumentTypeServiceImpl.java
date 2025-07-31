package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.raeicei.backend.mapper.DocumentTypeMapper;
import bg.bulsi.mvr.raeicei.backend.service.DocumentTypeService;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentTypeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentTypeResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.DocumentType;
import bg.bulsi.mvr.raeicei.model.repository.DocumentTypeRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class DocumentTypeServiceImpl implements DocumentTypeService {

    private final DocumentTypeRepository repository;
    private final DocumentTypeMapper mapper;


    @Override
    public DocumentType create(DocumentTypeDTO dto) {
        DocumentType entity = mapper.mapToEntity(dto);
        entity.setActive(true);
        return repository.save(entity);
    }

    @Override
    public DocumentType update(DocumentTypeResponseDTO dto) {
        DocumentType entity = getByID(dto.getId());
        entity = mapper.mapToEntity(entity, dto);
        entity.setActive(true);
        return repository.save(entity);
    }

    @Override
    public void delete(UUID id) {
        DocumentType entity = getByID(id);
        entity.setActive(false);
    }

    @Override
    public DocumentType getByID(UUID id) {
        return repository.findById(id).orElseThrow(() -> new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, "DocumentType", id.toString()));
    }

    @Override
    public List<DocumentType> getAllDocumentTypes() {
        return repository.findAllByActiveTrue();
    }
}
