package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

@Slf4j
@Component
@RequiredArgsConstructor
public class SignApplicationStep extends Step<AbstractApplication> {
    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered SignApplicationStep", application.getId());
        if (application.getStatus() != ApplicationStatus.PENDING_PAYMENT) {
            application.setStatus(ApplicationStatus.SIGNED);
        }
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.ISSUE_EID_SIGNED;
    }
}
