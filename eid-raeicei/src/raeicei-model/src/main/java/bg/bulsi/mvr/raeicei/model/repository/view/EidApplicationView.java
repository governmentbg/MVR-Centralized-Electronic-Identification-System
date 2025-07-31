package bg.bulsi.mvr.raeicei.model.repository.view;

import bg.bulsi.mvr.raeicei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.raeicei.model.enums.ApplicationType;

import java.time.LocalDateTime;
import java.util.UUID;


public interface EidApplicationView {
    UUID getId();

    String getApplicationNumber();

    String getEidName();

    ApplicationType getApplicationType();

    ApplicationStatus getStatus();

    LocalDateTime getCreateDate();
}
