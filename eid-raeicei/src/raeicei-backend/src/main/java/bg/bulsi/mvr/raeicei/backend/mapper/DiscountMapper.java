package bg.bulsi.mvr.raeicei.backend.mapper;

import bg.bulsi.mvr.raeicei.contract.dto.DiscountDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DiscountDoubleCurrencyResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DiscountResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.Discount;
import org.mapstruct.*;

import java.util.List;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public interface DiscountMapper {


    @Mappings({
            @Mapping(target = "eidManager", ignore = true),
            @Mapping(target = "value", source = "discount"),
    })
    Discount mapToEntity(DiscountResponseDTO dto);

    @Mappings({
        @Mapping(target = "discount", source = "value")
    })
    DiscountDTO mapToDto(Discount entity);

    @Mappings({
            @Mapping(target = "eidManagerId", source = "eidManager.id"),
            @Mapping(target = "discount", source = "value"),
            @Mapping(target = "providedServiceId", source = "providedService.id"),
    })
    DiscountResponseDTO mapToResponseDto(Discount entity);

    @Mappings({
            @Mapping(target = "eidManagerId", source = "eidManager.id"),
            @Mapping(target = "providedServiceId", source = "providedService.id"),
    })
    DiscountDoubleCurrencyResponseDTO mapToDiscountDoubleCurrencyResponseDto(Discount entity);

    @Mappings({
            @Mapping(target = "id", ignore = true),
            @Mapping(target = "value", source = "discount"),
    })
//    @Mapping(target = "id", ignore = true)
    Discount mapToEntity(@MappingTarget Discount entity, DiscountDTO dto);

    @Mapping(target = "eidManager", ignore = true)
    List<Discount> mapToEntityList(List<DiscountDTO> dtos);

    @Mapping(target = "eidManagerId", ignore = true)
    List<DiscountDTO> mapToDtoList(List<Discount> entities);

    List<DiscountResponseDTO> mapToResponseDtoList(List<Discount> entities);
}
