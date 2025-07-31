package bg.bulsi.mvr.raeicei.backend.facade;


import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorApplicationDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorApplicationResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorApplicationShortDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidApplicationFilter;
import bg.bulsi.mvr.raeicei.model.entity.application.EidAdministratorApplication;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import java.util.UUID;

public interface EidAdministratorApplicationFacade {

    EidAdministratorApplication create(EidAdministratorApplicationDTO dto);

    EidAdministratorApplicationResponseDTO getByID(UUID id);

    Page<EidAdministratorApplicationShortDTO> getByFilter(EidApplicationFilter filter, Pageable pageable);
}
