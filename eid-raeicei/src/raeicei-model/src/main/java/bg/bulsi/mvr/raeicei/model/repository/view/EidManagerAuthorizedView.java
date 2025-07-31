package bg.bulsi.mvr.raeicei.model.repository.view;

import java.util.List;
import java.util.UUID;

public interface EidManagerAuthorizedView {

    UUID getId();

    String getName();

    String getNameLatin();

    String getEikNumber();

    String getAddress();

    String getEmail();

    String getHomePage();

    String getManagerStatus();

    String getLogoUrl();

    String getCode();

    String getServiceType();

    List<UUID> getEidManagerFrontOfficeIds();

    List<UUID> getEmployeeIds();

    List<UUID> getAttachmentIds();

    List<UUID> getNoteIds();

    List<UUID> getProvidedServiceIds();

    String getAuthorizedPersons();
}