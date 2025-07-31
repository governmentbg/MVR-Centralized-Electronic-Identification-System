package bg.bulsi.mvr.mpozei.backend.service;

import bg.bulsi.mvr.common.exception.BaseMVRException;
import bg.bulsi.mvr.mpozei.backend.BaseTest;
import bg.bulsi.mvr.mpozei.backend.Factory;
import bg.bulsi.mvr.mpozei.backend.service.impl.ValidationService;
import bg.bulsi.mvr.mpozei.contract.dto.GuardianDetails;
import bg.bulsi.mvr.mpozei.contract.dto.IdentifierType;
import bg.bulsi.mvr.mpozei.contract.dto.PersonalIdentityDocument;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.application.ResumeEidApplication;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.mockito.Mockito;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.mock.mockito.MockBean;

import java.time.LocalDate;
import java.util.Arrays;

import static org.mockito.ArgumentMatchers.any;

public class ValidationServiceTests extends BaseTest{

	@Autowired
	private ValidationService validationService;

	@MockBean
	private RaeiceiService raeiceiService;
	
	@Test
	public void validateSignResumeEidFromDeskApplication_ShouldThrow_ForMissingValues() {
		AbstractApplication application = new ResumeEidApplication();
		application.getParams().setRequireGuardians(true);
		Mockito.when(raeiceiService.getDeviceById(any())).thenReturn(Factory.createDeviceDTO());

		BaseMVRException thrown = Assertions.assertThrows(BaseMVRException.class, () -> {
			this.validationService.validateSignResumeEidFromDeskApplication(application);
		});
		Assertions.assertEquals("Request is not valid", thrown.getMessage());
		Assertions.assertEquals(
				"[GUARDIAN_DETAILS_CANNOT_BE_NULL]",
				thrown.getAdditionalProperties().toString());
	}
	
	@Test
	public void validateSignResumeEidFromDeskApplication_ShouldThrow_ForMissingGuardianValues() {
		AbstractApplication application = new ResumeEidApplication();
		application.getTemporaryData().setGuardians(Arrays.asList(new GuardianDetails(null, null, null, null, null, null, null)));
		application.getParams().setRequireGuardians(true);
		Mockito.when(raeiceiService.getDeviceById(any())).thenReturn(Factory.createDeviceDTO());
		BaseMVRException thrown = Assertions.assertThrows(BaseMVRException.class, () -> {
			this.validationService.validateSignResumeEidFromDeskApplication(application);
		});

		Assertions.assertEquals("Request is not valid", thrown.getMessage());
		Assertions.assertEquals(
				"[GUARDIAN_CITIZEN_IDENTIFIER_NUMBER_CANNOT_BE_BLANK, GUARDIAN_LAST_NAME_CANNOT_BE_BLANK, GUARDIAN_SECOND_NAME_CANNOT_BE_BLANK, GUARDIAN_FIRST_NAME_CANNOT_BE_BLANK, GUARDIAN_CITIZEN_IDENTIFIER_TYPE_CANNOT_BE_NULL, GUARDIAN_IDENTITY_DOCUMENT_CANNOT_BE_NULL]",
				thrown.getAdditionalProperties().toString());
	}
	
	@Test
	public void validateSignResumeEidFromDeskApplicationWithGuardians_ShouldSucceed() {
		AbstractApplication application = new ResumeEidApplication();
		PersonalIdentityDocument document = PersonalIdentityDocument.builder()
				.identityIssueDate(LocalDate.now())
				.identityValidityToDate(LocalDate.now())
				.identityNumber("not_empty")
				.identityType("not_empty")
				.build();

		application.getTemporaryData().setGuardians(Arrays.asList(new GuardianDetails("not_empty", "not_empty", "not_empty", "not_empty", IdentifierType.EGN, "not_empty", document)));
		application.getParams().setRequireGuardians(true);
		application.getParams().setEmail("test@gmail.com");
		Mockito.when(raeiceiService.getDeviceById(any())).thenReturn(Factory.createDeviceDTO());
		this.validationService.validateSignResumeEidFromDeskApplication(application);
	}
	
	@Test
	public void validateSignResumeEidFromDeskApplicationWithoutGuardians_ShouldSucceed() {
		AbstractApplication application = new ResumeEidApplication();
		PersonalIdentityDocument document = PersonalIdentityDocument.builder()
				.identityIssueDate(LocalDate.now())
				.identityValidityToDate(LocalDate.now())
				.identityNumber("not_empty")
				.identityType("not_empty")
				.build();

		application.getTemporaryData().setGuardians(Arrays.asList(new GuardianDetails("not_empty", "not_empty", "not_empty", "not_empty", IdentifierType.EGN, "not_empty", document)));
		application.getParams().setEmail("test@gmail.com");
		Mockito.when(raeiceiService.getDeviceById(any())).thenReturn(Factory.createDeviceDTO());
		this.validationService.validateSignResumeEidFromDeskApplication(application);
	}
}
