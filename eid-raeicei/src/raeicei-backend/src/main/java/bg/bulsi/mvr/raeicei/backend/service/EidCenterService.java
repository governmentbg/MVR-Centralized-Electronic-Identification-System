package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.EidCenter;
import bg.bulsi.mvr.raeicei.model.repository.view.ReportOfEidManagers;

import java.util.List;
import java.util.UUID;

public interface EidCenterService {

    EidCenterDTO getEidCenterById(UUID id);

    EidCenterAuthorizedDTO getCenterWithAuthorizedPersonsById(UUID id);

    EidCenterFullDTO getFullEidCenterById(UUID id, Boolean onlyActiveElements);

    EidCenter getEidCenterEntityById(UUID id);

    List<EidCenterDTO> getAllEidCenters();

    List<EidCenterHistoryDTO> getAllEidCentersHistory(Boolean stoppedIncluded);

    List<EidCenterDTO> getAllEidCentersByStatus(EidManagerStatus eidManagerStatus);

    EidCenter createEidCenter(EidCenterDTO dto);

    EidCenter updateEidCenter(EidCenterDTO dto);

    EidCenter updateEidCenterFlatData(EidCenterDTO dto);

    List<ReportOfEidManagers> getEidCentersReport();
}
