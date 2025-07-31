//package bg.bulsi.mvr.raeicei.backend.mapper;
//
//import bg.bulsi.mvr.raeicei.contract.dto.DeviceTariffDTO;
//import bg.bulsi.mvr.raeicei.model.entity.DeviceTariff;
//import org.mapstruct.*;
//
//import java.util.List;
//
//@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
//public interface DeviceTariffMapper {
//
//    @Mapping(target = "device", ignore = true)
//    @Mapping(target = "eidAdministrator", ignore = true)
//    DeviceTariff mapToEntity(DeviceTariffDTO dto);
//
//    @Mapping(source = "id", target = "deviceId")
//    @Mapping(target = "eidAdministratorId", ignore = true)
//    DeviceTariffDTO mapToDto(DeviceTariff entity);
//
//    @Mapping(target = "eidAdministrator", ignore = true)
//    @Mapping(target = "device", ignore = true)
//    DeviceTariff mapToEntity(@MappingTarget DeviceTariff entity, DeviceTariffDTO dto);
//
//    @Mapping(target = "deviceId", source = "id")
//    @Mapping(target = "eidAdministrator", ignore = true)
//    List<DeviceTariff> mapToEntityList(List<DeviceTariffDTO> dtos);
//
//    @Mapping(source = "id", target = "deviceId")
//    @Mapping(target = "eidAdministratorId", ignore = true)
//    List<DeviceTariffDTO> mapToDtoList(List<DeviceTariff> entities);
//}
