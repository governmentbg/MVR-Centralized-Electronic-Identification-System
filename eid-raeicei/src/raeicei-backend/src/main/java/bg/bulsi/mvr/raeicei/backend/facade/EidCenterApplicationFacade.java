package bg.bulsi.mvr.raeicei.backend.facade;


import bg.bulsi.mvr.raeicei.contract.dto.EidApplicationFilter;
import bg.bulsi.mvr.raeicei.contract.dto.EidCenterApplicationDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidCenterApplicationResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidCenterApplicationShortDTO;
import bg.bulsi.mvr.raeicei.model.entity.application.EidCenterApplication;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import java.util.UUID;

public interface EidCenterApplicationFacade {

    EidCenterApplication create(EidCenterApplicationDTO dto);

    EidCenterApplicationResponseDTO getByID(UUID id);

    Page<EidCenterApplicationShortDTO> getByFilter(EidApplicationFilter filter, Pageable pageable);
}
