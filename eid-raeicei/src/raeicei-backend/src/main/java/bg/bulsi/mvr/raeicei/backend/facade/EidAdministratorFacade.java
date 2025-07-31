package bg.bulsi.mvr.raeicei.backend.facade;


import bg.bulsi.mvr.raeicei.contract.dto.*;

import java.util.List;
import java.util.UUID;

public interface EidAdministratorFacade {
    EidAdministratorDTO findEidAdministratorById(UUID id);

    EidAdministratorAuthorizedDTO findEidAdministratorWithAuthorizedPersonsById(UUID id);

    EidAdministratorFullDTO findFullEidAdministratorById(UUID id, Boolean onlyActiveElements);

    List<EidAdministratorDTO> getAllEidAdministrators();

    List<EidAdministratorHistoryDTO> getAllEidAdministratorsHistory(Boolean stoppedIncluded);

    List<EidAdministratorDTO> getAllEidAdministratorsByStatus(EidManagerStatus eidManagerStatus);

    EidAdministratorDTO createEidAdministrator(EidAdministratorDTO dto);

    EidAdministratorDTO updateEidAdministrator(EidAdministratorDTO dto);
}
