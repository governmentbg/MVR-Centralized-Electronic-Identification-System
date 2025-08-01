package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.external_administrators.stop_eid;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.backend.service.impl.ValidationService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.application.StopEidApplication;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Component;

import java.util.List;

@Component
@RequiredArgsConstructor
public class StopEidOnlineExtAdminPipeline extends Pipeline<AbstractApplication> {
    private final ValidationService validationService;

    @Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType applicationSubmissionType = application.getSubmissionType();
        return List.of(ApplicationSubmissionType.BASE_PROFILE, ApplicationSubmissionType.EID).contains(applicationSubmissionType)
                && application.getApplicationType() == ApplicationType.STOP_EID
                && application.getPipelineStatus() == PipelineStatus.INITIATED
                && !application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
    }

    @Override
    public void preProcess(AbstractApplication entity) {
        validationService.validateStopEidApplication((StopEidApplication)entity);
    }
}
