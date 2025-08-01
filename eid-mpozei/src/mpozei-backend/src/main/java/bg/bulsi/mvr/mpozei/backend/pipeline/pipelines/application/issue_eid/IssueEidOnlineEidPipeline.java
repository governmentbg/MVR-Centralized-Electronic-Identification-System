package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.issue_eid;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.backend.service.RaeiceiService;
import bg.bulsi.mvr.mpozei.backend.service.impl.ValidationService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceType;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Component;

import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.*;

@Component
@RequiredArgsConstructor
public class IssueEidOnlineEidPipeline extends Pipeline<AbstractApplication> {
    private final ValidationService validationService;
    private final RaeiceiService raeiceiService;

    @Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType applicationSubmissionType = application.getSubmissionType();
        return applicationSubmissionType == ApplicationSubmissionType.EID &&
                application.getApplicationType() == ApplicationType.ISSUE_EID &&
                application.getPipelineStatus() == PipelineStatus.INITIATED &&
                application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
    }

    @Override
    public void postProcess(AbstractApplication entity) {
        DeviceDTO device = raeiceiService.getDeviceById(entity.getDeviceId());
        if (entity.getStatus() != PENDING_PAYMENT) {
            if (device.getType().equals(DeviceType.MOBILE)) {
                entity.setStatus(SIGNED);
            } else if (device.getType().equals(DeviceType.CHIP_CARD)) {
                entity.setStatus(APPROVED);
            }
        }
    }

    @Override
    public void preProcess(AbstractApplication entity) {
        validationService.validateIssueEidOnlineEidApplication(entity);
    }
}
