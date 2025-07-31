package bg.bulsi.mvr.raeicei.backend.mapper;

import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceExtRequestDTO;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import org.mapstruct.Mapper;
import org.mapstruct.MappingTarget;
import org.mapstruct.NullValueCheckStrategy;

import java.util.List;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public interface DeviceMapper {

    Device mapToEntity(DeviceDTO dto);
    Device mapToEntityExt(DeviceExtRequestDTO dto);
    DeviceDTO mapToDto(Device entity);

    Device mapToEntity(@MappingTarget Device entity , DeviceDTO dto);
    Device mapToEntityExt(@MappingTarget Device entity , DeviceExtRequestDTO dto);

    List<Device> mapToEntityList(List<DeviceDTO> dtos);
    List<DeviceDTO> mapToDtoList(List<Device> entities);
}
