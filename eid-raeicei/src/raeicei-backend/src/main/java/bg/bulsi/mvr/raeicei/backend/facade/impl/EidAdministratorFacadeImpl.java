package bg.bulsi.mvr.raeicei.backend.facade.impl;

import bg.bulsi.mvr.raeicei.backend.facade.EidAdministratorFacade;
import bg.bulsi.mvr.raeicei.backend.mapper.EidManagerMapper;
import bg.bulsi.mvr.raeicei.backend.service.EidAdministratorService;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.EidAdministrator;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class EidAdministratorFacadeImpl implements EidAdministratorFacade {
    private final EidManagerMapper eidAdministratorMapper;
    private final EidAdministratorService eidAdministratorService;

    @Override
//    @Cacheable(value = EID_ADMINISTRATOR_CACHE, key = "#id")
    public EidAdministratorDTO findEidAdministratorById(UUID id) {
        return eidAdministratorService.findEidAdministratorById(id);
    }

    @Override
//    @Cacheable(value = EID_ADMINISTRATOR_CACHE, key = "#id")
    public EidAdministratorAuthorizedDTO findEidAdministratorWithAuthorizedPersonsById(UUID id) {
        return eidAdministratorService.findEidAdministratorWithAuthorizedPersonsById(id);
    }

    @Override
//    @Cacheable(value = EID_ADMINISTRATOR_CACHE, key = "#id")
    public EidAdministratorFullDTO findFullEidAdministratorById(UUID id, Boolean onlyActiveElements) {
        return eidAdministratorService.findFullEidAdministratorById(id, onlyActiveElements);
    }

    @Override
    public List<EidAdministratorDTO> getAllEidAdministrators() {
        return eidAdministratorService.getAllEidAdministrators();
    }

    @Override
    public List<EidAdministratorHistoryDTO> getAllEidAdministratorsHistory(Boolean stoppedIncluded) {
        return eidAdministratorService.getAllEidAdministratorsHistory(stoppedIncluded);
    }

    @Override
//    @Cacheable(value = EID_ADMINISTRATOR_LIST_CACHE)
    public List<EidAdministratorDTO> getAllEidAdministratorsByStatus(EidManagerStatus eidManagerStatus) {
        return eidAdministratorService.getAllEidAdministratorsByStatus(eidManagerStatus);
    }

    @Override
//    @CacheEvict(cacheNames = {EID_ADMINISTRATOR_LIST_CACHE, EID_ADMINISTRATOR_CACHE}, allEntries = true)
    public EidAdministratorDTO createEidAdministrator(EidAdministratorDTO dto) {
        EidAdministrator entity = eidAdministratorService.createEidAdministrator(dto);
        return eidAdministratorMapper.mapToAdministratorDto(entity);
    }

    @Override
//    @CacheEvict(cacheNames = {EID_ADMINISTRATOR_LIST_CACHE, EID_ADMINISTRATOR_CACHE}, allEntries = true)
    public EidAdministratorDTO updateEidAdministrator(EidAdministratorDTO dto) {
        EidAdministrator entity = eidAdministratorService.updateEidAdministrator(dto);
        return eidAdministratorMapper.mapToAdministratorDto(entity);
    }
}
