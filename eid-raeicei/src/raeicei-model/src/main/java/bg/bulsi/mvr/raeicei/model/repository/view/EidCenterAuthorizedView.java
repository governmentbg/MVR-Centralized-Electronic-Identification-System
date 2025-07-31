package bg.bulsi.mvr.raeicei.model.repository.view;

public interface EidCenterAuthorizedView extends EidManagerAuthorizedView {

    String getClientId();

    String getClientSecret();
}