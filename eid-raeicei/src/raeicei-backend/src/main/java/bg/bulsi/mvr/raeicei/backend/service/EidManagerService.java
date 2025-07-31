package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.model.entity.EidManager;

import java.util.List;
import java.util.UUID;

public interface EidManagerService {
	
	void addProvidedServicesToEidManager(EidManager eidManager);
	
	void addFrontOfficesToEidManager(EidManager eidManager, List<UUID> frontOfficesIds);

	void addAuthorizedPersonsToEidManager(EidManager eidManager, List<UUID> authorizedPersonsIds);

	void addEmployeesToEidManager(EidManager eidManager, List<UUID> employeesIds);
}
