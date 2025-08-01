package bg.bulsi.mvr.mpozei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeDTO;

import java.util.List;
import java.util.UUID;

public interface RaeiceiService {
    EidAdministratorDTO getEidAdministratorById(UUID id);

    EidManagerFrontOfficeDTO getOfficeById(UUID id);

    EidManagerFrontOfficeDTO getOfficeByName(String name);

    DeviceDTO getDeviceById(UUID id);

    List<DeviceDTO> getDeviceByType(String name);
}
