package bg.bulsi.mvr.raeicei.backend.mapper;

import bg.bulsi.mvr.raeicei.contract.dto.RegionDTO;
import bg.bulsi.mvr.raeicei.model.enums.NomLanguage;
import bg.bulsi.mvr.raeicei.model.enums.Region;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.Mappings;
import org.mapstruct.NullValueCheckStrategy;

import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public abstract class RegionMapper {

    @Mappings({
            @Mapping(target = "descriptions", source = "desciptions"),
            @Mapping(target = "code", expression = "java(entity.name())")
    })
    public abstract RegionDTO mapToDto(Region entity);

    public abstract List<RegionDTO> mapResultList(List<Region> list);


    public Map<String, String> mapDescription(Map<NomLanguage, String> modelMap) {
        return modelMap.entrySet().stream().collect(Collectors.toMap(e -> e.getKey().getValue(), Map.Entry::getValue));
    }
}
