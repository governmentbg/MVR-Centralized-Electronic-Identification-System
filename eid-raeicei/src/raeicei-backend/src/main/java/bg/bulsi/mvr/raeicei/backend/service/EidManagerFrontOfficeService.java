package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.OfficesByRegionDTO;
import bg.bulsi.mvr.raeicei.model.entity.EidManagerFrontOffice;

import java.util.List;
import java.util.UUID;

public interface EidManagerFrontOfficeService {

    EidManagerFrontOffice getEidManagerFrontOfficeById(UUID id);

    List<EidManagerFrontOffice> getAllEidManagerFrontOfficesByEidManagerId(UUID eidManagerId);

    EidManagerFrontOffice createEidManagerFrontOffice(EidManagerFrontOfficeResponseDTO dto);

    EidManagerFrontOffice updateEidManagerFrontOffice(EidManagerFrontOfficeDTO dto, UUID eidManagerId);

    EidManagerFrontOffice getEidManagerFrontOfficeByName(String name);

    List<EidManagerFrontOfficeDTO> getAllEidManagerFrontOfficesByRegion(OfficesByRegionDTO dto);

    void deleteEidManagerFrontOffice(UUID id);
}
