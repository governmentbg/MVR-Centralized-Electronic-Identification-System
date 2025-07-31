package bg.bulsi.mvr.raeicei.backend.mapper;

import bg.bulsi.mvr.common.exception.FaultMVRException;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.*;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import bg.bulsi.mvr.raeicei.model.entity.application.AbstractApplication;
import bg.bulsi.mvr.raeicei.model.entity.application.EidAdministratorApplication;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import bg.bulsi.mvr.raeicei.model.repository.ProvidedServiceRepository;
import bg.bulsi.mvr.raeicei.model.repository.view.EidAdministratorAuthorizedView;
import bg.bulsi.mvr.raeicei.model.repository.view.EidAdministratorView;
import bg.bulsi.mvr.raeicei.model.repository.view.EidCenterAuthorizedView;
import bg.bulsi.mvr.raeicei.model.repository.view.EidCenterView;
import com.fasterxml.jackson.annotation.JsonInclude.Include;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.JsonMappingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.extern.slf4j.Slf4j;
import org.mapstruct.*;

import java.time.LocalDateTime;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.stream.Collectors;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
@Slf4j
public abstract class EidManagerMapper {

	public OffsetDateTime map(LocalDateTime dateTime) {
		return dateTime.atOffset(ZoneOffset.ofHours(0));
	}

	public LocalDateTime map(OffsetDateTime dateTime) {
		return dateTime.atZoneSameInstant(ZoneOffset.UTC).toLocalDateTime();
	}

	private ProvidedServiceRepository providedServiceRepository;
//    EidManager mapToEntity(EidManagerDTO dto);
//    @Mapping(target = "eidManagerFrontOffices", ignore = true)
//    EidManager mapToEntity(@MappingTarget EidManager entity , EidManagerDTO dto);
//    EidManagerDTO mapToDto(EidManager entity);
//    List<EidManager> mapToEntityList(List<EidManagerDTO> dtos);
//    List<EidManagerDTO> mapToDtoList(List<EidManager> entities);
//
//    EidManagerDTO map(EidManagerView view);
//
//    List<EidManagerDTO> map(List<EidManagerView> views);
	@Mapping(target = "deviceIds", expression = "java(new ArrayList<UUID>(deviceIds.keySet()))")
	abstract public List<EidAdministratorDTO> map(List<EidAdministratorView> views);

	abstract public List<EidAdministratorHistoryDTO> mapToHistoryDtoList(List<EidAdministratorView> views);

	abstract public EidAdministratorDTO map(EidAdministratorView view);

	abstract public EidAdministratorAuthorizedDTO map(EidAdministratorAuthorizedView view);

	abstract public EidAdministratorDTO mapToAdministratorDto(EidAdministrator eidAdministrator);

	abstract public EidAdministratorFullDTO mapToAdministratorFullDto(EidAdministrator eidAdministrator);

	abstract public EidCenterDTO mapToEidCenterDto(EidCenter eidCenter);

	abstract public EidCenterFullDTO mapToEidCenterFullDto(EidCenter eidCenter);

	abstract public EidCenter mapToEidCenterEntity(EidCenterDTO eidCenter);

	abstract public List<EidCenterDTO> mapToEidCenterDtoList(List<EidCenter> eidCenters);

	abstract public EidAdministrator mapToEntity(EidAdministratorDTO dto);

	abstract public EidAdministrator mapToEntity(@MappingTarget EidAdministrator entity, EidAdministratorDTO dto);

	abstract public EidCenterDTO mapViewToEidCenterDto(EidCenterView eidCenter);

	abstract public EidCenterAuthorizedDTO mapViewToEidCenterAuthorizedDto(EidCenterAuthorizedView eidCenter);

	abstract public List<EidCenterDTO> mapViewToEidCenterDtoList(List<EidCenterView> eidCenters);

	abstract public List<EidCenterHistoryDTO> mapToHistoryDto(List<EidCenterView> eidCenters);

	@Mappings({
			@Mapping(target = "name", source = "companyName"),
			@Mapping(target = "nameLatin", source = "companyNameLatin"),
			@Mapping(target = "eidManagerFrontOfficeIds", source = "application.eidManagerFrontOffices", qualifiedByName = "eidManagerFrontOfficesToListOfUuid"),
			@Mapping(target = "authorizedPersonsIds", source = "application.authorizedPersons", qualifiedByName = "authorizedPersonsToListOfUuid"),
			@Mapping(target = "employeesIds", source = "application.employees", qualifiedByName = "employeesToListOfUuid"),
			@Mapping(target = "eidFrontOffices", source = "application.eidManagerFrontOffices", qualifiedByName = "eidManagerFrontOfficesToListOfShortDto"),
			@Mapping(target = "deviceIds", source = "application.devices", qualifiedByName = "devicesToListOfUuid"),
			@Mapping(target = "attachmentIds", source = "application.attachments", qualifiedByName = "attachmentsToListOfUuid"),
			@Mapping(target = "noteIds", source = "application.notes", qualifiedByName = "notesToListOfUuid"),
			@Mapping(target = "serviceType", source = "managerType"),
	})
	abstract public EidAdministratorDTO mapApplicationToAdministratorDto(EidAdministratorApplication application);

	@Mappings({
			@Mapping(target = "name", source = "companyName"),
			@Mapping(target = "nameLatin", source = "companyNameLatin"),
			@Mapping(target = "eidManagerFrontOfficeIds", source = "application.eidManagerFrontOffices", qualifiedByName = "eidManagerFrontOfficesToListOfUuid"),
			@Mapping(target = "authorizedPersonsIds", source = "application.authorizedPersons", qualifiedByName = "authorizedPersonsToListOfUuid"),
			@Mapping(target = "employeesIds", source = "application.employees", qualifiedByName = "employeesToListOfUuid"),
			@Mapping(target = "attachmentIds", source = "application.attachments", qualifiedByName = "attachmentsToListOfUuid"),
			@Mapping(target = "noteIds", source = "application.notes", qualifiedByName = "notesToListOfUuid"),
			@Mapping(target = "serviceType", source = "managerType"),
	})
	abstract public EidCenterDTO mapApplicationToCenterDto(AbstractApplication application);

	@Named("devicesToListOfUuid")
	public List<UUID> mapDeviceIds(List<Device> devices) {
		return devices.stream().map(Device::getId).collect(Collectors.toList());
	}

	@Named("eidManagerFrontOfficesToListOfUuid")
	public List<UUID> mapEidManagerFrontOfficeIds(List<EidManagerFrontOffice> eidManagerFrontOffices) {
		return eidManagerFrontOffices.stream().map(EidManagerFrontOffice::getId).collect(Collectors.toList());
	}

	@Named("authorizedPersonsToListOfUuid")
	public List<UUID> mapAuthorizedPersonIds(List<Contact> authorizedPersons) {
		return authorizedPersons.stream().map(Contact::getId).collect(Collectors.toList());
	}

	@Named("employeesToListOfUuid")
	public List<UUID> mapEmployeesIds(List<Employee> employees) {
		return employees.stream().map(Employee::getId).collect(Collectors.toList());
	}

	@Named("eidManagerFrontOfficesToListOfShortDto")
	public List<FrontOfficeShortDTO> mapEidFrontOffices(List<EidManagerFrontOffice> eidManagerFrontOffices) {
		List<FrontOfficeShortDTO> frontOfficeShortDTOs = new ArrayList<>();
		for (EidManagerFrontOffice eidManagerFrontOffice : eidManagerFrontOffices) {
			FrontOfficeShortDTO frontOfficeShortDTO = new FrontOfficeShortDTO(eidManagerFrontOffice.getId(), eidManagerFrontOffice.getName(), eidManagerFrontOffice.getRegion().name());
			frontOfficeShortDTOs.add(frontOfficeShortDTO);
		}
		return frontOfficeShortDTOs;
	}

	@Named("attachmentsToListOfUuid")
	public List<UUID> mapAttachmentIds(List<Document> attachments) {
		return attachments.stream().map(Document::getId).collect(Collectors.toList());
	}

	@Named("notesToListOfUuid")
	public List<UUID> mapNotesIds(List<Note> notes) {
		return notes.stream().map(Note::getId).collect(Collectors.toList());
	}

	@Mapping(source = "eidManager.id", target = "eidManagerId")
	public abstract EidManagerFrontOfficeResponseDTO mapToResponseDto(EidManagerFrontOffice entity);

	public EidManagerStatus mapAppStatus(ApplicationStatus dto){
		return EidManagerStatus.valueOf(dto.name());
	}

	@Mappings({
			@Mapping(target = "serviceType", source = "managerType")
	})
	abstract public ProvidedServiceResponseDTO mapToProvidedServiceResponseDto(ProvidedService providedService);

	public List<FrontOfficeShortDTO> map(String source) {
		ObjectMapper mapper = new ObjectMapper();
		mapper.disable(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES);
		mapper.enable(DeserializationFeature.ACCEPT_EMPTY_ARRAY_AS_NULL_OBJECT);
		mapper.setSerializationInclusion(Include.NON_NULL);
		
		log.info("Offices Json Map: {}" , source);
		List<FrontOfficeShortDTO> details=null;
		if (!"[{}]".equals(source) && source != null) {
			try {
				 details = mapper.readValue(source, new TypeReference<List<FrontOfficeShortDTO>>(){});
			} catch (JsonMappingException e) {
				log.error(e.getMessage(),e);
				throw new FaultMVRException(e.getCause());
			} catch (JsonProcessingException e) {
				log.error(e.getMessage(),e);
				throw new FaultMVRException(e.getCause());
			}

		}
		log.info("List Result: {}",details);
		return details;
	}

	public List<ContactDTO> maptoContact(String source) {
		ObjectMapper mapper = new ObjectMapper();
		mapper.disable(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES);
		mapper.enable(DeserializationFeature.ACCEPT_EMPTY_ARRAY_AS_NULL_OBJECT);
		mapper.setSerializationInclusion(Include.NON_NULL);

		log.info("AuthorizedPersons Json Map: {}", source);
		List<ContactDTO> details = null;
		if (!"[{}]".equals(source) && source != null) {
			try {
				details = mapper.readValue(source, new TypeReference<List<ContactDTO>>() {
				});
			} catch (JsonMappingException e) {
				log.error(e.getMessage(), e);
				throw new FaultMVRException(e.getCause());
			} catch (JsonProcessingException e) {
				log.error(e.getMessage(), e);
				throw new FaultMVRException(e.getCause());
			}

		}
		log.info("List Result: {}", details);
		return details;
	}

	public List<DeviceDTO> mapToDevice(String source) {
		ObjectMapper mapper = new ObjectMapper();
		mapper.disable(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES);
		mapper.enable(DeserializationFeature.ACCEPT_EMPTY_ARRAY_AS_NULL_OBJECT);
		mapper.setSerializationInclusion(Include.NON_NULL);

		log.info("Devices Json Map: {}", source);
		List<DeviceDTO> details = null;
		if (!"[{}]".equals(source) && source != null) {
			try {
				details = mapper.readValue(source, new TypeReference<List<DeviceDTO>>() {
				});
			} catch (JsonMappingException e) {
				log.error(e.getMessage(), e);
				throw new FaultMVRException(e.getCause());
			} catch (JsonProcessingException e) {
				log.error(e.getMessage(), e);
				throw new FaultMVRException(e.getCause());
			}

		}
		log.info("List Result: {}", details);
		return details;
	}

	public List<UUID> map(Map<UUID, String> value) {
		return new ArrayList<UUID>(value.keySet());
	}

	@Mappings({
//			@Mapping(target = "citizenIdentifierType", expression = "java(bg.bulsi.mvr.raeicei.contract.dto.IdentifierTypeDTO.fromValue(entity.getCitizenIdentifierType().name()))"),
			@Mapping(source = "EIdentity", target = "eIdentity")
	})
	public abstract ContactDTO mapToAuthorizedPersonDto(Contact entity);

	public FrontOfficeShortDTO map(String[] source) {
		if (source.length == 3) {
			FrontOfficeShortDTO dest = new FrontOfficeShortDTO(UUID.fromString(source[0]), source[1], source[3]);
			return dest;
		} else {
			return null;
		}

	}

	public ContactDTO mapToContact(String[] source) {
		if (source.length == 9) {
			ContactDTO dest = new ContactDTO(UUID.fromString(source[0]), source[1], source[2], Boolean.valueOf(source[3]), source[4], source[5], IdentifierTypeDTO.valueOf(source[6]), source[7], UUID.fromString(source[8]));
			return dest;
		} else {
			return null;
		}
	}

	public DeviceDTO mapToDevice(String[] source) {
		if (source.length == 7) {
			DeviceDTO dest = new DeviceDTO(UUID.fromString(source[0]), source[1], DeviceType.fromValue(source[2]), source[3], source[4], source[5], Boolean.valueOf(source[6]));
			return dest;
		} else {
			return null;
		}
	}
}
