package bg.bulsi.mvr.raeicei.backend.facade;

import java.util.List;
import java.util.UUID;

import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceRequestDTO;
import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceResponseDTO;
import bg.bulsi.mvr.raeicei.model.enums.EidServiceType;

public interface ProvidedServiceFacade {
    ProvidedServiceResponseDTO getProvidedServiceById(UUID id);

    ProvidedServiceResponseDTO getProvidedServiceByApplicationType(EidServiceType applicationType);
    
    List<ProvidedServiceResponseDTO> getAllProvidedServices();

    ProvidedServiceResponseDTO createProvidedService(ProvidedServiceRequestDTO dto);

    ProvidedServiceResponseDTO updateProvidedService(UUID id,ProvidedServiceRequestDTO dto);

    void deleteProvidedService(UUID id);
}
