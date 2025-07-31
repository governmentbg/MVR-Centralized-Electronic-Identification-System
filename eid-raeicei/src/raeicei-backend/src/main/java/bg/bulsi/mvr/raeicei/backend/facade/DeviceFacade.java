package bg.bulsi.mvr.raeicei.backend.facade;


import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceExtRequestDTO;

import java.util.List;
import java.util.UUID;

public interface DeviceFacade {
    DeviceDTO getDeviceById(UUID id);

    List<DeviceDTO> getAllDevices();

    List<DeviceDTO> getDevices4AdministratorId(UUID aeiId);

    DeviceDTO createDevice(DeviceDTO dto, UUID eidAdministratorId);

    DeviceDTO createExtDevice(DeviceExtRequestDTO dto, UUID eidAdministratorId);

    DeviceDTO updateDevice(DeviceDTO dto, UUID eidAdministratorId);

    DeviceDTO updateExtDevice(DeviceExtRequestDTO dto, UUID eidAdministratorId);

    List<DeviceDTO> getDevicesByType(String type);

    void deleteDevice(UUID id, UUID eidAdministratorId);
}
