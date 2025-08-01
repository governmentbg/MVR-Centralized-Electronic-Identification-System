package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.issue_eid;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.backend.service.impl.ValidationService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Component;

import java.util.List;

import static bg.bulsi.mvr.common.pipeline.PipelineStatus.EXPORT_APPLICATION;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.APPROVED;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.PAID;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.BASE_PROFILE;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.EID;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationType.ISSUE_EID;

@Component
@RequiredArgsConstructor
public class SignIssueEidOnlinePipeline extends Pipeline<AbstractApplication> {
    private final ValidationService validationService;

    @Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType submissionType = application.getSubmissionType();
        return List.of(BASE_PROFILE, EID).contains(submissionType) &&
                List.of(PAID, APPROVED).contains(application.getStatus()) &&
                application.getPipelineStatus() == EXPORT_APPLICATION &&
                application.getApplicationType() == ISSUE_EID &&
                application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
    }

    @Override
    public void preProcess(AbstractApplication entity) {
        validationService.validateSignApplication(entity);
    }
    
    @Override
    public void postProcess(AbstractApplication entity) {
        entity.setStatus(ApplicationStatus.SIGNED);
    }
}
