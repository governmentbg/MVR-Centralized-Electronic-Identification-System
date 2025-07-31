package bg.bulsi.mvr.raeicei.backend.facade.impl;

import bg.bulsi.mvr.raeicei.backend.facade.EidAdministratorApplicationFacade;
import bg.bulsi.mvr.raeicei.backend.mapper.EidAdministratorApplicationMapper;
import bg.bulsi.mvr.raeicei.backend.service.EidAdministratorApplicationService;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorApplicationDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorApplicationResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorApplicationShortDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidApplicationFilter;
import bg.bulsi.mvr.raeicei.model.entity.application.EidAdministratorApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class EidAdministratorApplicationFacadeImpl implements EidAdministratorApplicationFacade {

    private final EidAdministratorApplicationService service;
    private final EidAdministratorApplicationMapper mapper;

    @Override
    public EidAdministratorApplication create(EidAdministratorApplicationDTO dto) {
        return service.create(dto);
    }

    @Override
    public EidAdministratorApplicationResponseDTO getByID(UUID id) {
        EidAdministratorApplication entity = service.getById(id);
        return mapper.mapToDto(entity);
    }

    @Override
    public Page<EidAdministratorApplicationShortDTO> getByFilter(EidApplicationFilter filter, Pageable pageable) {
        return service.getByFilter(filter, pageable);
    }
}
