package bg.bulsi.mvr.raeicei.model.repository.view;

import java.io.Serializable;
import java.util.UUID;

public interface EIdFronOfficeView  extends Serializable{
	
	EidManagerView getManager();
	String getId();
	String getName();
	String getregion();
	
	
}
