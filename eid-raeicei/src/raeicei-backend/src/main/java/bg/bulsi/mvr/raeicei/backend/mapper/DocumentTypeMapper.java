package bg.bulsi.mvr.raeicei.backend.mapper;

import bg.bulsi.mvr.raeicei.contract.dto.DocumentTypeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentTypeResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.DocumentType;
import org.mapstruct.Mapper;
import org.mapstruct.MappingTarget;
import org.mapstruct.NullValueCheckStrategy;

import java.util.List;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public interface DocumentTypeMapper {

    DocumentType mapToEntity(DocumentTypeDTO dto);

    DocumentTypeDTO mapToDto(DocumentType entity);

    DocumentType mapToEntity(DocumentTypeResponseDTO dto);

    DocumentType mapToEntity(@MappingTarget DocumentType entity, DocumentTypeResponseDTO dto);

    DocumentTypeResponseDTO mapToResponseDto(DocumentType entity);

    List<DocumentTypeResponseDTO> mapResultList(List<DocumentType> list);
}
