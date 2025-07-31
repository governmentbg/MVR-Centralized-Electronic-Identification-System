package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.revoke_eid;

import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.BASE_PROFILE;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.EID;

import java.util.List;

import bg.bulsi.mvr.common.util.MVRConstants;
import org.springframework.stereotype.Component;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;

@Component
public class CompleteRevokeOnlinePipeline extends Pipeline<AbstractApplication> {

    @Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType submissionType = application.getSubmissionType();
        if (List.of(EID, BASE_PROFILE).contains(submissionType)) {
            return application.getStatus() == ApplicationStatus.PAID &&
                    application.getApplicationType() == ApplicationType.REVOKE_EID
                    && application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
        }
        return false;
    }

    @Override
    public void postProcess(AbstractApplication entity) {
        entity.setStatus(ApplicationStatus.COMPLETED);
    }
}
