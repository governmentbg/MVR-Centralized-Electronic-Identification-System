package bg.bulsi.backend.service.impl;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.util.CitizenIdentifierNumberValidator;
import bg.bulsi.mvr.reicontract.dto.CitizenDataDTO;
import bg.bulsi.mvr.reicontract.dto.IdentifierTypeDTO;
import org.springframework.stereotype.Service;

import java.util.*;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;

@Service
public class ValidationService {

	public void validateCitizenDataDto(CitizenDataDTO citizenDataDTO) {
		 Set<String> validationErrors = new HashSet<>();

		 this.validateCitizenIdentifierAndType(citizenDataDTO.getCitizenIdentifierNumber(), citizenDataDTO.getCitizenIdentifierType(), validationErrors);
		 this.validateCitizenSecondAndLastName(citizenDataDTO.getSecondName(), citizenDataDTO.getLastName(), validationErrors);
		 
		 handleErrors(validationErrors);
	}
	
	private  void validateCitizenIdentifierAndType(String number, IdentifierTypeDTO type, Set<String> validationErrors) {
        // validate citizenIdentifierType
        if (Objects.isNull(type)) {
            addError(validationErrors, CITIZEN_IDENTIFIER_TYPE_CANNOT_BE_NULL);
        } else {
        	// validate citizenIdentifierNumber
            if (Objects.isNull(number)) {
                addError(validationErrors, CITIZEN_IDENTIFIER_NUMBER_CANNOT_BE_NULL);
            } else {
                if (!validateCitizenIdentifierNumber(number, type)) {
                    addError(validationErrors, CITIZEN_IDENTIFIER_NUMBER_NOT_VALID);
                }
            }
        }
    }

    protected void validateCitizenSecondAndLastName(String secondName, String lastName, Set<String> validationErrors) {
        if (secondName == null && lastName == null) {
        	addError(validationErrors, SECOND_OR_LAST_NAME_REQUIRED);
        }
    }
	
    private boolean validateCitizenIdentifierNumber(String number, IdentifierTypeDTO type) {
        return switch (type) {
            case EGN -> CitizenIdentifierNumberValidator.validateEGN(number);
            case LNCh ->  CitizenIdentifierNumberValidator.validateLNCH(number);
            default -> false;
        };
    }

	private void handleErrors(Set<String> validationErrors) {
        if (!validationErrors.isEmpty()) {
            throw new ValidationMVRException("Request is not valid", VALIDATION_ERROR, new HashSet<>(validationErrors));
        }
	}
    
    private void addError(Set<String> errors, ErrorCode error) {
        if (errors == null) {
            errors = new HashSet<>();
        }
        errors.add(error.name());
    }
}
