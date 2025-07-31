package bg.bulsi.mvr.raeicei.backend.mapper;

import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.EidManagerFrontOffice;
import org.mapstruct.*;

import java.util.List;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public abstract class EidManagerFrontOfficeMapper {

    public abstract EidManagerFrontOffice mapToEntity(EidManagerFrontOfficeDTO dto);

    @Mappings({
            @Mapping(source = "eidManagerId", target = "eidManager.id")
    })
    public abstract EidManagerFrontOffice mapToEntity(EidManagerFrontOfficeResponseDTO dto);

    //
    public abstract EidManagerFrontOfficeDTO mapToDto(EidManagerFrontOffice entity);

    @Mapping(source = "eidManager.id", target = "eidManagerId")
    public abstract EidManagerFrontOfficeResponseDTO mapToResponseDto(EidManagerFrontOffice entity);

    @Mappings({
            @Mapping(target = "eidManager", ignore = true),
            @Mapping(target = "workingHours", expression = "java(dto.getWorkingHours() == null ? null : new java.util.ArrayList<>(dto.getWorkingHours()))")
    })
    public abstract EidManagerFrontOffice mapToEntity(@MappingTarget EidManagerFrontOffice entity, EidManagerFrontOfficeDTO dto);

    public abstract List<EidManagerFrontOffice> mapToEntityList(List<EidManagerFrontOfficeDTO> dtos);

    public abstract List<EidManagerFrontOfficeDTO> mapToDtoList(List<EidManagerFrontOffice> entities);

}
