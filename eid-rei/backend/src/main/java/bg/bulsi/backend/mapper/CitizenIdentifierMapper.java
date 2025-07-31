package bg.bulsi.backend.mapper;

import bg.bulsi.mvr.reicontract.dto.CitizenDataDTO;
import bg.bulsi.mvr.reicontract.dto.EidentityDTO;
import bg.bulsi.mvr.reicontract.dto.IdentifierTypeDTO;
import bg.bulsi.reimodel.model.CitizenIdentifier;
import bg.bulsi.reimodel.repository.view.CitizenIdentifierView;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.MappingTarget;
import org.springframework.stereotype.Component;

import java.util.UUID;

@Component
@Mapper(componentModel = "spring")
public interface CitizenIdentifierMapper {
    @Mapping(source = "citizenIdentifierNumber", target = "number")
    @Mapping(source = "citizenIdentifierType", target = "type")
    CitizenIdentifier map(CitizenDataDTO eIdentityDTO);
    void update(CitizenDataDTO eIdentityDTO, @MappingTarget CitizenIdentifier eIdentity);

    default CitizenIdentifier mapToNewCI(CitizenIdentifier source) {
        if (source == null) {
            return null;
        }
        CitizenIdentifier target = new CitizenIdentifier();
        target.setFirstName(source.getFirstName());
        target.setSecondName(source.getSecondName());
        target.setLastName(source.getLastName());
        target.setActive(true);
        target.setEidentity(source.getEidentity());
        return target;
    }

    default EidentityDTO map(CitizenIdentifierView view){
        if (view == null) {
            return null;
        }
        EidentityDTO dto = new EidentityDTO();
        dto.setActive(view.getActive());
        dto.setId(view.getEidentityId());
        dto.setCitizenIdentifierNumber(view.getNumber());
        dto.setCitizenIdentifierType(IdentifierTypeDTO.valueOf(view.getType().name()));
        dto.setFirstName(view.getFirstName());
        dto.setSecondName(view.getSecondName());
        dto.setLastName(view.getLastName());
        return dto;
    }

    default CitizenDataDTO map(CitizenIdentifier entity) {
        if (entity == null) {
            return null;
        }
        CitizenDataDTO dto = new CitizenDataDTO();
        dto.setFirstName(entity.getFirstName());
        dto.setSecondName(entity.getSecondName());
        dto.setLastName(entity.getLastName());
        return dto;
    }

    default EidentityDTO mapToEidentityDTO(CitizenIdentifier entity) {
        if (entity == null) {
            return null;
        }
        EidentityDTO dto = new EidentityDTO();
        dto.setId(entity.getEidentity().getId());
        dto.setActive(entity.getActive());
        dto.setFirstName(entity.getFirstName());
        dto.setSecondName(entity.getSecondName());
        dto.setLastName(entity.getLastName());
        dto.setCitizenIdentifierNumber(entity.getNumber());
        dto.setCitizenIdentifierType(IdentifierTypeDTO.fromValue(entity.getType().name()));

        return dto;
    }

}
