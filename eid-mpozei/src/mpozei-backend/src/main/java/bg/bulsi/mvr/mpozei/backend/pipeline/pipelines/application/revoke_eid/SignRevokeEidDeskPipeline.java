package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.revoke_eid;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.backend.service.impl.ValidationService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Component;

@Component
@RequiredArgsConstructor
public class SignRevokeEidDeskPipeline extends Pipeline<AbstractApplication> {
    private final ValidationService validationService;

    @Override
    public boolean canProcess(AbstractApplication entity) {
        return entity.getApplicationType() == ApplicationType.REVOKE_EID
                && entity.getStatus() == ApplicationStatus.APPROVED
                && entity.getPipelineStatus() == PipelineStatus.EXPORT_APPLICATION
                && entity.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
    }

    @Override
    public void preProcess(AbstractApplication application) {
        validationService.validateSignRevokeEidFromDeskApplication(application);
    }

    @Override
    public void postProcess(AbstractApplication entity) {
        entity.setStatus(ApplicationStatus.COMPLETED);
    }
}
