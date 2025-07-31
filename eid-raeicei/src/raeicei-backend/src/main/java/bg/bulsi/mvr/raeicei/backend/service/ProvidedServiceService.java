package bg.bulsi.mvr.raeicei.backend.service;

import java.util.List;
import java.util.UUID;

import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceRequestDTO;
import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import bg.bulsi.mvr.raeicei.model.enums.EidServiceType;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;

public interface ProvidedServiceService {

	ProvidedService getProvidedServiceById(UUID id);

	ProvidedService getProvidedServiceByApplicationType(EidServiceType applicationType);
	
    List<ProvidedService> getAllProvidedServices();

    List<ProvidedService> getAllProvidedServicesByType(ManagerType serviceType);
    
    ProvidedService createProvidedService(ProvidedServiceRequestDTO dto);

    ProvidedService updateProvidedService(UUID id, ProvidedServiceRequestDTO dto);

    void deleteProvidedService(UUID id);
}
