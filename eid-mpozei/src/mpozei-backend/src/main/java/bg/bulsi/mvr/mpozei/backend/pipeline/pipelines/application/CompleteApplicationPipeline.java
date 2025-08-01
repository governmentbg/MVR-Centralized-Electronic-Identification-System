package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import org.springframework.stereotype.Component;

import java.util.List;

import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.*;

@Component
public class CompleteApplicationPipeline extends Pipeline<AbstractApplication> {
    @Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType submissionType = application.getSubmissionType();
        if (List.of(EID, DESK, BASE_PROFILE).contains(submissionType)) {
            return application.getStatus() == ApplicationStatus.CERTIFICATE_STORED &&
                    application.getPipelineStatus() == PipelineStatus.SEND_NOTIFICATION
                    && application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
        }
        return false;
    }

    @Override
    public void postProcess(AbstractApplication entity) {
        entity.setStatus(ApplicationStatus.COMPLETED);
    }
}
