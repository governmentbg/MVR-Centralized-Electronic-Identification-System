package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceExtRequestDTO;
import bg.bulsi.mvr.raeicei.model.entity.Device;

import java.util.List;
import java.util.UUID;

public interface DeviceService {

    Device getDeviceById(UUID id);

    List<Device> getAllDevices();

    List<Device> getDevices4AdministratorId(UUID aeiId);

    Device createDevice(DeviceDTO dto, UUID eidAdministratorId);

    Device createExtDevice(DeviceExtRequestDTO dto, UUID eidAdministratorId);

    Device updateDevice(DeviceDTO dto, UUID eidAdministratorId);

    Device updateExtDevice(DeviceExtRequestDTO dto, UUID eidAdministratorId);

    List<Device> getDevicesByType(String type);

    void deleteDevice(UUID id, UUID eidAdministratorId);
}
