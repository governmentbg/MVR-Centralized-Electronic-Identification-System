package bg.bulsi.backend.service;

import bg.bulsi.backend.BaseTest;
import bg.bulsi.backend.service.impl.CitizenIdentifierServiceImpl;
import bg.bulsi.mvr.common.exception.BaseMVRException;
import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.exception.WrongStatusException;
import bg.bulsi.mvr.reicontract.dto.CitizenDataDTO;
import bg.bulsi.reimodel.repository.view.CitizenIdentifierView;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.MethodOrderer;
import org.junit.jupiter.api.Order;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.TestMethodOrder;
import org.springframework.beans.factory.annotation.Autowired;

import java.util.UUID;

@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
public class CitizenIdentifierServiceTest extends BaseTest {
    @Autowired
    private CitizenIdentifierServiceImpl citizenIdentifierService;

    @Order(1)
    @Test
    public void createCitizenIdentifier_Should_Succeed() {
        // Arrange
        CitizenDataDTO dto = createCitizenDataDTO();
        // Act
        CitizenIdentifierView ci = citizenIdentifierService.create(dto);
        // Assert
        Assertions.assertEquals(ci.getFirstName(), dto.getFirstName());
        Assertions.assertEquals(ci.getSecondName(), dto.getSecondName());
        Assertions.assertEquals(ci.getNumber(), dto.getCitizenIdentifierNumber());
        Assertions.assertEquals(ci.getLastName(), dto.getLastName());
        Assertions.assertEquals(ci.getActive(), true);
        Assertions.assertEquals(ci.getType().name(), dto.getCitizenIdentifierType().name());
    }

    @Test
    public void createCitizenIdentifier_Should_Return_AlreadyExisting() {
        // Arrange
        CitizenDataDTO dto = createCitizenDataDTO();
        citizenIdentifierService.create(dto);
        CitizenDataDTO dto2 = createCitizenDataDTO();
        dto2.setCitizenIdentifierNumber(dto.getCitizenIdentifierNumber());
        // Act
        CitizenIdentifierView ci = citizenIdentifierService.create(dto2);
        // Assert
        Assertions.assertEquals(ci.getFirstName(), dto.getFirstName());
        Assertions.assertEquals(ci.getSecondName(), dto.getSecondName());
        Assertions.assertEquals(ci.getNumber(), dto.getCitizenIdentifierNumber());
        Assertions.assertEquals(ci.getLastName(), dto.getLastName());
        Assertions.assertEquals(ci.getActive(), true);
        Assertions.assertEquals(ci.getType().name(), dto.getCitizenIdentifierType().name());
    }

    @Test
    public void createCitizenIdentifier_MissingSecondAndLastName_Should_Throw() {
        // Arrange
        CitizenDataDTO dto = createCitizenDataDTO();
        dto.setSecondName(null);
        dto.setLastName(null);
        
        // Act & Assert
        ValidationMVRException validationMVRException = Assertions.assertThrows(ValidationMVRException.class,
                () -> citizenIdentifierService.create(dto));
        Assertions.assertTrue(validationMVRException.getAdditionalProperties().contains(ErrorCode.Fields.SECOND_OR_LAST_NAME_REQUIRED));
    }
    
    @Test
    public void updateCitizenIdentifier_Should_Succeed() {
        // Arrange
        CitizenIdentifierView initial = citizenIdentifierService.create(createCitizenDataDTO());
        CitizenDataDTO dto = createCitizenDataDTO();
        dto.setCitizenIdentifierNumber(initial.getNumber());
        // Act
        CitizenIdentifierView ci = citizenIdentifierService.update(initial.getEidentityId(), dto);
        // Assert
        Assertions.assertEquals(ci.getFirstName(), dto.getFirstName());
        Assertions.assertEquals(ci.getSecondName(), dto.getSecondName());
        Assertions.assertEquals(ci.getLastName(), dto.getLastName());
        Assertions.assertEquals(ci.getNumber(), dto.getCitizenIdentifierNumber());
        Assertions.assertEquals(ci.getActive(), true);
        Assertions.assertEquals(ci.getType().name(), dto.getCitizenIdentifierType().name());
    }

    @Test
    public void updateCitizenIdentifier_Should_Throw_NotFound() {
        // Arrange
        CitizenDataDTO dto = createCitizenDataDTO();
        // Act Assert
        Assertions.assertThrows(EntityNotFoundException.class,
                () -> citizenIdentifierService.update(UUID.randomUUID(), dto));
    }


    @Test
    public void updateCitizenIdentifierActive_Should_Throw_NotFound() {
        // Arrange Act Assert
        Assertions.assertThrows(EntityNotFoundException.class,
                () -> citizenIdentifierService.updateActiveByEidentityId(UUID.randomUUID(), false));
    }

    @Test
    public void updateCitizenIdentifierActive_Should_Succeed() {
        // Arrange
        CitizenIdentifierView initial = citizenIdentifierService.create(createCitizenDataDTO());
        // Act Assert
        citizenIdentifierService.updateActiveByEidentityId(initial.getEidentityId(), false);
    }
    
    @Test
    public void updateCitizenIdentifier_MissingSecondAndLastName_Should_Throw() {
        // Arrange
        CitizenDataDTO dto = createCitizenDataDTO();
        dto.setSecondName(null);
        dto.setLastName(null);
        
        // Act & Assert
        ValidationMVRException validationMVRException = Assertions.assertThrows(ValidationMVRException.class,
                () -> citizenIdentifierService.update(UUID.randomUUID(), dto));
        Assertions.assertTrue(validationMVRException.getAdditionalProperties().contains(ErrorCode.Fields.SECOND_OR_LAST_NAME_REQUIRED));
    }
}
