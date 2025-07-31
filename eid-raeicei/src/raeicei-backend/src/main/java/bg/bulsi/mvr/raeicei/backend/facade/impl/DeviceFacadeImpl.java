package bg.bulsi.mvr.raeicei.backend.facade.impl;

import bg.bulsi.mvr.raeicei.backend.config.RedisConfig;
import bg.bulsi.mvr.raeicei.backend.facade.DeviceFacade;
import bg.bulsi.mvr.raeicei.backend.mapper.DeviceMapper;
import bg.bulsi.mvr.raeicei.backend.service.DeviceService;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceExtRequestDTO;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.cache.annotation.CacheEvict;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class DeviceFacadeImpl implements DeviceFacade {
    private final DeviceMapper deviceMapper;
    private final DeviceService deviceService;

    @Override
    @Cacheable(value = RedisConfig.DEVICE_CACHE, key = "#id")
    public DeviceDTO getDeviceById(UUID id) {
        Device entity = deviceService.getDeviceById(id);
        return deviceMapper.mapToDto(entity);
    }

    @Override
    @Cacheable(value = RedisConfig.DEVICE_LIST_CACHE)
    public List<DeviceDTO> getAllDevices() {
        List<Device> entities = deviceService.getAllDevices();
        return deviceMapper.mapToDtoList(entities);
    }

    @Override
    @CacheEvict(cacheNames = {RedisConfig.DEVICE_CACHE, RedisConfig.DEVICE_LIST_CACHE}, allEntries = true)
    public DeviceDTO createDevice(DeviceDTO dto, UUID eidAdministratorId) {
        Device entity = deviceService.createDevice(dto, eidAdministratorId);
        return deviceMapper.mapToDto(entity);
    }

    @Override
    @CacheEvict(cacheNames = {RedisConfig.DEVICE_CACHE, RedisConfig.DEVICE_LIST_CACHE}, allEntries = true)
    public DeviceDTO createExtDevice(DeviceExtRequestDTO dto, UUID eidAdministratorId) {
        Device entity = deviceService.createExtDevice(dto, eidAdministratorId);
        return deviceMapper.mapToDto(entity);
    }

    @Override
    @CacheEvict(cacheNames = {RedisConfig.DEVICE_CACHE, RedisConfig.DEVICE_LIST_CACHE}, allEntries = true)
    public DeviceDTO updateDevice(DeviceDTO dto, UUID eidAdministratorId) {
        Device entity = deviceService.updateDevice(dto, eidAdministratorId);
        return deviceMapper.mapToDto(entity);
    }

    @Override
    @CacheEvict(cacheNames = {RedisConfig.DEVICE_CACHE, RedisConfig.DEVICE_LIST_CACHE}, allEntries = true)
    public DeviceDTO updateExtDevice(DeviceExtRequestDTO dto, UUID eidAdministratorId) {
        Device entity = deviceService.updateExtDevice(dto, eidAdministratorId);
        return deviceMapper.mapToDto(entity);
    }

    @Override
    @Cacheable(value = RedisConfig.DEVICE_CACHE, key = "#type")
    public List<DeviceDTO> getDevicesByType(String type) {
        List<Device> entities = deviceService.getDevicesByType(type);
        return deviceMapper.mapToDtoList(entities);
    }

    @Override
    public List<DeviceDTO> getDevices4AdministratorId(UUID aeiId) {
        List<Device> entities = deviceService.getDevices4AdministratorId(aeiId);
        return deviceMapper.mapToDtoList(entities);
    }

    @Override
    public void deleteDevice(UUID id, UUID eidAdministratorId) {
        deviceService.deleteDevice(id, eidAdministratorId);
    }
}
