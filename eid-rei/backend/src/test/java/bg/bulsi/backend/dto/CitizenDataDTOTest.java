package bg.bulsi.backend.dto;

import bg.bulsi.backend.BaseTest;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.reicontract.dto.CitizenDataDTO;
import jakarta.validation.ConstraintViolation;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.validation.beanvalidation.LocalValidatorFactoryBean;

import java.util.Set;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class CitizenDataDTOTest extends BaseTest {

    @Autowired
    private LocalValidatorFactoryBean validator;

    @Test
    public void validateCitizenIdentifierNumber() {
        //Arrange
        CitizenDataDTO dto = createCitizenDataDTO();

        // validate length
        dto.setCitizenIdentifierNumber("1111111111111");
        Set<ConstraintViolation<CitizenDataDTO>> violations = validator.validateProperty(dto, "citizenIdentifierNumber");
        assertTrue(violations.stream().allMatch(e -> e.getMessage().equals(ErrorCode.Fields.CITIZEN_IDENTIFIER_NUMBER_LENGTH_NOT_VALID)));

        // validate regex
        dto.setCitizenIdentifierNumber("testtesttest");
        violations = validator.validateProperty(dto, "citizenIdentifierNumber");
        assertTrue(violations.stream().anyMatch(e -> e.getMessage().equals(ErrorCode.Fields.CITIZEN_IDENTIFIER_NUMBER_NOT_VALID)));

        //validate not null
        dto.setCitizenIdentifierNumber(null);
        violations = validator.validateProperty(dto, "citizenIdentifierNumber");
        assertTrue(violations.stream().anyMatch(e -> e.getMessage().equals(ErrorCode.Fields.CITIZEN_IDENTIFIER_NUMBER_CANNOT_BE_NULL)));
    }

    @Test
    public void validateCitizenDataFirstName() {
        //Arrange
        CitizenDataDTO dto = createCitizenDataDTO();

        // validate length
        dto.setFirstName("");
        Set<ConstraintViolation<CitizenDataDTO>> violations = validator.validateProperty(dto, "firstName");
        assertTrue(violations.stream().anyMatch(e -> e.getMessage().equals(ErrorCode.Fields.FIRST_NAME_NOT_VALID)));

        //validate not null
        dto.setFirstName(null);
        violations = validator.validateProperty(dto, "firstName");
        assertTrue(violations.stream().anyMatch(e -> e.getMessage().equals(ErrorCode.Fields.FIRST_NAME_CANNOT_BE_BLANK)));
        
        //validate proper first name
        dto.setFirstName("Нга-бе");
        violations = validator.validateProperty(dto, "firstName");
        assertEquals(0, violations.size());
    }

    @Test
    public void validateCitizenDataSecondName() {
        //Arrange
        CitizenDataDTO dto = createCitizenDataDTO();

        // validate length
        dto.setSecondName("");
        Set<ConstraintViolation<CitizenDataDTO>> violations = validator.validateProperty(dto, "secondName");
        assertTrue(violations.stream().anyMatch(e -> e.getMessage().equals(ErrorCode.Fields.SECOND_NAME_NOT_VALID)));
        
        // validate proper second name
        dto.setSecondName("Нга-бе");
        violations = validator.validateProperty(dto, "secondName");
        assertEquals(0, violations.size());
    }

    @Test
    public void validateCitizenDataLastName() {
    	  //Arrange
        CitizenDataDTO dto = createCitizenDataDTO();

        // validate length
        dto.setLastName("");
        Set<ConstraintViolation<CitizenDataDTO>> violations = validator.validateProperty(dto, "lastName");
        assertTrue(violations.stream().anyMatch(e -> e.getMessage().equals(ErrorCode.Fields.LAST_NAME_NOT_VALID)));
        
        // validate proper second name
        dto.setLastName("Нга-бе");
        violations = validator.validateProperty(dto, "secondName");
        assertEquals(0, violations.size());
    }

    @Test
    public void validateCitizenDataCitizenIdentifierType() {
        //Arrange
        CitizenDataDTO dto = createCitizenDataDTO();

        //validate not null
        dto.setCitizenIdentifierType(null);
        Set<ConstraintViolation<CitizenDataDTO>> violations = validator.validateProperty(dto, "citizenIdentifierType");
        
        assertEquals(1, violations.size());
        assertTrue(violations.stream().allMatch(e -> e.getMessage().equals("must not be null")));
    }
}
