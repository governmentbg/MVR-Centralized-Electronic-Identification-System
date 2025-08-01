package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.external_administrators.issue_eid;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.backend.service.impl.ValidationService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Component;

@Component
@RequiredArgsConstructor
public class IssueEidDeskExtAdminPipeline extends Pipeline<AbstractApplication> {


    private final ValidationService validationService;

    @Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType submissionType = application.getSubmissionType();
        return submissionType == ApplicationSubmissionType.DESK
                && application.getApplicationType() == ApplicationType.ISSUE_EID
                && application.getPipelineStatus() == PipelineStatus.INITIATED
                && !application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
    }

    @Override
    public void preProcess(AbstractApplication entity) {
        validationService.validateCreateIssueEidApplication(entity);
    }
}
