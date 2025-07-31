package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.external_administrators;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import org.springframework.stereotype.Component;

import java.util.List;

import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.*;

@Component
public class CompleteIssueEidExtAdminPipeline extends Pipeline<AbstractApplication> {
    @Override
    public boolean canProcess(AbstractApplication application) {
        if (List.of(EID, DESK, BASE_PROFILE).contains(application.getSubmissionType())) {
            return application.getStatus() == ApplicationStatus.CERTIFICATE_STORED &&
                    application.getPipelineStatus() == PipelineStatus.SEND_NOTIFICATION
                    && !application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
        }
        return false;
    }

    @Override
    public void postProcess(AbstractApplication entity) {
        entity.setStatus(ApplicationStatus.COMPLETED);
    }
}
