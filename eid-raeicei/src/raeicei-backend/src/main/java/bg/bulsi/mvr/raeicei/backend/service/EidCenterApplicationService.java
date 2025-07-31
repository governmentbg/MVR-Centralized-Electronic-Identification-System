package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.EidCenterApplicationDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidCenterApplicationShortDTO;
import bg.bulsi.mvr.raeicei.model.entity.application.EidCenterApplication;
import bg.bulsi.mvr.raeicei.contract.dto.EidApplicationFilter;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import java.util.UUID;

public interface EidCenterApplicationService {

    EidCenterApplication create(EidCenterApplicationDTO dto);

    EidCenterApplication getById(UUID id);

    Page<EidCenterApplicationShortDTO> getByFilter(EidApplicationFilter filter, Pageable pageable);
}
