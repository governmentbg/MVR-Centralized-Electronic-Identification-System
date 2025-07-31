package bg.bulsi.mvr.raeicei.backend.facade.impl;

import bg.bulsi.mvr.raeicei.backend.facade.EidCenterFacade;
import bg.bulsi.mvr.raeicei.backend.mapper.EidManagerMapper;
import bg.bulsi.mvr.raeicei.backend.service.EidCenterService;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.EidCenter;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class EidCenterFacadeImpl implements EidCenterFacade {

    private final EidManagerMapper eidManagerMapper;
    private final EidCenterService eidCenterService;

    @Override
//    @Cacheable(value = EID_CENTER_CACHE, key = "#id")
    public EidCenterDTO findEidCenterById(UUID id) {
        return this.eidCenterService.getEidCenterById(id);
    }

    @Override
//    @Cacheable(value = EID_CENTER_CACHE, key = "#id")
    public EidCenterAuthorizedDTO findEidCenterWithAuthorizedPersonsById(UUID id) {
        return this.eidCenterService.getCenterWithAuthorizedPersonsById(id);
    }

    @Override
//    @Cacheable(value = EID_CENTER_CACHE, key = "#id")
    public EidCenterFullDTO findFullEidCenterById(UUID id, Boolean onlyActiveElements) {
        return this.eidCenterService.getFullEidCenterById(id, onlyActiveElements);
    }

    @Override
    public List<EidCenterDTO> getAllEidCenter() {
        return this.eidCenterService.getAllEidCenters();
    }

    @Override
    public List<EidCenterHistoryDTO> getAllEidCenterHistory(Boolean stoppedIncluded) {
        return this.eidCenterService.getAllEidCentersHistory(stoppedIncluded);
    }

    @Override
//    @Cacheable(value = EID_CENTER_LIST_CACHE)
    public List<EidCenterDTO> getAllEidCentersByStatus(EidManagerStatus eidManagerStatus) {
        return this.eidCenterService.getAllEidCentersByStatus(eidManagerStatus);
    }

    @Override
//    @CacheEvict(cacheNames = {EID_CENTER_LIST_CACHE, EID_CENTER_CACHE}, allEntries = true)
    public EidCenterDTO createEidCenter(EidCenterDTO dto) {
        EidCenter entity = eidCenterService.createEidCenter(dto);
        return eidManagerMapper.mapToEidCenterDto(entity);
    }

    @Override
//    @CacheEvict(cacheNames = {EID_CENTER_LIST_CACHE, EID_CENTER_CACHE}, allEntries = true)
    public EidCenterDTO updateEidCenter(EidCenterDTO dto) {
        EidCenter entity = eidCenterService.updateEidCenter(dto);
        return eidManagerMapper.mapToEidCenterDto(entity);
    }
}
