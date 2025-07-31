package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorApplicationDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorApplicationShortDTO;
import bg.bulsi.mvr.raeicei.model.entity.application.EidAdministratorApplication;
import bg.bulsi.mvr.raeicei.contract.dto.EidApplicationFilter;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import java.util.UUID;

public interface EidAdministratorApplicationService {

    EidAdministratorApplication create(EidAdministratorApplicationDTO dto);

    EidAdministratorApplication getById(UUID id);

    Page<EidAdministratorApplicationShortDTO> getByFilter(EidApplicationFilter filter, Pageable pageable);
}
