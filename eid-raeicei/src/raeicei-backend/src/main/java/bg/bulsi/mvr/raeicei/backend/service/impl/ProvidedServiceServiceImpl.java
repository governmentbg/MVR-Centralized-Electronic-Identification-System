package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.raeicei.backend.mapper.ProvidedServiceMapper;
import bg.bulsi.mvr.raeicei.backend.service.ProvidedServiceService;
import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceRequestDTO;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import bg.bulsi.mvr.raeicei.model.enums.EidServiceType;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;
import bg.bulsi.mvr.raeicei.model.repository.ProvidedServiceRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class ProvidedServiceServiceImpl implements ProvidedServiceService {

	private final ProvidedServiceRepository providedServiceRepository;
	
	private final ProvidedServiceMapper providedServiceMapper;
	
	@Override
	public ProvidedService getProvidedServiceById(UUID id) {
		return this.providedServiceRepository.findById(id)
				.orElseThrow(() -> new EntityNotFoundException(ErrorCode.PROVIDED_SERVICE_NOT_FOUND_BY_ID, id.toString()));
	}


	@Override
	public ProvidedService getProvidedServiceByApplicationType(EidServiceType applicationType) {
		return this.providedServiceRepository.findByApplicationTypeAndApplicationTypeIsNotNull(applicationType)
				.orElseThrow(() -> new EntityNotFoundException(ErrorCode.PROVIDED_SERVICE_NOT_FOUND_BY_ID, applicationType.toString()));
	}
	
	@Override
	public List<ProvidedService> getAllProvidedServices() {
		return this.providedServiceRepository.findAll();
	}

	@Override
	public ProvidedService createProvidedService(ProvidedServiceRequestDTO dto) {
		//TODO: should we add the newly created ProvidedService to existing EidManager depending on type
		ProvidedService providedService = this.providedServiceMapper.toEntity(dto);
		providedService.setIsActive(null);
		return this.providedServiceRepository.save(providedService);
	}

	@Override
	public ProvidedService updateProvidedService(UUID id, ProvidedServiceRequestDTO dto) {
		ProvidedService providedService = this.providedServiceRepository.findById(id)
				.orElseThrow(() -> new EntityNotFoundException(ErrorCode.PROVIDED_SERVICE_NOT_FOUND_BY_ID, id.toString()));

		providedService.setName(dto.getName());
		providedService.setNameLatin(dto.getNameLatin());
		providedService.setManagerType(ManagerType.valueOf(dto.getServiceType().name()));
		providedService.setIsActive(null);
		
		return this.providedServiceRepository.save(providedService);
	}

	@Override
	public List<ProvidedService> getAllProvidedServicesByType(ManagerType serviceType) {
		return this.providedServiceRepository.findAllByManagerType(serviceType);
	}

	@Override
	public void deleteProvidedService(UUID id) {
		ProvidedService providedService = getProvidedServiceById(id);
		providedService.setIsActive(false);
	}
}
