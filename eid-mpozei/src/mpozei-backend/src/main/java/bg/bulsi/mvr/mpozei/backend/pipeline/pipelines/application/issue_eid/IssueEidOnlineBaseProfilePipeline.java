package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.issue_eid;

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
public class IssueEidOnlineBaseProfilePipeline extends Pipeline<AbstractApplication> {
    private final ValidationService validationService;

    @Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType applicationSubmissionType = application.getSubmissionType();
        return applicationSubmissionType == ApplicationSubmissionType.BASE_PROFILE
                && application.getApplicationType() == ApplicationType.ISSUE_EID
                && application.getPipelineStatus() == PipelineStatus.INITIATED &&
                application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
    }

    @Override
    public void postProcess(AbstractApplication entity) {
        if (entity.getStatus() != ApplicationStatus.PENDING_PAYMENT) {
            entity.setStatus(ApplicationStatus.APPROVED);
        }
    }

    @Override
    public void preProcess(AbstractApplication entity) {
        validationService.validateIssueEidOnlineApplication(entity);
    }
}
