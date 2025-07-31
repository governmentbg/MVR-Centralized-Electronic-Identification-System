package bg.bulsi.mvr.mpozei.backend.pipeline.steps.mock;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Component;

@Slf4j
@RequiredArgsConstructor
@Component(value="ejbcaEndEntityCreationStep")
@ConditionalOnProperty(prefix = "certificate-creation", name = "dev", havingValue = "true")
public class MockEjbcaEndEntityCreationStep extends Step<AbstractApplication> {
    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered EjbcaCertificateRevocationStep", application.getId());
        createEndEntityInEjbca(application);
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.EJBCA_END_ENTITY_CREATION;
    }

    private void createEndEntityInEjbca(AbstractApplication application) {
        log.info("Sending request to create end entity in ejbca for eidentityId: {}", application.getEidentityId());
        application.getParams().setEndEntityProfileName("End Entity MVR DEV Profile");
        log.info("Received success response for end entity creation for eidentityId: {}", application.getEidentityId());
    }
}
