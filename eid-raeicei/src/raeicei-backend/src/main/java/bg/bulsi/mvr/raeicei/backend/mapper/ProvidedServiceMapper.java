package bg.bulsi.mvr.raeicei.backend.mapper;

import java.util.List;

import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.Mappings;
import org.mapstruct.NullValueCheckStrategy;

import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceRequestDTO;
import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.ServiceType;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import bg.bulsi.mvr.raeicei.model.enums.EidServiceType;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public interface ProvidedServiceMapper {
	
	@Mappings({
		
		//@Mapping(target = "serviceType", source = "managerType"),
		@Mapping(target = "managerType", expression = "java(bg.bulsi.mvr.raeicei.model.enums.ManagerType.valueOf(dto.getServiceType().name()))")
	})
	ProvidedService toEntity(ProvidedServiceRequestDTO dto);
	
	List<ProvidedServiceResponseDTO> toEntityList(List<ProvidedService> list);
	
	@Mappings({
		
		//@Mapping(target = "serviceType", source = "managerType"),
		@Mapping(target = "serviceType", expression = "java(bg.bulsi.mvr.raeicei.contract.dto.ServiceType.fromValue(entity.getManagerType().name()))"),
		@Mapping(target = "applicationType", expression = "java(entity.getApplicationType() != null ? entity.getApplicationType().name() : null)")
	})
	ProvidedServiceResponseDTO toResponseDto(ProvidedService entity);
	
	   default ServiceType convertServiceType(ManagerType managerType) {
		      return ServiceType.fromValue(managerType.name());
		   }
}
