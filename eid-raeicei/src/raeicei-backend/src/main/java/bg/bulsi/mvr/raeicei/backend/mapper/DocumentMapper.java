package bg.bulsi.mvr.raeicei.backend.mapper;

import bg.bulsi.mvr.raeicei.contract.dto.DocumentDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.Document;
import bg.bulsi.mvr.raeicei.model.entity.DocumentType;
import org.mapstruct.*;

import java.time.LocalDateTime;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.List;
import java.util.UUID;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public interface DocumentMapper {

    default OffsetDateTime map(LocalDateTime dateTime) {
        return dateTime.atOffset(ZoneOffset.ofHours(0));
    }

    default LocalDateTime map(OffsetDateTime dateTime) {
        return dateTime.atZoneSameInstant(ZoneOffset.UTC).toLocalDateTime();
    }

    @Mappings({
            @Mapping(target = "documentType", expression = "java(new bg.bulsi.mvr.raeicei.model.entity.DocumentType(dto.getDocumentType()))"),
            @Mapping(target = "filePath", expression = "java(\"\")"),
            @Mapping(target = "outgoing", source = "isOutgoing")
    })
    Document mapToEntity(DocumentDTO dto);

    @Mapping(target = "isOutgoing", source = "outgoing")
    DocumentResponseDTO mapFromEntity(Document entity);

    default public DocumentType mapFromUUID(UUID idString) {
        return new DocumentType(idString);
    }
//    default public UUID mapToUUID(DocumentType entity) {
//        return entity.getId();
//    }

    @Mappings({
            @Mapping(target = "documentType", expression = "java(entity.getId())")
    })
    DocumentDTO mapToDto(Document entity);

//    DocumentResponseDTO mapToResponseDto(Document entity);

    Document mapToEntity(@MappingTarget Document entity, DocumentDTO dto);

    @Named("attachments2Documents")
    List<Document> mapToEntityList(List<DocumentDTO> dtos);

    List<DocumentResponseDTO> mapToDtoList(List<Document> entities);
}
