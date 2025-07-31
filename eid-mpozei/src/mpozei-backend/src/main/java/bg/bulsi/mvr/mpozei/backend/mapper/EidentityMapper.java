package bg.bulsi.mvr.mpozei.backend.mapper;

import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenProfileAttachDTO;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenProfileDTO;
import bg.bulsi.mvr.mpozei.backend.dto.EidentityDTO;
import bg.bulsi.mvr.mpozei.backend.dto.ProfileStatus;
import bg.bulsi.mvr.mpozei.contract.dto.CitizenProfileResponse;
import bg.bulsi.mvr.mpozei.contract.dto.EidentityExternalResponse;
import bg.bulsi.mvr.mpozei.contract.dto.EidentityResponse;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.NullValueCheckStrategy;
import org.springframework.stereotype.Component;

import java.util.Objects;

@Component
@Mapper(componentModel = "spring",
        nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public abstract class EidentityMapper {

    public EidentityResponse map(EidentityDTO eidentity, CitizenProfileDTO citizenProfile) {
        EidentityResponse response = new EidentityResponse();
        if (eidentity != null) {
            response.setEidentityId(eidentity.getId());
            response.setActive(eidentity.getActive());
            response.setFirstName(eidentity.getFirstName());
            response.setSecondName(eidentity.getSecondName());
            response.setLastName(eidentity.getLastName());
            response.setCitizenIdentifierNumber(eidentity.getCitizenIdentifierNumber());
            response.setCitizenIdentifierType(eidentity.getCitizenIdentifierType());
        }
        if (citizenProfile != null) {
        	response.setActive(ProfileStatus.ENABLED == citizenProfile.getStatus());
        	response.setCitizenProfileId(citizenProfile.getId());
            response.setEmail(citizenProfile.getEmail());
            response.setPhoneNumber(citizenProfile.getPhoneNumber());
            response.setFirebaseId(citizenProfile.getFirebaseId());
            response.setFirstNameLatin(citizenProfile.getFirstNameLatin());
            response.setSecondNameLatin(citizenProfile.getSecondNameLatin());
            response.setLastNameLatin(citizenProfile.getLastNameLatin());
        }
        
        return response;
    }

//    public EidentityResponse map(EidentityDTO eidentity, CitizenProfileResponse citizenProfile) {
//        EidentityResponse response = new EidentityResponse();
//        if (eidentity != null) {
//            response.setEidentityId(eidentity.getId());
//            response.setActive(eidentity.getActive());
//            response.setFirstName(eidentity.getFirstName());
//            response.setSecondName(eidentity.getSecondName());
//            response.setLastName(eidentity.getLastName());
//            if (Objects.nonNull(UserContextHolder.getFromServletContext().getRequesterUserId())) {
//                response.setCitizenIdentifierNumber(eidentity.getCitizenIdentifierNumber());
//                response.setCitizenIdentifierType(eidentity.getCitizenIdentifierType());
//            }
//        }
//        if (citizenProfile != null) {
//            response.setEmail(citizenProfile.getEmail());
//            response.setPhoneNumber(citizenProfile.getPhoneNumber());
//            response.setCitizenProfileId(citizenProfile.getId());
//            response.setFirebaseId(citizenProfile.getFirebaseId());
//            response.setFirstNameLatin(citizenProfile.getFirstNameLatin());
//            response.setSecondNameLatin(citizenProfile.getSecondNameLatin());
//            response.setLastNameLatin(citizenProfile.getLastNameLatin());
//            if (eidentity == null) {
//                response.setFirstName(citizenProfile.getFirstName());
//                response.setSecondName(citizenProfile.getSecondName());
//                response.setLastName(citizenProfile.getLastName());
//            }
//        }
//        return response;
//    }

    public EidentityExternalResponse mapToEidentityExternalResponse(EidentityDTO eidentity, CitizenProfileDTO citizenProfile) {
    	EidentityExternalResponse response = new EidentityExternalResponse();
        if (eidentity != null) {
            response.setEidentityId(eidentity.getId());
            response.setActive(eidentity.getActive());
            response.setFirstName(eidentity.getFirstName());
            response.setSecondName(eidentity.getSecondName());
            response.setLastName(eidentity.getLastName());
            if (Objects.nonNull(UserContextHolder.getFromServletContext().getEidentityId())) {
                response.setCitizenIdentifierNumber(eidentity.getCitizenIdentifierNumber());
                response.setCitizenIdentifierType(eidentity.getCitizenIdentifierType());
            }
        }
        if (citizenProfile != null) {
            response.setEmail(citizenProfile.getEmail());
            response.setPhoneNumber(citizenProfile.getPhoneNumber());
            response.setCitizenProfileId(citizenProfile.getId());
            response.setFirebaseId(citizenProfile.getFirebaseId());
            response.setFirstNameLatin(citizenProfile.getFirstNameLatin());
            response.setSecondNameLatin(citizenProfile.getSecondNameLatin());
            response.setLastNameLatin(citizenProfile.getLastNameLatin());
            response.setIs2FaEnabled(citizenProfile.getIs2FaEnabled());
            response.setFirstName(citizenProfile.getFirstName());
            response.setSecondName(citizenProfile.getSecondName());
            response.setLastName(citizenProfile.getLastName());
        }
        return response;
    }
    
    @Mapping(source = "id", target = "eidentityId")
    public abstract CitizenProfileAttachDTO map(EidentityDTO eidentity);
}
