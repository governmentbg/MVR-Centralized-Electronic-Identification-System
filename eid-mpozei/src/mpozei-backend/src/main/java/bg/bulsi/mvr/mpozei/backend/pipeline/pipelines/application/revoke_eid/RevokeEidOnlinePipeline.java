package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.revoke_eid;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.backend.service.impl.ValidationService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Component;

import java.util.List;

import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.EID;

@Component
@RequiredArgsConstructor
public class RevokeEidOnlinePipeline extends Pipeline<AbstractApplication> {
    private final ValidationService validationService;

    @Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType applicationSubmissionType = application.getSubmissionType();
        return List.of(EID).contains(applicationSubmissionType)
                && application.getApplicationType() == ApplicationType.REVOKE_EID
                && application.getPipelineStatus() == PipelineStatus.INITIATED
                && application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
    }

    @Override
    public void postProcess(AbstractApplication entity) {
//        if (entity.getStatus() != ApplicationStatus.PENDING_PAYMENT) {
//            entity.setStatus(ApplicationStatus.APPROVED);
//        }
    }

    @Override
    public void preProcess(AbstractApplication entity) {
        validationService.validateRevokeEidFromDeskApplication(entity);
    }
}
