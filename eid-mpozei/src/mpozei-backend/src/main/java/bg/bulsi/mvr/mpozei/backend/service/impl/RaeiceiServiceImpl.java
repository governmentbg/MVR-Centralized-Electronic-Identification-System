package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.mpozei.backend.client.RaeiceiClient;
import bg.bulsi.mvr.mpozei.backend.service.RaeiceiService;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeDTO;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class RaeiceiServiceImpl implements RaeiceiService {
    private final RaeiceiClient raeiceiClient;

    @Override
    @Cacheable(value = "eidAdministrator", key = "#id")
    public EidAdministratorDTO getEidAdministratorById(UUID id) {
        return raeiceiClient.getEidAdministratorById(id);
    }

    @Override
    @Cacheable(value = "eidManagerFrontOffice", key = "#id")
    public EidManagerFrontOfficeDTO getOfficeById(UUID id) {
        return raeiceiClient.getOfficeById(id);
    }

    @Override
    @Cacheable(value = "eidManagerFrontOffice", key = "#name")
    public EidManagerFrontOfficeDTO getOfficeByName(String name) {
        return raeiceiClient.getOfficeByName(name);
    }

    @Override
    @Cacheable(value = "device", key = "#id")
    public DeviceDTO getDeviceById(UUID id) {
        return raeiceiClient.getDeviceById(id);
    }

    @Override
    @Cacheable(value = "device", key = "#type")
    public List<DeviceDTO> getDeviceByType(String type) {
        return raeiceiClient.getDeviceByType(type);
    }
}
