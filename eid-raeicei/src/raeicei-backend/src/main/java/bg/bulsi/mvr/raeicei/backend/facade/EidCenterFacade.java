package bg.bulsi.mvr.raeicei.backend.facade;

import bg.bulsi.mvr.raeicei.contract.dto.*;

import java.util.List;
import java.util.UUID;

public interface EidCenterFacade {
    EidCenterDTO findEidCenterById(UUID id);

    EidCenterAuthorizedDTO findEidCenterWithAuthorizedPersonsById(UUID id);

    EidCenterFullDTO findFullEidCenterById(UUID id, Boolean onlyActiveElements);

    List<EidCenterDTO> getAllEidCenter();

    List<EidCenterHistoryDTO> getAllEidCenterHistory(Boolean stoppedIncluded);

    List<EidCenterDTO> getAllEidCentersByStatus(EidManagerStatus eidManagerStatus);

    EidCenterDTO createEidCenter(EidCenterDTO dto);

    EidCenterDTO updateEidCenter(EidCenterDTO dto);
}
