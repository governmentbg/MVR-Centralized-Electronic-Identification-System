package bg.bulsi.mvr.raeicei.model.repository.view;

import java.util.List;
import java.util.UUID;

public interface EidAdministratorAuthorizedView extends EidManagerAuthorizedView {

    List<UUID> getDeviceIds();

    String getDownloadUrl();
}