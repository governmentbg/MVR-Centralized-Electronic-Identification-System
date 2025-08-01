package bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.issue_eid;

import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import org.springframework.stereotype.Component;

import java.util.List;

import static bg.bulsi.mvr.common.pipeline.PipelineStatus.*;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.*;

@Component
public class EnrollForCertificatePipeline extends Pipeline<AbstractApplication> {
    @Override
    public boolean canProcess(AbstractApplication application) {
        ApplicationSubmissionType submissionType = application.getSubmissionType();
        Boolean isOnlineOffice = application.getParams().getIsOnlineOffice();
        boolean isMvrIssueEid = application.getApplicationType() == ApplicationType.ISSUE_EID
                && application.getEidAdministratorId().equals(MVRConstants.MVR_ADMINISTRATOR_ID);
        if (DESK == submissionType) {
            return List.of(ISSUE_EID_SIGNED, CREATE_PAYMENT).contains(application.getPipelineStatus()) &&
            		List.of(ApplicationStatus.SIGNED, ApplicationStatus.PAID).contains(application.getStatus()) &&
                    isMvrIssueEid;
//            case when you create the application online but you complete it on desk (CHIP_CARD)
        } else if (EID == submissionType && !isOnlineOffice) {
            return List.of(EXPORT_APPLICATION, ISSUE_EID_SIGNED, CREATE_PAYMENT).contains(application.getPipelineStatus()) &&
            		List.of(ApplicationStatus.SIGNED, ApplicationStatus.PAID).contains(application.getStatus()) &&
                    isMvrIssueEid;
//            case when you create the application online and you complete it online (MOBILE)
        } else if (EID == submissionType && isOnlineOffice) {
            return List.of(EXPORT_APPLICATION, CREATE_PAYMENT).contains(application.getPipelineStatus()) &&
            		List.of(ApplicationStatus.SIGNED, ApplicationStatus.PAID).contains(application.getStatus()) &&
                    isMvrIssueEid;
        } else if (BASE_PROFILE == submissionType) {
            return List.of(EXPORT_APPLICATION, ISSUE_EID_SIGNED, CREATE_PAYMENT).contains(application.getPipelineStatus()) &&
            		List.of(ApplicationStatus.SIGNED, ApplicationStatus.PAID).contains(application.getStatus()) &&
                    isMvrIssueEid;
        }
        return false;
    }

    @Override
    public void postProcess(AbstractApplication entity) {
        entity.setStatus(ApplicationStatus.GENERATED_CERTIFICATE);
    }
}
