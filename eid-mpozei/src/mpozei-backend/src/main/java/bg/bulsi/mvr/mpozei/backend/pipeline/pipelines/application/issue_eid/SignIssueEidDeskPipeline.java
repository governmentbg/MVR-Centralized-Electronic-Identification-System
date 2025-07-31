package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.issue_eid;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.backend.service.impl.ValidationService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Component;

import java.util.List;

import static bg.bulsi.mvr.common.pipeline.PipelineStatus.EXPORT_APPLICATION;
import static bg.bulsi.mvr.common.pipeline.PipelineStatus.ISSUE_EID_SIGNED;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.DESK;

@Component
@RequiredArgsConstructor
public class SignIssueEidDeskPipeline extends Pipeline<AbstractApplication> {
    private final ValidationService validationService;

    @Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType submissionType = application.getSubmissionType();
        return submissionType == DESK &&
                List.of(EXPORT_APPLICATION, ISSUE_EID_SIGNED).contains(application.getPipelineStatus()) &&
                application.getApplicationType() == ApplicationType.ISSUE_EID &&
                application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
    }

    @Override
    public void preProcess(AbstractApplication entity) {
        validationService.validateSignApplication(entity);
    }
    
    @Override
    public void postProcess(AbstractApplication entity) {
        if (entity.getStatus() != ApplicationStatus.PENDING_PAYMENT) {
            entity.setStatus(ApplicationStatus.SIGNED);
        }
    }
}
