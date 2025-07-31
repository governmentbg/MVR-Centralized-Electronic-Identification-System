package bg.bulsi.mvr.raeicei.backend.facade.impl;

import bg.bulsi.mvr.raeicei.backend.facade.FileUploadFacade;
import bg.bulsi.mvr.raeicei.backend.mapper.DocumentMapper;
import bg.bulsi.mvr.raeicei.backend.service.FileUploadService;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.Document;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class FileUploadFacadeImpl implements FileUploadFacade {

    private final FileUploadService service;
    private final DocumentMapper mapper;

    @Override
    public DocumentResponseDTO getById(UUID id) {
        Document entity = service.getById(id);
        return mapper.mapFromEntity(entity);
    }

    @Override
    public DocumentResponseDTO downloadTechnicalCheckProtocol() {
        DocumentResponseDTO protocol = new DocumentResponseDTO();
        UUID id = UUID.randomUUID();
        protocol.setId(id);
        protocol.setFileName("Technical check protocol " + id + ".docx");

        try {
            try (var inputStream = getClass().getResourceAsStream("/protocol/technical_check.docx")) {
                if (inputStream == null) {
                    throw new RuntimeException("File not found in resources: protocol/technical_check.docx");
                }

                byte[] content = inputStream.readAllBytes();
                protocol.setContent(content);

            }
        } catch (IOException e) {
            throw new RuntimeException("Failed to read file from resources: protocol/technical_check.docx", e);
        }

        return protocol;
    }

//    @Override
//    public List<DocumentDTO> getDocumentsForApplication(UUID applicationId) {
//        List<Document> entities = service.getDocumentsForApplication(applicationId);
//        return mapper.mapToDtoList(entities);
//    }

    @Override
    public void delete(UUID id) {
        service.delete(id);
    }

    @Override
    public DocumentDTO createForApplication(DocumentDTO dto, UUID applicationId, Boolean isInternal) {
        Document entity = service.createForApplication(dto, applicationId, isInternal);
        return mapper.mapToDto(entity);
    }

    @Override
    public DocumentDTO createForEidManager(DocumentDTO dto, UUID eidManagerId, Boolean isInternal) {
        Document entity = service.createForEidManager(dto, eidManagerId, isInternal);
        return mapper.mapToDto(entity);
    }
}
