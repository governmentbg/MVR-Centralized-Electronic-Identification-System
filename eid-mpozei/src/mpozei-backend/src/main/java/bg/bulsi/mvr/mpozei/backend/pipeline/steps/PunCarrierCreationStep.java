package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.PunClient;
import bg.bulsi.mvr.mpozei.backend.dto.PunCreateCarrierRequest;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.util.UUID;

@Slf4j
@Component
@RequiredArgsConstructor
public class PunCarrierCreationStep extends Step<AbstractApplication> {
    private final PunClient punClient;
    private final ApplicationMapper applicationMapper;

    @Override
    public AbstractApplication process(AbstractApplication input) {
        log.info("Application with id: {} entered PunCarrierCreationStep", input.getId());
        PunCreateCarrierRequest request = applicationMapper.mapToPunCarrierRequest(input);
        UUID punCertificateId = punClient.createPunCarrier(request);
        return input;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.PUN_CERTIFICATE_CREATION;
    }
}
