package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.raeicei.backend.mapper.DeviceMapper;
import bg.bulsi.mvr.raeicei.backend.service.DeviceService;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceExtRequestDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceType;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerStatus;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import bg.bulsi.mvr.raeicei.model.entity.EidAdministrator;
import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.repository.DeviceRepository;
import bg.bulsi.mvr.raeicei.model.repository.EidAdministratorRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Objects;
import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;

@Slf4j
@Service
@RequiredArgsConstructor
public class DeviceServiceImpl implements DeviceService {

    private final DeviceRepository repository;
    private final EidAdministratorRepository administratorRepository;
    private final DeviceMapper mapper;

    @Value("${mvr-uuid.aei}")
    private UUID MVR_AEI_UUID;

    @Value("${mvr-uuid.cei}")
    private UUID MVR_CEI_UUID;

    @Override
    public Device getDeviceById(UUID id) {
        return repository.findById(id).orElseThrow(() -> new EntityNotFoundException(ENTITY_NOT_FOUND, "Device", id.toString()));
    }

    @Override
    public List<Device> getAllDevices() {
        return repository.findAllByIsActiveTrue();
    }

    @Override
    public List<Device> getDevices4AdministratorId(UUID aeiId) {
        EidAdministrator eidAdministrator = getEidAdministratorById(aeiId);

        return eidAdministrator.getDevices();
    }

    @Override
    public Device createDevice(DeviceDTO dto, UUID eidAdministratorId) {
        validateDto(dto);

        Device entity = mapper.mapToEntity(dto);
        entity.setIsActive(null);

        EidAdministrator eidAdministrator = getEidAdministratorById(eidAdministratorId);

        checkEidManagerCurrentStatus(eidAdministrator);

        eidAdministrator.getDevices().add(entity);

        if (!eidAdministrator.getId().equals(MVR_AEI_UUID) && !eidAdministrator.getId().equals(MVR_CEI_UUID)) {
            eidAdministrator.setManagerStatus(EidManagerStatus.PENDING_ATTACHMENTS);
        }

        return repository.save(entity);
    }

    @Override
    public Device createExtDevice(DeviceExtRequestDTO dto, UUID eidAdministratorId) {
        validateExtDto(dto);

        Device entity = mapper.mapToEntityExt(dto);
        entity.setType(DeviceType.OTHER);
        entity.setIsActive(null);

        EidAdministrator eidAdministrator = getEidAdministratorById(eidAdministratorId);

        checkEidManagerCurrentStatus(eidAdministrator);

        eidAdministrator.getDevices().add(entity);

        if (!eidAdministrator.getId().equals(MVR_AEI_UUID) && !eidAdministrator.getId().equals(MVR_CEI_UUID)) {
            eidAdministrator.setManagerStatus(EidManagerStatus.PENDING_ATTACHMENTS);
        }

        return repository.save(entity);
    }

    @Override
    public Device updateDevice(DeviceDTO dto, UUID eidAdministratorId) {
        validateDto(dto);

        Device entity = getDeviceById(dto.getId());
        entity = mapper.mapToEntity(entity, dto);
        entity.setIsActive(null);

        EidAdministrator eidAdministrator = getEidAdministratorById(eidAdministratorId);

        checkEidManagerCurrentStatus(eidAdministrator);

        if (!eidAdministrator.getId().equals(MVR_AEI_UUID) && !eidAdministrator.getId().equals(MVR_CEI_UUID)) {
            eidAdministrator.setManagerStatus(EidManagerStatus.PENDING_ATTACHMENTS);
        }

        return repository.save(entity);
    }

    @Override
    public Device updateExtDevice(DeviceExtRequestDTO dto, UUID eidAdministratorId) {
        validateExtDto(dto);

        Device entity = getDeviceById(dto.getId());
        entity = mapper.mapToEntityExt(entity, dto);
        entity.setType(DeviceType.OTHER);
        entity.setIsActive(null);

        EidAdministrator eidAdministrator = getEidAdministratorById(eidAdministratorId);

        checkEidManagerCurrentStatus(eidAdministrator);

        if (!eidAdministrator.getId().equals(MVR_AEI_UUID) && !eidAdministrator.getId().equals(MVR_CEI_UUID)) {
            eidAdministrator.setManagerStatus(EidManagerStatus.PENDING_ATTACHMENTS);
        }

        return repository.save(entity);
    }

    @Override
    public List<Device> getDevicesByType(String type) {
        return repository.findAllByType(DeviceType.valueOf(type));
    }

    @Override
    public void deleteDevice(UUID id, UUID eidAdministratorId) {
        Device device = getDeviceById(id);
        device.setIsActive(false);

        EidAdministrator eidAdministrator = getEidAdministratorById(eidAdministratorId);

        checkEidManagerCurrentStatus(eidAdministrator);

        eidAdministrator.getDevices().remove(device);
    }

    private void validateDto(DeviceDTO dto) {
        if ((dto.getAuthorizationLink() == null || dto.getAuthorizationLink().isBlank()) && (dto.getBackchannelAuthorizationLink() == null || dto.getBackchannelAuthorizationLink().isBlank())) {
            throw new ValidationMVRException(LINK_REQUIRED);
        }
    }

    private void validateExtDto(DeviceExtRequestDTO dto) {
        if ((dto.getAuthorizationLink() == null || dto.getAuthorizationLink().isBlank()) && (dto.getBackchannelAuthorizationLink() == null || dto.getBackchannelAuthorizationLink().isBlank())) {
            throw new ValidationMVRException(LINK_REQUIRED);
        }
    }

    private EidAdministrator getEidAdministratorById(UUID eidAdministratorId) {
        EidAdministrator eidAdministrator = administratorRepository.findById(eidAdministratorId).orElseThrow(() ->
                new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, eidAdministratorId.toString(), eidAdministratorId.toString()));
        return eidAdministrator;
    }

    private static void checkEidManagerCurrentStatus(EidManager eidManager) {
        if (EidManagerStatus.STOP.equals(eidManager.getManagerStatus()) || EidManagerStatus.SUSPENDED.equals(eidManager.getManagerStatus()) || EidManagerStatus.IN_REVIEW.equals(eidManager.getManagerStatus())) {
            throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
        }
    }
}
