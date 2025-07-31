package bg.bulsi.mvr.raeicei.model.repository.view;

import java.util.List;
import java.util.Map;
import java.util.UUID;

public interface EidAdministratorView extends EidManagerView{

    //Map<UUID,String> getDeviceIds();
    
    List<UUID> getDeviceIds();

    String getDevices();

    String getDownloadUrl();
}