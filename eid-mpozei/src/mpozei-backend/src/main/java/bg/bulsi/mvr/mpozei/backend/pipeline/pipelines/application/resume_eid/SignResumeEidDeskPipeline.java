package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.resume_eid;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.backend.service.impl.ValidationService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Component;

@Component
@RequiredArgsConstructor
public class SignResumeEidDeskPipeline extends Pipeline<AbstractApplication> {
    private final ValidationService validationService;
    
	@Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType applicationSubmissionType = application.getSubmissionType();
        return applicationSubmissionType == ApplicationSubmissionType.DESK &&
                application.getApplicationType() == ApplicationType.RESUME_EID
        		&& application.getStatus() == ApplicationStatus.APPROVED
        		&& application.getPipelineStatus() == PipelineStatus.EXPORT_APPLICATION
                && application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
    }
	
    @Override
    public void preProcess(AbstractApplication application) {
        validationService.validateSignResumeEidFromDeskApplication(application);
    }
	
    @Override
    public void postProcess(AbstractApplication entity) {
        entity.setStatus(ApplicationStatus.COMPLETED);
    }
}
