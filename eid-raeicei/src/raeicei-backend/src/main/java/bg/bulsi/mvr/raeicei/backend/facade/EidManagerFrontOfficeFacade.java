package bg.bulsi.mvr.raeicei.backend.facade;

import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.OfficesByRegionDTO;

import java.util.List;
import java.util.UUID;

public interface EidManagerFrontOfficeFacade {
    EidManagerFrontOfficeResponseDTO getEidManagerFrontOfficeById(UUID id);

    List<EidManagerFrontOfficeDTO> getAllEidManagerFrontOfficesByEidManagerId(UUID eidManagerId);

    EidManagerFrontOfficeResponseDTO createEidManagerFrontOffice(EidManagerFrontOfficeResponseDTO dto);

    EidManagerFrontOfficeResponseDTO updateEidManagerFrontOffice(EidManagerFrontOfficeDTO dto, UUID eidManagerId);

    EidManagerFrontOfficeDTO getEidManagerFrontOfficeByName(String name);

    List<EidManagerFrontOfficeDTO> getAllEidManagerFrontOfficesByRegion(OfficesByRegionDTO dto);

    void deleteEidManagerFrontOffice(UUID id);
}
