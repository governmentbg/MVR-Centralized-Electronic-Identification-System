package bg.bulsi.mvr.mpozei.backend.service;

import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.exception.BaseMVRException;
import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.mpozei.backend.BaseTest;
import bg.bulsi.mvr.mpozei.backend.Factory;
import bg.bulsi.mvr.mpozei.backend.client.*;
import bg.bulsi.mvr.mpozei.backend.dto.CertificateStatusDTO;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenCertificateSummaryDTO;
import bg.bulsi.mvr.mpozei.backend.rabbitmq.ListenerDispatcher;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.application.IssueEidApplication;
import bg.bulsi.mvr.mpozei.model.nomenclature.NomLanguage;
import bg.bulsi.mvr.mpozei.model.nomenclature.NomenclatureType;
import bg.bulsi.mvr.mpozei.model.nomenclature.ReasonNomenclature;
import bg.bulsi.mvr.mpozei.model.pivr.*;
import bg.bulsi.mvr.mpozei.model.repository.ApplicationRepository;
import bg.bulsi.mvr.mpozei.model.repository.HtmlHelpPageRepository;
import bg.bulsi.mvr.mpozei.model.repository.NomenclatureTypeRepository;
import bg.bulsi.mvr.mpozei.model.repository.ReasonsNomRepository;
import bg.bulsi.mvr.pdf_generator.PdfGenerator;
import jakarta.validation.ConstraintViolation;
import lombok.extern.slf4j.Slf4j;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.mockito.MockedStatic;
import org.mockito.Mockito;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.boot.test.mock.mockito.SpyBean;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageImpl;
import org.springframework.data.domain.PageRequest;
import org.springframework.transaction.annotation.Propagation;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.beanvalidation.LocalValidatorFactoryBean;

import java.time.OffsetDateTime;
import java.util.*;

import static bg.bulsi.mvr.mpozei.backend.Factory.*;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationType.ISSUE_EID;
import static bg.bulsi.mvr.mpozei.contract.dto.CertificateStatus.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.*;

@Slf4j
public class ListenerDispatcherTests extends BaseTest {
	@Autowired
	private ListenerDispatcher listenerDispatcher;

	@MockBean
	private ReiClient reiClient;

	@MockBean
	private RueiClient rueiClient;

	@MockBean
	private EjbcaClient ejbcaClient;

	@MockBean
	private PunClient punClient;

	@MockBean
	private EDeliveryClient eDeliveryClient;
	
	@Autowired
	private ApplicationRepository<AbstractApplication> applicationRepository;

	@MockBean
	private PdfGenerator pdfGenerator;

	@MockBean
	private HtmlHelpPageRepository htmlHelpPageRepository;
	
	@SpyBean
	private CertificateService certificateService;

	@MockBean
	private PivrClient pivrClient;

	@MockBean
	private RaeiceiService raeiceiService;

	@Autowired
	NomenclatureService nomenclatureService;

	@Autowired
	NomenclatureTypeRepository nomenclatureTypeRepository;

	@Autowired
	ReasonsNomRepository reasonsNomRepository;

	@Autowired
	private LocalValidatorFactoryBean validator;
	
	@Autowired
	private OpenSearchService openSearchService;

	@Test
	@Transactional
	public void createIssueEidApplicationFromDesk_Should_Succeed() {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromServletContext)
					.thenReturn(emptyUserContext);
			// Arrange & Act
			DeskApplicationRequest request = setupCreateApplication();

			// Act
			ApplicationResponse result = (ApplicationResponse) listenerDispatcher.createDeskApplication(request, null).getValue();

			// Assert
			Assertions.assertEquals(result.getStatus(), ApplicationStatus.SUBMITTED);
			Assertions.assertNotNull(result.getId());
		}
	}

	@Test
	@Transactional
	public void createIssueEid_Should_Throw_For_Null_Values() {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromServletContext)
			.thenReturn(emptyUserContext);
			// Arrange
			DeskApplicationRequest request = new DeskApplicationRequest();
			request.setApplicationType(ISSUE_EID);
			request.setCitizenship("BULGARIAN");

			when(reiClient.createEidentity(any())).thenReturn(UUID.randomUUID());
			when(rueiClient.attachCitizenProfile(any())).thenReturn(createCitizenProfileDTO(request));

			// Act & Assert
			Set<ConstraintViolation<DeskApplicationRequest>> violations = validator.validate(request);

			Assertions.assertEquals(
					"[ConstraintViolationImpl{interpolatedMessage='CITIZEN_IDENTIFIER_NUMBER_CANNOT_BE_NULL', propertyPath=citizenIdentifierNumber, rootBeanClass=class bg.bulsi.mvr.mpozei.contract.dto.DeskApplicationRequest, messageTemplate='CITIZEN_IDENTIFIER_NUMBER_CANNOT_BE_NULL'}, ConstraintViolationImpl{interpolatedMessage='CITIZEN_IDENTIFIER_TYPE_CANNOT_BE_NULL', propertyPath=citizenIdentifierType, rootBeanClass=class bg.bulsi.mvr.mpozei.contract.dto.DeskApplicationRequest, messageTemplate='CITIZEN_IDENTIFIER_TYPE_CANNOT_BE_NULL'}, ConstraintViolationImpl{interpolatedMessage='DEVICE_ID_CANNOT_BE_NULL', propertyPath=deviceId, rootBeanClass=class bg.bulsi.mvr.mpozei.contract.dto.DeskApplicationRequest, messageTemplate='DEVICE_ID_CANNOT_BE_NULL'}, ConstraintViolationImpl{interpolatedMessage='FIRST_NAME_CANNOT_BE_BLANK', propertyPath=firstName, rootBeanClass=class bg.bulsi.mvr.mpozei.contract.dto.DeskApplicationRequest, messageTemplate='FIRST_NAME_CANNOT_BE_BLANK'}, ConstraintViolationImpl{interpolatedMessage='FIRST_NAME_LATIN_CANNOT_BE_BLANK', propertyPath=firstNameLatin, rootBeanClass=class bg.bulsi.mvr.mpozei.contract.dto.DeskApplicationRequest, messageTemplate='FIRST_NAME_LATIN_CANNOT_BE_BLANK'}, ConstraintViolationImpl{interpolatedMessage='PERSONAL_IDENTITY_DOCUMENT_CANNOT_BE_NULL', propertyPath=personalIdentityDocument, rootBeanClass=class bg.bulsi.mvr.mpozei.contract.dto.DeskApplicationRequest, messageTemplate='PERSONAL_IDENTITY_DOCUMENT_CANNOT_BE_NULL'}]",
					Arrays.stream(violations.toArray()).sorted(Comparator.comparing(Object::toString)).toList().toString()
			);
		}
	}

	@Test
	public void createIssueEid_Should_Throw_For_Invalid_Values() {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromServletContext)
			.thenReturn(emptyUserContext);
			// Arrange
			DeskApplicationRequest request = createInvalidApplicationRequest();
			Mockito.when(raeiceiService.getDeviceById(any())).thenReturn(Factory.createDeviceDTO());
			Mockito.when(raeiceiService.getDeviceByType(any())).thenReturn(List.of(Factory.createDeviceDTO()));
			when(reiClient.createEidentity(any())).thenReturn(UUID.randomUUID());
			when(rueiClient.attachCitizenProfile(any())).thenReturn(createCitizenProfileDTO(request));
			when(raeiceiService.getEidAdministratorById((any()))).thenReturn(createEidAdministrator());
			when(raeiceiService.getOfficeById(((any())))).thenReturn(createEidAdministratorFrontOffice());
			// Act & Assert
			BaseMVRException thrown = Assertions.assertThrows(BaseMVRException.class, () -> {
				listenerDispatcher.createDeskApplication(request, null);
			});

			Assertions.assertEquals("Request is not valid", thrown.getMessage());

			Set<ConstraintViolation<DeskApplicationRequest>> violations = validator.validate(request);
			Assertions.assertEquals(
					"[ConstraintViolationImpl{interpolatedMessage='FIRST_NAME_LATIN_CANNOT_BE_BLANK', propertyPath=firstNameLatin, rootBeanClass=class bg.bulsi.mvr.mpozei.contract.dto.DeskApplicationRequest, messageTemplate='FIRST_NAME_LATIN_CANNOT_BE_BLANK'}, ConstraintViolationImpl{interpolatedMessage='FIRST_NAME_NOT_VALID', propertyPath=firstName, rootBeanClass=class bg.bulsi.mvr.mpozei.contract.dto.DeskApplicationRequest, messageTemplate='FIRST_NAME_NOT_VALID'}, ConstraintViolationImpl{interpolatedMessage='LAST_NAME_NOT_VALID', propertyPath=lastName, rootBeanClass=class bg.bulsi.mvr.mpozei.contract.dto.DeskApplicationRequest, messageTemplate='LAST_NAME_NOT_VALID'}, ConstraintViolationImpl{interpolatedMessage='PERSONAL_IDENTITY_DOCUMENT_CANNOT_BE_NULL', propertyPath=personalIdentityDocument, rootBeanClass=class bg.bulsi.mvr.mpozei.contract.dto.DeskApplicationRequest, messageTemplate='PERSONAL_IDENTITY_DOCUMENT_CANNOT_BE_NULL'}, ConstraintViolationImpl{interpolatedMessage='SECOND_NAME_NOT_VALID', propertyPath=secondName, rootBeanClass=class bg.bulsi.mvr.mpozei.contract.dto.DeskApplicationRequest, messageTemplate='SECOND_NAME_NOT_VALID'}]",
					Arrays.stream(violations.toArray()).sorted(Comparator.comparing(Object::toString)).toList().toString()
			);
		}
	}

//	@Test
//    @Transactional
//	public void signIssueEidApplication_Should_Succeed() {
//		// Arrange
//		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
//			userContextHolder.when(UserContextHolder::getFromServletContext)
//					.thenReturn(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(),"af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc", true,null));
//
//			DeskApplicationRequest request = setupCreateApplication();
//			ApplicationResponse result = (ApplicationResponse) listenerDispatcher.createDeskApplication(request).getValue();
//
//			// Export Application
//			when(rueiClient.getCertificateById(any())).thenReturn(Factory.createCitizenCertificateSummaryDTO(CREATED));
//			when(punClient.getPunCarriersByEidentityId(any())).thenReturn((List.of(Factory.createPunCarrier())));
//			listenerDispatcher.exportApplication(result.getId());
//
//			ApplicationSignRequest signRequest = new ApplicationSignRequest();
//			signRequest.setEmail("ivan@gmail.com");
//			signRequest.setPhoneNumber("0899345678");
//
//			Map<String, Object> payload = new HashMap<>();
//			payload.put("applicationId", result.getId());
//			payload.put("applicationSignRequest", signRequest);
//
//			// Act
//			ApplicationStatus status = (ApplicationStatus) listenerDispatcher.signApplication(payload).getValue();
//
//			// Assert
//			Assertions.assertEquals(status, ApplicationStatus.SIGNED);
//		}
//	}

//	@Test
//    @Transactional
//	public void signIssueEidApplication_Should_Throw_For_Null_Values() {
//		// Arrange
//		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
//			userContextHolder.when(UserContextHolder::getFromServletContext)
//					.thenReturn(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(),"af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc", true,""));
//			DeskApplicationRequest request = createApplicationRequest();
//			request.setApplicationType(ISSUE_EID);
////			request.setEmail(null);
//
//			when(reiClient.createEidentity(any())).thenReturn(request.getEidentityId());
//			when(rueiClient.attachCitizenProfile(any())).thenReturn(createCitizenProfileDTO(request));
//			when(reiClient.findEidentityByNumberAndType(any(), any())).thenReturn(createEidentityDTO());
//			when(rueiClient.getCitizenProfileByEidentityId(any())).thenReturn(createCitizenProfileDTO(request));
//
//			when(pivrClient.getDateOfDeath(any(), any())).thenReturn(new CitizenDateOfDeath(null));
//			when(pivrClient.getDateOfProhibition(any(), any())).thenReturn(new CitizenProhibition(null, ProhibitionType.NONE, null));
//			when(pivrClient.getPersonalIdentityV2(any(), any()))
//					.thenReturn(Factory.createRegiXResultWithPersonalIdentity());
//
//			ApplicationResponse result = (ApplicationResponse) listenerDispatcher.createDeskApplication(request).getValue();
//
//			// Export Application
//			when(rueiClient.getCertificateById(any())).thenReturn(Factory.createCitizenCertificateSummaryDTO(CREATED));
//			when(punClient.getPunCarriersByEidentityId(any())).thenReturn((List.of(Factory.createPunCarrier())));
//			listenerDispatcher.exportApplication(result.getId());
//
//			ApplicationSignRequest signRequest = new ApplicationSignRequest();
//
//			Map<String, Object> payload = new HashMap<>();
//			payload.put("applicationId", result.getId());
//			payload.put("applicationSignRequest", signRequest);
//
//			// Act & Assert
//			BaseMVRException thrown = Assertions.assertThrows(BaseMVRException.class, () -> {
//				listenerDispatcher.signApplication(payload);
//			});
//
//			Assertions.assertEquals("Request is not valid", thrown.getMessage());
//			Assertions.assertEquals("{email=[Email cannot be null]}", thrown.getAdditionalProperties().toString());
//		}
//	}

	@Test
    @Transactional
	public void signIssueEidApplication_Should_Throw_For_Invalid_Values() {
		// Arrange
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromServletContext)
			.thenReturn(emptyUserContext);

			DeskApplicationRequest request = setupCreateApplication();

			ApplicationResponse result = (ApplicationResponse) listenerDispatcher.createDeskApplication(request, null).getValue();

			// Export Application
			when(rueiClient.getCertificateById(any())).thenReturn(Factory.createCitizenCertificateSummaryDTO(CREATED));
			when(punClient.getPunCarriersByEidentityId(any())).thenReturn((List.of(Factory.createPunCarrier())));
			listenerDispatcher.exportApplication(result.getId(), null);

			ApplicationSignRequest signRequest = new ApplicationSignRequest();
			signRequest.setEmail("111111111111111111111111111111111111");
			signRequest.setPhoneNumber("");

			Map<String, Object> payload = new HashMap<>();
			payload.put("applicationId", result.getId());
			payload.put("applicationSignRequest", signRequest);

			// Act & Assert
			BaseMVRException thrown = Assertions.assertThrows(BaseMVRException.class, () -> {
				listenerDispatcher.signApplication(payload, null);
			});

			Assertions.assertEquals("Request is not valid", thrown.getMessage());
			Assertions.assertEquals("[EMAIL_NOT_VALID]",
					thrown.getAdditionalProperties().toString());
		}
	}

	@Test
	@Transactional(propagation = Propagation.REQUIRES_NEW)
	public void createStopEidApplicationFromDesk_Should_Succeed() {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromServletContext)
			.thenReturn(emptyUserContext);
			// Arrange & Act
			DeskApplicationRequest request = setupStopEidFromDeskApplication();

			// Act
			ApplicationResponse result = (ApplicationResponse) listenerDispatcher.createDeskApplication(request, null).getValue();

			// Assert
			Assertions.assertEquals(ApplicationStatus.APPROVED, result.getStatus());
			Assertions.assertNotNull(result.getId());
		}
	}

//	@Test
//    @Transactional
//	public void signStopEidApplicationFromDesk_Should_Succeed() {
//		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
//			userContextHolder.when(UserContextHolder::getFromServletContext)
//					.thenReturn(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(),"af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc", true,""));
//
//			// Create Stop Eid
//			DeskApplicationRequest request = setupStopEidFromDeskApplication();
//			ApplicationResponse result = (ApplicationResponse) listenerDispatcher.createDeskApplication(request).getValue();
//
//			// Export Application
//			when(rueiClient.getCertificateById(any())).thenReturn(Factory.createCitizenCertificateSummaryDTO(ACTIVE));
//			when(punClient.getPunCarrierByEidentityIdAndCertificateId(any(), any())).thenReturn((List.of(Factory.createPunCarrier())));
//			listenerDispatcher.exportApplication(result.getId());
//
//			// Sign Stop Eid
//			ApplicationStatus status = (ApplicationStatus) listenerDispatcher
//					.signApplication(createSignStopFromDeskApplication(result.getId())).getValue();
//
////            AbstractApplication application = applicationRepository.getReferenceById(result.getId());
//
//			verify(panFeignClient, times(1)).sendNotification(any(), any());
//			// TODO: the line below throws database exception check the reason
//			// List<CertificateHistory> certificateHistoryList =
//			// certificateService.getCertificateHistoryByCertificateId(request.getCertificateId());
//			verify(certificateService, times(1)).createCertificateHistory(any());
//
//			// TODO: is this the expected ApplicationStatus
//			Assertions.assertEquals(ApplicationStatus.COMPLETED, status);
//		}
//	}

//	@Test
//	@Transactional
//	public void signStopEidApplicationFromDeskWithGuardians_Should_Succeed() {
//		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
//			userContextHolder.when(UserContextHolder::getFromServletContext)
//					.thenReturn(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(),"af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc", true,""));
//
//			// Create Stop Eid
//			DeskApplicationRequest request = setupStopEidFromDeskApplication();
//
//			RegiXResult regiXResult = createRegiXResultWithPersonalIdentity();
//			PersonalIdentityInfoResponseType personalIdentityInfoResponseType = ((PersonalIdentityInfoResponseType) regiXResult.getResponse().get("PersonalIdentityInfoResponse"));
//			personalIdentityInfoResponseType.setBirthDate(CURRENT_OFFSET_DATE_TIME.minusYears(15));
//
//			when(pivrClient.getPersonalIdentityV2(any(), any()))
//			.thenReturn(regiXResult);
//
//			ApplicationResponse result = (ApplicationResponse) listenerDispatcher.createDeskApplication(request).getValue();
//
//			// Export Application
//			when(rueiClient.getCertificateById(any())).thenReturn(Factory.createCitizenCertificateSummaryDTO(ACTIVE));
//			when(punClient.getPunCarrierByEidentityIdAndCertificateId(any(), any())).thenReturn((List.of(Factory.createPunCarrier())));
//			listenerDispatcher.exportApplication(result.getId());
//
//			// Sign Stop Eid
//			ApplicationStatus status = (ApplicationStatus) listenerDispatcher
//					.signApplication(createSignStopFromDeskApplication(result.getId())).getValue();
//
////            AbstractApplication application = applicationRepository.getReferenceById(result.getId());
//
//			verify(panFeignClient, times(1)).sendNotification(any(), any());
//			// TODO: the line below throws database exception check the reason
//			// List<CertificateHistory> certificateHistoryList =
//			// certificateService.getCertificateHistoryByCertificateId(request.getCertificateId());
//			verify(certificateService, times(1)).createCertificateHistory(any());
//
//			// TODO: is this the expected ApplicationStatus
//			Assertions.assertEquals(ApplicationStatus.COMPLETED, status);
//		}
//	}

	@Test
    @Transactional
	public void createResumeEidApplicationFromDesk_Should_Succeed() {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromServletContext)
			.thenReturn(emptyUserContext);
			// Arrange & Act
			DeskApplicationRequest request = setupResumeEidFromDeskApplication();

			// Act
			ApplicationResponse result = (ApplicationResponse) listenerDispatcher.createDeskApplication(request, null).getValue();

			// Assert
			Assertions.assertEquals(ApplicationStatus.APPROVED, result.getStatus());
			Assertions.assertNotNull(result.getId());
		}
	}
	
//	@Test
//    @Transactional
//	public void signResumeEidApplicationFromDesk_Should_Succeed() {
//		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
//			userContextHolder.when(UserContextHolder::getFromServletContext)
//					.thenReturn(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(),"af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc", true,""));
//
//			// Create Resume Eid
//			DeskApplicationRequest request = setupResumeEidFromDeskApplication();
//			ApplicationResponse result = (ApplicationResponse) listenerDispatcher.createDeskApplication(request).getValue();
//
//			// Export Application
//			when(rueiClient.getCertificateById(any())).thenReturn(Factory.createCitizenCertificateSummaryDTO(STOPPED));
//			when(punClient.getPunCarrierByEidentityIdAndCertificateId(any(), any())).thenReturn((List.of(Factory.createPunCarrier())));
//			listenerDispatcher.exportApplication(result.getId());
//
//			// Sign Resume Eid
//			ApplicationStatus status = (ApplicationStatus) listenerDispatcher
//					.signApplication(createSignResumeFromDeskApplication(result.getId())).getValue();
//
////            AbstractApplication application = applicationRepository.getReferenceById(result.getId());
//
//			verify(panFeignClient, times(1)).sendNotification(any(), any());
//			// TODO: the line below throws database exception check the reason
//			// List<CertificateHistory> certificateHistoryList =
//			// certificateService.getCertificateHistoryByCertificateId(request.getCertificateId());
//			verify(certificateService, times(1)).createCertificateHistory(any());
//
//			Assertions.assertEquals(ApplicationStatus.COMPLETED, status);
//		}
//	}
	
//	@Test
//	@Transactional
//	public void signResumeEidApplicationFromDeskWithGuardians_Should_Succeed() {
//		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
//			userContextHolder.when(UserContextHolder::getFromServletContext)
//					.thenReturn(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(),"af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc", true,""));
//
//			// Create Resume Eid
//			DeskApplicationRequest request = setupResumeEidFromDeskApplication();
//
//			RegiXResult regiXResult = createRegiXResultWithPersonalIdentity();
//			PersonalIdentityInfoResponseType personalIdentityInfoResponseType = ((PersonalIdentityInfoResponseType) regiXResult.getResponse().get("PersonalIdentityInfoResponse"));
//			personalIdentityInfoResponseType.setBirthDate(CURRENT_OFFSET_DATE_TIME.minusYears(15));
//
//			when(pivrClient.getPersonalIdentityV2(any(), any()))
//			.thenReturn(regiXResult);
//
//			ApplicationResponse result = (ApplicationResponse) listenerDispatcher.createDeskApplication(request).getValue();
//
//			// Export Application
//			when(rueiClient.getCertificateById(any())).thenReturn(Factory.createCitizenCertificateSummaryDTO(STOPPED));
//			when(punClient.getPunCarrierByEidentityIdAndCertificateId(any(), any())).thenReturn((List.of(Factory.createPunCarrier())));
//			listenerDispatcher.exportApplication(result.getId());
//
//			// Sign Resume Eid
//			ApplicationStatus status = (ApplicationStatus) listenerDispatcher
//					.signApplication(createSignResumeFromDeskApplication(result.getId())).getValue();
//
//			verify(panFeignClient, times(1)).sendNotification(any(), any());
//			// TODO: the line below throws database exception check the reason
//			// List<CertificateHistory> certificateHistoryList =
//			// certificateService.getCertificateHistoryByCertificateId(request.getCertificateId());
//			verify(certificateService, times(1)).createCertificateHistory(any());
//
//			Assertions.assertEquals(ApplicationStatus.COMPLETED, status);
//		}
//	}
	
	// TODO: 11/8/2023 add tests for creating certificate

	@Test
	public void findCitizenCertificates_Should_Succeed() {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromServletContext)
			.thenReturn(emptyUserContext);

			// Arrange
			CitizenCertificateFilter filter = new CitizenCertificateFilter();
			filter.setEidentityId(UUID.randomUUID());
			FindCertificateResponse certificate = new FindCertificateResponse();
			certificate.setSerialNumber("123");
			certificate.setId(UUID.fromString("952c8d8f-260f-4ec4-a156-793e64efbfdf"));
//			when(punClient.getPunCarriersByEidentityId(any())).thenReturn((List.of(Factory.createPunCarrier())));
			when(raeiceiService.getDeviceById(any())).thenReturn(createDeviceDTO());
			when(rueiClient.findCitizenCertificates(any(), any(), any(), any(), any(), any(), any(), any(), any(), anyBoolean(), any()))
					.thenReturn(new PageImpl<>(List.of(certificate)));

			// Act
			listenerDispatcher.findCitizenCertificates(filter);

			// Assert
			verify(rueiClient, times(1)).findCitizenCertificates(any(), any(), any(), any(), any(), any(), any(), any(), any(), anyBoolean(), any());
//			verify(punClient, times(1)).getPunCarriersByEidentityId(any());
		}
	}

	@Test
	public void findCitizenCertificates_WithSetExpiringSet_Should_Succeed() {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromServletContext)
			.thenReturn(emptyUserContext);

			// Arrange
			CitizenCertificateFilter filter = new CitizenCertificateFilter();
			filter.setEidentityId(UUID.randomUUID());
			FindCertificateResponse certificate1 = new FindCertificateResponse();
			certificate1.setSerialNumber("123");
			certificate1.setId(UUID.fromString("952c8d8f-260f-4ec4-a156-793e64efbfdf"));
			certificate1.setValidityUntil(OffsetDateTime.now().minusDays(25));
			
			FindCertificateResponse certificate2 = new FindCertificateResponse();
			certificate2.setSerialNumber("1234");
			certificate2.setId(UUID.fromString("152c8d8f-260f-4ec4-a156-793e64efbfdf"));
			certificate2.setValidityUntil(OffsetDateTime.now().minusDays(55));
			
			FindCertificateResponse certificate3 = new FindCertificateResponse();
			certificate3.setSerialNumber("1235");
			certificate3.setId(UUID.fromString("352c8d8f-260f-4ec4-a156-793e64efbfdf"));
			certificate3.setValidityUntil(OffsetDateTime.now().plusDays(10));
			
			FindCertificateResponse certificate4 = new FindCertificateResponse();
			certificate4.setSerialNumber("1236");
			certificate4.setId(UUID.fromString("452c8d8f-260f-4ec4-a156-793e64efbfdf"));
			certificate4.setValidityUntil(OffsetDateTime.now().plusDays(100));
			
			FindCertificateResponse certificate5 = new FindCertificateResponse();
			certificate5.setSerialNumber("1237");
			certificate5.setId(UUID.fromString("552c8d8f-260f-4ec4-a156-793e64efbfdf"));
			certificate5.setValidityUntil(OffsetDateTime.now());
			
//			when(punClient.getPunCarriersByEidentityId(any())).thenReturn((List.of(Factory.createPunCarrier())));
			when(raeiceiService.getDeviceById(any())).thenReturn(createDeviceDTO());
			when(rueiClient.findCitizenCertificates(any(), any(), any(), any(), any(), any(), any(), any(), any(), anyBoolean(), any()))
					.thenReturn(new PageImpl<>(List.of(certificate1, certificate2, certificate3, certificate4, certificate5)));

			// Act
			Page<FindCertificateResponse> result = (Page<FindCertificateResponse>) listenerDispatcher.findCitizenCertificates(filter).getValue();

			// Assert
			verify(rueiClient, times(1)).findCitizenCertificates(any(), any(), any(), any(), any(), any(), any(), any(), any(), anyBoolean(), any());
			//Expiring
			Assertions.assertEquals(3, result.get().filter(c -> c.getIsExpiring()).toList().size());
			//Not Expiring
			Assertions.assertEquals(2, result.get().filter(c -> !c.getIsExpiring()).toList().size());

//			verify(punClient, times(1)).getPunCarriersByEidentityId(any());
		}
	}
	
	@Test
	@Transactional
	public void updateApplicationStatus_Should_Succeed() {
		// Arrange
		DeskApplicationRequest request = setupCreateApplication();
		AbstractApplication application = new AbstractApplication();
//		application.setId(UUID.randomUUID());
		application.setStatus(ApplicationStatus.GENERATED_CERTIFICATE);
		applicationRepository.save(application);

		Map<String, String> payload = new HashMap<>();
		ApplicationStatus status = ApplicationStatus.COMPLETED;
		payload.put("id", application.getId().toString());
		payload.put("status", status.name());

		// Act
		ApplicationStatus response = (ApplicationStatus) listenerDispatcher.updateApplicationStatus(payload, null).getValue();

		Assertions.assertSame(response, status);
	}

//	@Test
//    @Transactional
//	void findEidentityByNumberAndType_Should_Succeed() {
//		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
//			userContextHolder.when(UserContextHolder::getFromServletContext)
//					.thenReturn(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(),"af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc", true,""));
//
//			// Arrange
//			Map<String, String> payload = new HashMap<>();
//			payload.put("citizenIdentifierNumber", "0751114397");
//			payload.put("citizenIdentifierType", IdentifierType.EGN.name());
//
//			when(reiClient.findEidentityByNumberAndType(any(), any())).thenReturn(new EidentityDTO());
//			when(rueiClient.getCitizenProfileByEidentityId(any())).thenReturn(new CitizenProfileDTO());
//
//			// Act
//			EidentityResponse response = (EidentityResponse) listenerDispatcher.findEidentityByNumberAndType(payload)
//					.getValue();
//
//			// Assert
//			Assertions.assertNotNull(response);
//		}
//	}

	@Test
    @Transactional
	void getCertificateById_Should_Succeed() {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromServletContext)
			.thenReturn(emptyUserContext);

			CitizenCertificateSummaryDTO certificate = new CitizenCertificateSummaryDTO();
			certificate.setEidAdministratorId(UUID.fromString("194a90a0-3b9d-47f5-865a-ad8bcf2c3acc"));

			when(rueiClient.getCertificateById(any())).thenReturn(certificate);
			when(punClient.getPunCarrierByEidentityIdAndCertificateId(any(), any())).thenReturn(new ArrayList<>());
			when(raeiceiService.getEidAdministratorById((any()))).thenReturn(createEidAdministrator());
			when(raeiceiService.getOfficeById(((any())))).thenReturn(createEidAdministratorFrontOffice());

			CitizenCertificateSummaryResponse response = (CitizenCertificateSummaryResponse) listenerDispatcher
					.getCertificateById(UUID.randomUUID()).getValue();

			// Assert
			Assertions.assertNotNull(response);
		}
	}

	@Test
	@Transactional
	void getApplicationById_Should_Succeed() {
		// Arrange
		AbstractApplication expected = new AbstractApplication();
		when(raeiceiService.getEidAdministratorById((any()))).thenReturn(createEidAdministrator());
		when(raeiceiService.getOfficeById(((any())))).thenReturn(createEidAdministratorFrontOffice());
		Mockito.when(raeiceiService.getDeviceById(any())).thenReturn(Factory.createDeviceDTO());
		applicationRepository.save(expected);
		// Act
		ApplicationDetailsResponse result = (ApplicationDetailsResponse) listenerDispatcher
				.getApplicationById(expected.getId(), null).getValue();

		// Assert
		Assertions.assertEquals(expected.getId(), result.getId());
	}

	@Test
	void getApplicationById_Should_Throw_IfNotFound() {
		AbstractApplication expected = new AbstractApplication();
		expected.setId(UUID.randomUUID());
		// Arrange

		// Act
		// Assert
		EntityNotFoundException thrown = Assertions.assertThrows(EntityNotFoundException.class, () -> {
			listenerDispatcher.getApplicationById(UUID.randomUUID(), null);
		});
		Assertions.assertEquals(thrown.getStatus(), 404);
	}

//    @Test
//    @Transactional
//    void exportApplication_Should_Succeed() {
//        try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
//            // Arrange
//            userContextHolder.when(UserContextHolder::getFromServletContext)
//                    .thenReturn(new UserContext("", "", "", "", "", "", "", List.of(""), UUID.randomUUID()));
//        // Arrange
//        AbstractApplication application = new AbstractApplication();
//        application.addParam(APPLICATION_SUBMISSION_TYPE, ApplicationSubmissionType.DESK);
//        application.setApplicationType(ApplicationType.STOP_EID);
//        application.setPipelineStatus(PipelineStatus.RUEI_BASE_PROFILE_VERIFICATION);
//        applicationRepository.save(application);
//        byte[] expected = HexFormat.of().parseHex("1A3F");
//        when(pdfGenerator.generatePdf(any(), any()))
//                .thenReturn(expected);
//
//        // Act
//        byte[] result = (byte[]) listenerDispatcher.exportApplication(application.getId()).getValue();
//
//        // Assert
//        Assertions.assertEquals(new String(expected), new String(result));
//        }
//    }

//	@Test
//    @Transactional
//	void denyApplication_Should_Succeed() {
//		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
//			// Arrange
//			userContextHolder.when(UserContextHolder::getFromServletContext)
//					.thenReturn(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(),"af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc", true,""));
//			DeskApplicationRequest request = setupCreateApplication();
//			ApplicationResponse result = (ApplicationResponse) listenerDispatcher.createDeskApplication(request).getValue();
//			DenyApplicationDTO dto = new DenyApplicationDTO(result.getId(), UUID.fromString("36325da5-54a4-42e8-9bd8-b04e69c65bf0"), null);
//			// Act
//			ApplicationStatus status = (ApplicationStatus) listenerDispatcher.denyApplication(dto).getValue();
//			// Assert
//			Assertions.assertEquals(status, ApplicationStatus.DENIED);
//		}
//	}

	@Test
	void findApplications_Should_Succeed() {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			// Arrange
			userContextHolder.when(UserContextHolder::getFromServletContext)
			.thenReturn(emptyUserContext);
			when(raeiceiService.getEidAdministratorById((any()))).thenReturn(createEidAdministrator());
			when(raeiceiService.getOfficeById(((any())))).thenReturn(createEidAdministratorFrontOffice());
			ApplicationFilter filter = new ApplicationFilter();
			filter.setPageable(PageRequest.of(0, 1));

			// Act Assert
			Page<ApplicationDTO> applications = (Page<ApplicationDTO>) listenerDispatcher.findApplications(filter)
					.getValue();
		}
	}

	@Transactional
	protected AbstractApplication saveNewApplication() {
		AbstractApplication application = new IssueEidApplication();
		application.setApplicationType(ISSUE_EID);
		application.setCitizenIdentifierNumber("123");
		return applicationRepository.save(application);
	}

	@Test
	@Transactional
	void getAllReasons_Should_Succeed() {
		// Arrange
//		NomenclatureType type = new NomenclatureType();
//		type.setName("TEST");
//		nomenclatureService.createNomenclatureType(type);
//
//		ReasonNomenclature reasonNomenclature = new ReasonNomenclature();
//		reasonNomenclature.setNomCode(type);
//		reasonNomenclature.setLanguage(NomLanguage.BG);
//		reasonNomenclature.setName("Random_name");
//		nomenclatureService.createNomenclature(reasonNomenclature);
//
//		// Act
		List<NomenclatureTypeDTO> nomonclaturesTypes = (List<NomenclatureTypeDTO>) listenerDispatcher.getAllReasons().getValue();

		// Assert
		Assertions.assertTrue(
                nomonclaturesTypes
                .stream()
                .filter(nt -> nt.getId().equals(UUID.fromString("227438ca-19cc-4dce-8fe0-a2baeffb6f4e")))
                .findFirst()
                .map(nt -> nt.getNomenclatures())
                .get()
                .stream()
                .anyMatch(n -> n.getId().equals(UUID.fromString("5cc4daef-835c-4d0c-a842-a7cfe4c8cc87"))));
	}

	private DeskApplicationRequest setupCreateApplication() {
		DeskApplicationRequest request = createApplicationRequest();
		request.setApplicationType(ISSUE_EID);

		when(reiClient.createEidentity(any())).thenReturn(UUID.randomUUID());
		when(rueiClient.attachCitizenProfile(any())).thenReturn(createCitizenProfileDTO(request));
		when(reiClient.findEidentityByNumberAndType(any(), any())).thenReturn(createEidentityDTO());
		when(rueiClient.getCitizenProfileByEidentityId(any())).thenReturn(createCitizenProfileDTO(request));
		when(raeiceiService.getEidAdministratorById((any()))).thenReturn(createEidAdministrator());
		when(raeiceiService.getOfficeById(((any())))).thenReturn(createEidAdministratorFrontOffice());
		when(pivrClient.getDateOfDeath(any(), any())).thenReturn(new CitizenDateOfDeath(null));

		when(pivrClient.getDateOfProhibition(any(), any())).thenReturn(new CitizenProhibition(null, ProhibitionType.NONE, null));

		when(pivrClient.getPersonalIdentityV2(any(), any()))
				.thenReturn(Factory.createRegiXResultWithPersonalIdentity());

		when(rueiClient.getCertificateById(any())).thenReturn(Factory.createCitizenCertificateSummaryDTO(CREATED));
		Mockito.when(raeiceiService.getDeviceById(any())).thenReturn(Factory.createDeviceDTO());
        Mockito.when(raeiceiService.getDeviceByType(any())).thenReturn(List.of(Factory.createDeviceDTO()));
		return request;
	}

	private Map<String, Object> createSignStopFromDeskApplication(UUID applicationId) {
		ApplicationSignRequest signRequest = new ApplicationSignRequest();
		signRequest.setPhoneNumber("+359111111111");
		signRequest.setEmail("1111111@a11bv.bg");
		signRequest.setGuardians(Factory.createGuardianDetails());

		Map<String, Object> payload = new HashMap<>();
		payload.put("applicationId", applicationId);
		payload.put("applicationSignRequest", signRequest);

		when(ejbcaClient.searchCertificates(any())).thenReturn(Factory.searchCertificates());

		when(rueiClient.getCertificateById(any())).thenReturn(Factory.createCitizenCertificateSummaryDTO(ACTIVE));

		when(rueiClient.validateCitizenCertificate(any())).thenReturn(CertificateStatusDTO.ACTIVE);

		return payload;
	}

	private Map<String, Object> createSignResumeFromDeskApplication(UUID applicationId) {
		ApplicationSignRequest signRequest = new ApplicationSignRequest();
		signRequest.setPhoneNumber("+359111111111");
		signRequest.setEmail("1111111@a11bv.bg");
		signRequest.setGuardians(Factory.createGuardianDetails());

		Map<String, Object> payload = new HashMap<>();
		payload.put("applicationId", applicationId);
		payload.put("applicationSignRequest", signRequest);

		when(ejbcaClient.searchCertificates(any())).thenReturn(Factory.searchCertificates());

		when(rueiClient.getCertificateById(any())).thenReturn(Factory.createCitizenCertificateSummaryDTO(STOPPED));

		when(rueiClient.validateCitizenCertificate(any())).thenReturn(CertificateStatusDTO.ACTIVE);

		return payload;
	}
	
	private DeskApplicationRequest setupResumeEidFromDeskApplication() {
		DeskApplicationRequest request = new DeskApplicationRequest();
		request.setFirstName("Фатима");
		request.setSecondName("Мохамедова");
		request.setLastName("Фатимова");
		request.setFirstNameLatin("Fatima");
		request.setSecondNameLatin("Mohamed");
		request.setLastNameLatin("Fatimova");
		request.setCitizenIdentifierNumber("0551178590");
		request.setCitizenship("BULGARIAN");
		request.setCitizenIdentifierType(IdentifierType.EGN);
		request.setDeviceId(UUID.fromString("bc9f97f8-b004-4b61-ac85-7d1a7cb05f14"));
//		request.setPhoneNumber("+359111111111");
//		request.setEmail("1111111@a11bv.bg");
		request.setApplicationType(ApplicationType.RESUME_EID);
		request.setReasonId(UUID.fromString("a380962f-ed2f-4c3a-a36d-f56892aed708"));
		request.setCertificateId(CERTIFICATE_ID);
		request.setCitizenship("BULGARIAN");

		PersonalIdentityDocument identityDocument = Factory.createPersonalIdentityDocument();
		request.setPersonalIdentityDocument(identityDocument);

		when(pivrClient.getDateOfDeath(any(), any())).thenReturn(new CitizenDateOfDeath(null));

		when(pivrClient.getDateOfProhibition(any(), any())).thenReturn(new CitizenProhibition(null, ProhibitionType.NONE, null));

		when(pivrClient.getPersonalIdentityV2(any(), any()))
				.thenReturn(Factory.createRegiXResultWithPersonalIdentity());

		when(reiClient.findEidentityByNumberAndType(any(), any())).thenReturn(Factory.createEidentityDTO());

		when(rueiClient.getCitizenProfileByEidentityId(any())).thenReturn(Factory.createCitizenProfileDTO(request));

		when(rueiClient.getCertificateById(any())).thenReturn(Factory.createCitizenCertificateSummaryDTO(STOPPED));
		when(raeiceiService.getEidAdministratorById((any()))).thenReturn(createEidAdministrator());
		when(raeiceiService.getOfficeById(((any())))).thenReturn(createEidAdministratorFrontOffice());
		Mockito.when(raeiceiService.getDeviceById(any())).thenReturn(Factory.createDeviceDTO());
        Mockito.when(raeiceiService.getDeviceByType(any())).thenReturn(List.of(Factory.createDeviceDTO()));
		return request;
	}
	
	private DeskApplicationRequest setupStopEidFromDeskApplication() {
		DeskApplicationRequest request = new DeskApplicationRequest();
		request.setFirstName("Фатима");
		request.setSecondName("Мохамедова");
		request.setLastName("Фатимова");
		request.setFirstNameLatin("Fatima");
		request.setSecondNameLatin("Mohamed");
		request.setLastNameLatin("Fatimova");
		request.setCitizenIdentifierNumber("0551178590");
		request.setCitizenship("BULGARIAN");
		request.setCitizenIdentifierType(IdentifierType.EGN);
		request.setDeviceId(UUID.fromString("bc9f97f8-b004-4b61-ac85-7d1a7cb05f14"));
//		request.setPhoneNumber("+359111111111");
//		request.setEmail("1111111@a11bv.bg");
		request.setApplicationType(ApplicationType.STOP_EID);
		request.setReasonId(UUID.fromString("5cc4daef-835c-4d0c-a842-a7cfe4c8cc87"));
		request.setCertificateId(CERTIFICATE_ID);
		request.setCitizenship("BULGARIAN");

		PersonalIdentityDocument identityDocument = Factory.createPersonalIdentityDocument();
		request.setPersonalIdentityDocument(identityDocument);

		when(pivrClient.getDateOfDeath(any(), any())).thenReturn(new CitizenDateOfDeath(null));

		when(pivrClient.getDateOfProhibition(any(), any())).thenReturn(new CitizenProhibition(null, ProhibitionType.NONE, null));

		when(pivrClient.getPersonalIdentityV2(any(), any()))
				.thenReturn(Factory.createRegiXResultWithPersonalIdentity());

		when(reiClient.findEidentityByNumberAndType(any(), any())).thenReturn(Factory.createEidentityDTO());

		when(rueiClient.getCitizenProfileByEidentityId(any())).thenReturn(Factory.createCitizenProfileDTO(request));

		when(rueiClient.getCertificateById(any())).thenReturn(Factory.createCitizenCertificateSummaryDTO(ACTIVE));
		when(raeiceiService.getEidAdministratorById((any()))).thenReturn(createEidAdministrator());
		when(raeiceiService.getOfficeById(((any())))).thenReturn(createEidAdministratorFrontOffice());
		Mockito.when(raeiceiService.getDeviceById(any())).thenReturn(Factory.createDeviceDTO());
        Mockito.when(raeiceiService.getDeviceByType(any())).thenReturn(List.of(Factory.createDeviceDTO()));
		return request;
	}
}