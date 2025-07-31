package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.ReiClient;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenDataDTO;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.EIDENTITY_CANNOT_BE_CREATED;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertNotNull;

@Slf4j
@Component
@RequiredArgsConstructor
public class ReiEidentityCreationStep extends Step<AbstractApplication> {
    private final ReiClient reiClient;
    private final ApplicationMapper applicationMapper;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered ReiEidentityCreationStep", application.getId());
        UUID eidentityId = createEidentityInRei(application);
        application.setEidentityId(eidentityId);
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.REI_EIDENTITY_CREATION;
    }

    private UUID createEidentityInRei(AbstractApplication input) {
        CitizenDataDTO citizenData = applicationMapper.mapToCitizenDataDTO(input);
        log.info("Sending request to create EIdentity");
        UUID eidentityId = reiClient.createEidentity(citizenData);
        assertNotNull(eidentityId, EIDENTITY_CANNOT_BE_CREATED);
        log.info("Received success response for EIdentity creation for eidentityId: {}", eidentityId);
        return eidentityId;
    }
}
