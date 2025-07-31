package bg.bulsi.mvr.raeicei.backend.facade.impl;

import bg.bulsi.mvr.raeicei.backend.config.RedisConfig;
import bg.bulsi.mvr.raeicei.backend.facade.ProvidedServiceFacade;
import bg.bulsi.mvr.raeicei.backend.mapper.ProvidedServiceMapper;
import bg.bulsi.mvr.raeicei.backend.service.ProvidedServiceService;
import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceRequestDTO;
import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import bg.bulsi.mvr.raeicei.model.enums.EidServiceType;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.cache.annotation.CacheEvict;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class ProvidedServiceFacadeImpl implements ProvidedServiceFacade {

    private final ProvidedServiceService providedServiceService;
    
	private final ProvidedServiceMapper providedServiceMapper;
    
    @Cacheable(value = RedisConfig.PROVIDED_SERVICE_CACHE, key = "#id")
	@Override
	public ProvidedServiceResponseDTO getProvidedServiceById(UUID id) {
		return this.providedServiceMapper.toResponseDto(providedServiceService.getProvidedServiceById(id));

	}
    
    @Cacheable(value = RedisConfig.PROVIDED_SERVICE_CACHE, key = "#applicationType")
	@Override
	public ProvidedServiceResponseDTO getProvidedServiceByApplicationType(EidServiceType applicationType) {
    	ProvidedService providedService = providedServiceService.getProvidedServiceByApplicationType(applicationType);
    	
    	ProvidedServiceResponseDTO dto  = this.providedServiceMapper.toResponseDto(providedService);
    	
		return dto;
	}
    
    @Cacheable(value = RedisConfig.PROVIDED_SERVICE_LIST_CACHE)
	@Override
	public List<ProvidedServiceResponseDTO> getAllProvidedServices() {
		return this.providedServiceMapper.toEntityList(providedServiceService.getAllProvidedServices());

	}
    
    @CacheEvict(cacheNames = {RedisConfig.PROVIDED_SERVICE_CACHE, RedisConfig.PROVIDED_SERVICE_LIST_CACHE}, allEntries = true)
	@Override
	public ProvidedServiceResponseDTO createProvidedService(ProvidedServiceRequestDTO dto) {
		return this.providedServiceMapper.toResponseDto( providedServiceService.createProvidedService(dto));
	}
	
    @CacheEvict(cacheNames = {RedisConfig.PROVIDED_SERVICE_CACHE, RedisConfig.PROVIDED_SERVICE_LIST_CACHE}, allEntries = true)
	@Override
	public ProvidedServiceResponseDTO updateProvidedService(UUID id, ProvidedServiceRequestDTO dto) {
		return this.providedServiceMapper.toResponseDto(providedServiceService.updateProvidedService(id, dto));
	}

	@CacheEvict(cacheNames = {RedisConfig.PROVIDED_SERVICE_CACHE, RedisConfig.PROVIDED_SERVICE_LIST_CACHE}, allEntries = true)
	@Override
	public void deleteProvidedService(UUID id) {
		this.providedServiceService.deleteProvidedService(id);
	}
}
