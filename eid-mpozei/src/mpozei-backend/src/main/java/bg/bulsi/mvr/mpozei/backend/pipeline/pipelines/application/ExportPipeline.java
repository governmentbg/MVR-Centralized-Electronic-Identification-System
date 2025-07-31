package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.backend.service.RaeiceiService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceType;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import org.springframework.stereotype.Component;

import java.util.List;

import static bg.bulsi.mvr.common.pipeline.PipelineStatus.*;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.APPROVED;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.PAID;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.*;

@Component
public class ExportPipeline extends Pipeline<AbstractApplication> {
    private final RaeiceiService raeiceiService;

    public ExportPipeline(RaeiceiService raeiceiService) {
        this.raeiceiService = raeiceiService;
        this.setRepeatable(true);
    }

    @Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType submissionType = application.getSubmissionType();
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        if (submissionType == DESK) {
            return application.getPipelineStatus() == SIGNATURE_CREATION ||
                    application.getPipelineStatus() == EXPORT_APPLICATION
                            && application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
        } else if (BASE_PROFILE == submissionType) {
            return List.of(CREATE_PAYMENT, EXPORT_APPLICATION).contains(application.getPipelineStatus()) &&
                    List.of(PAID, APPROVED).contains(application.getStatus())
                    && application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
        } else if (EID == submissionType) {
            return device.getType().equals(DeviceType.CHIP_CARD) &&
                    List.of(CREATE_PAYMENT, EXPORT_APPLICATION).contains(application.getPipelineStatus()) &&
                    List.of(PAID, APPROVED).contains(application.getStatus())
                    && application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
        }
        return false;
    }
}
