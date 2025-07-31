package bg.bulsi.mvr.raeicei.backend.facade.impl;

import bg.bulsi.mvr.raeicei.backend.facade.EidCenterApplicationFacade;
import bg.bulsi.mvr.raeicei.backend.mapper.EidCenterApplicationMapper;
import bg.bulsi.mvr.raeicei.backend.service.EidCenterApplicationService;
import bg.bulsi.mvr.raeicei.contract.dto.EidApplicationFilter;
import bg.bulsi.mvr.raeicei.contract.dto.EidCenterApplicationDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidCenterApplicationResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidCenterApplicationShortDTO;
import bg.bulsi.mvr.raeicei.model.entity.application.EidCenterApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class EidCenterApplicationFacadeImpl implements EidCenterApplicationFacade {

    private final EidCenterApplicationService service;
    private final EidCenterApplicationMapper mapper;

    @Override
    public EidCenterApplication create(EidCenterApplicationDTO dto) {
        return service.create(dto);
    }

    @Override
    public EidCenterApplicationResponseDTO getByID(UUID id) {
        EidCenterApplication entity = service.getById(id);
        return mapper.mapToDto(entity);
    }

    @Override
    public Page<EidCenterApplicationShortDTO> getByFilter(EidApplicationFilter filter, Pageable pageable) {
        return service.getByFilter(filter, pageable);
    }
}
