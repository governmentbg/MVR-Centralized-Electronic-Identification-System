package bg.bulsi.mvr.raeicei.backend.facade.impl;

import bg.bulsi.mvr.raeicei.backend.config.RedisConfig;
import bg.bulsi.mvr.raeicei.backend.facade.EidManagerFrontOfficeFacade;
import bg.bulsi.mvr.raeicei.backend.mapper.EidManagerFrontOfficeMapper;
import bg.bulsi.mvr.raeicei.backend.service.EidManagerFrontOfficeService;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.OfficesByRegionDTO;
import bg.bulsi.mvr.raeicei.model.entity.EidManagerFrontOffice;
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
public class EidManagerFrontOfficeFacadeImpl implements EidManagerFrontOfficeFacade {
    private final EidManagerFrontOfficeService eidManagerFrontOfficeService;
    private final EidManagerFrontOfficeMapper eidManagerFrontOfficeMapper;

    @Override
    @Cacheable(value = RedisConfig.EID_ADMINISTRATOR_OFFICE_CACHE, key = "#id")
    public EidManagerFrontOfficeResponseDTO getEidManagerFrontOfficeById(UUID id) {
        EidManagerFrontOffice entity = eidManagerFrontOfficeService.getEidManagerFrontOfficeById(id);
        return eidManagerFrontOfficeMapper.mapToResponseDto(entity);
    }

    @Override
    @Cacheable(value = RedisConfig.EID_ADMINISTRATOR_OFFICE_LIST_CACHE, key = "#eidManagerId")
    public List<EidManagerFrontOfficeDTO> getAllEidManagerFrontOfficesByEidManagerId(UUID eidManagerId) {
        List<EidManagerFrontOffice> entities = eidManagerFrontOfficeService.getAllEidManagerFrontOfficesByEidManagerId(eidManagerId);
        return eidManagerFrontOfficeMapper.mapToDtoList(entities);
    }

    @Override
    @CacheEvict(cacheNames = {RedisConfig.EID_ADMINISTRATOR_OFFICE_CACHE, RedisConfig.EID_ADMINISTRATOR_OFFICE_LIST_CACHE}, allEntries = true)
    public EidManagerFrontOfficeResponseDTO createEidManagerFrontOffice(EidManagerFrontOfficeResponseDTO dto) {
        EidManagerFrontOffice entity = eidManagerFrontOfficeService.createEidManagerFrontOffice(dto);
        return eidManagerFrontOfficeMapper.mapToResponseDto(entity);
    }

    @Override
    @CacheEvict(cacheNames = {RedisConfig.EID_ADMINISTRATOR_OFFICE_CACHE, RedisConfig.EID_ADMINISTRATOR_OFFICE_LIST_CACHE}, allEntries = true)
    public EidManagerFrontOfficeResponseDTO updateEidManagerFrontOffice(EidManagerFrontOfficeDTO dto, UUID eidManagerId) {
        EidManagerFrontOffice entity = eidManagerFrontOfficeService.updateEidManagerFrontOffice(dto, eidManagerId);
        return eidManagerFrontOfficeMapper.mapToResponseDto(entity);
    }

    @Override
    @Cacheable(value = RedisConfig.EID_ADMINISTRATOR_OFFICE_CACHE, key = "#name")
    public EidManagerFrontOfficeDTO getEidManagerFrontOfficeByName(String name) {
        EidManagerFrontOffice entity = eidManagerFrontOfficeService.getEidManagerFrontOfficeByName(name);
        return eidManagerFrontOfficeMapper.mapToDto(entity);
    }

    @Override
    public List<EidManagerFrontOfficeDTO> getAllEidManagerFrontOfficesByRegion(OfficesByRegionDTO dto) {
        return eidManagerFrontOfficeService.getAllEidManagerFrontOfficesByRegion(dto);
    }

    @Override
    public void deleteEidManagerFrontOffice(UUID id) {
        eidManagerFrontOfficeService.deleteEidManagerFrontOffice(id);
    }
}
