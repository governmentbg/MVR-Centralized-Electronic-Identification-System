package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.external_administrators.issue_eid;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import org.springframework.stereotype.Component;

import java.util.List;
import java.util.Objects;

import static bg.bulsi.mvr.common.pipeline.PipelineStatus.ISSUE_EID_SIGNED;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.*;

@Component
public class EnrollForCertificateExtAdminPipeline extends Pipeline<AbstractApplication> {
    @Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType submissionType = application.getSubmissionType();
        if (List.of(DESK, EID, BASE_PROFILE).contains(submissionType)) {
            return ISSUE_EID_SIGNED == application.getPipelineStatus()
                    && Objects.equals(ApplicationStatus.SIGNED, application.getStatus())
                    && application.getApplicationType() == ApplicationType.ISSUE_EID
                    && !application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
        }
        return false;
    }

    @Override
    public void postProcess(AbstractApplication entity) {
        entity.setStatus(ApplicationStatus.GENERATED_CERTIFICATE);
    }
}
