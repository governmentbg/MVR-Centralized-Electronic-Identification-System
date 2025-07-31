package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.service.ApplicationService;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Lazy;
import org.springframework.stereotype.Component;

@Slf4j
@RequiredArgsConstructor
@Component
public class ExistingApplicationVerificationStep extends Step<AbstractApplication> {

	@Lazy
	@Autowired
    private ApplicationService applicationService;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered ExistingApplicationVerificationStep", application.getId());
    	
        if(this.applicationService.getActiveOnlineIssueApplicationsForChipCard(application.getCitizenProfileId(), application.getEidentityId(), application.getId())) {
			throw new ValidationMVRException(ErrorCode.ONLY_ONE_ACTIVE_APPLICATION_ALLOWED);
        }
        
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.EXISTING_APPLICATION_VERIFICATION;
    }
}
