package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.stop_eid;

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

import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.APPROVED;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.DESK;

@Component
@RequiredArgsConstructor
public class SignStopEidDeskPipeline extends Pipeline<AbstractApplication> {
    private final ValidationService validationService;
    
	@Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType submissionType = application.getSubmissionType();
        return submissionType == DESK &&
                application.getApplicationType() == ApplicationType.STOP_EID &&
                application.getStatus() == APPROVED &&
                application.getPipelineStatus() == PipelineStatus.EXPORT_APPLICATION
                && application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
    }
	
    @Override
    public void preProcess(AbstractApplication application) {
        validationService.validateSignStopEidApplication(application);
    }
	
    @Override
    public void postProcess(AbstractApplication entity) {
        entity.setStatus(ApplicationStatus.COMPLETED);
    }
}
