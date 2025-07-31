package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.EidAdministrator;
import bg.bulsi.mvr.raeicei.model.repository.view.ReportOfEidManagers;

import java.util.List;
import java.util.UUID;

public interface EidAdministratorService {

    EidAdministrator getEidAdministratorById(UUID id);

    EidAdministratorDTO findEidAdministratorById(UUID id);

    EidAdministratorAuthorizedDTO findEidAdministratorWithAuthorizedPersonsById(UUID id);

    EidAdministratorFullDTO findFullEidAdministratorById(UUID id, Boolean onlyActiveElements);

    List<EidAdministratorDTO> getAllEidAdministrators();

    List<EidAdministratorHistoryDTO> getAllEidAdministratorsHistory(Boolean stoppedIncluded);

    List<EidAdministratorDTO> getAllEidAdministratorsByStatus(EidManagerStatus eidManagerStatus);

    EidAdministrator createEidAdministrator(EidAdministratorDTO dto);

    EidAdministrator updateEidAdministrator(EidAdministratorDTO dto);

    EidAdministrator updateEidAdministratorFlatData(EidAdministratorDTO dto);

    List<ReportOfEidManagers> getEidAdministratorsReport();
}
