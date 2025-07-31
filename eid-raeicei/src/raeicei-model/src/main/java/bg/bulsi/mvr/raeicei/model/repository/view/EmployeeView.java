package bg.bulsi.mvr.raeicei.model.repository.view;

import bg.bulsi.mvr.raeicei.model.enums.IdentifierType;

import java.util.List;
import java.util.UUID;


public interface EmployeeView {
    UUID getId();

    List<String> getRoles();

    String getName();

    String getNameLatin();

    String getPhoneNumber();

    Boolean getIsActive();

    String getEmail();

    IdentifierType getCitizenIdentifierType();

    String getCitizenIdentifierNumber();
}
