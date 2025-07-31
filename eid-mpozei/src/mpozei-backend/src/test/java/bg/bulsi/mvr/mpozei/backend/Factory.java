package bg.bulsi.mvr.mpozei.backend;

import bg.bulsi.mvr.mpozei.backend.dto.*;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaSearchCertificateDTO;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaSearchCertificateResponse;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.pivr.PersonalIdentityInfoResponseType;
import bg.bulsi.mvr.mpozei.model.pivr.RegiXResult;
import bg.bulsi.mvr.mpozei.model.pivr.common.Nationality;
import bg.bulsi.mvr.mpozei.model.pivr.common.PersonNames;
import bg.bulsi.mvr.mpozei.model.pivr.common.ReturnInformation;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.OffsetDateTime;
import java.util.*;

import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceType;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeDTO;
import bg.bulsi.mvr.raeicei.model.entity.Device;

import org.springframework.security.oauth2.client.registration.ClientRegistration;
import org.springframework.security.oauth2.core.AuthorizationGrantType;

import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationType.ISSUE_EID;

public class Factory {

	public static final String CERTIFICATE = "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";
	
	public static final UUID CERTIFICATE_ID = UUID.randomUUID();
	
	public static final LocalDate CURRENT_LOCAL_DATE = LocalDate.now();
	
	public static EidentityDTO createEidentityDTO() {
		EidentityDTO dto = new EidentityDTO();
		dto.setFirstName("Фатима");
		dto.setSecondName("Мохамедова");
		dto.setLastName("Фатимова");
		dto.setCitizenIdentifierNumber("0551178590");
		dto.setCitizenIdentifierType(IdentifierType.EGN);
		dto.setId(UUID.fromString("484462cb-55cb-4dd6-92de-61057cc642b4"));
		return dto;
	}
	
	public static RegiXResult createRegiXResultWithPersonalIdentity() {
		PersonalIdentityInfoResponseType responseType = new PersonalIdentityInfoResponseType();
		responseType.setBirthDate(LocalDate.now().minusYears(20));
		PersonNames personNames = new PersonNames();
		personNames.setFirstNameLatin("Fatima");
		personNames.setSurnameLatin("Mohamed");
		personNames.setLastNameLatin("Fatimova");
		responseType.setPersonNames(personNames);
		
		Nationality nationality = new Nationality();
		nationality.setNationalityName("Българка");
		nationality.setNationalityCode("БЛГ");
		nationality.setNationalityNameLatin("Bulgarian");
		responseType.setNationalityList(List.of(nationality));
		
		//populate PersonalIdDocument
		responseType.setIssueDate(CURRENT_LOCAL_DATE);
		responseType.setValidDate(CURRENT_LOCAL_DATE);
		responseType.setIssuerName("МВР");
		responseType.setIssuerNameLatin("MVR");
		responseType.setIdentityDocumentNumber("12312323123");
		responseType.setDocumentType("Паспорт");
		responseType.setDocumentTypeLatin("Password");
		
		ReturnInformation returnInformation = new ReturnInformation();
		returnInformation.setReturnCode("0000");
		responseType.setReturnInformations(returnInformation);
		
		Map<String, Object> response = new HashMap<>();
		response.put("PersonalIdentityInfoResponse", responseType);
		
		RegiXResult regiXResult = new RegiXResult();
		regiXResult.setError(null);
		regiXResult.setHasFailed(false);
		regiXResult.setResponse(response);
		
		return regiXResult; 
	}
	
    public static DeskApplicationRequest createApplicationRequest() {
        DeskApplicationRequest request = new DeskApplicationRequest();
        request.setFirstName("Фатима");
        request.setSecondName("Мохамедова");
        request.setLastName("Фатимова");
        request.setFirstNameLatin("Fatima");
        request.setSecondNameLatin("Mohamed");
        request.setLastNameLatin("Fatimova");
        request.setCitizenIdentifierNumber("0551178590");
        request.setCitizenIdentifierType(IdentifierType.EGN);
        request.setCitizenship("BULGARIAN");
        request.setDeviceId(UUID.fromString("bc9f97f8-b004-4b61-ac85-7d1a7cb05f14"));
//        request.setPhoneNumber("+359111111111");
//        request.setEmail("1111111@a11bv.bg");
        request.setPersonalIdentityDocument(createPersonalIdentityDocument());
        
        return request;
    }

    public static DeskApplicationRequest createInvalidApplicationRequest() {
        DeskApplicationRequest request = new DeskApplicationRequest();
        request.setApplicationType(ISSUE_EID);
        request.setFirstName("11111111111111111111111111111111111111111111111111111111111111");
        request.setSecondName("11111111111111111111111111111111111111111111111111111111111111");
        request.setLastName("11111111111111111111111111111111111111111111111111111111111111");
//        request.setPhoneNumber("11111111111111111111111111111111111111111111111111111111111111");
//        request.setEmail("11111111111111111111111111111111111111111111111111111111111111");
        request.setCitizenship("BULGARIAN");
        request.setCitizenIdentifierType(IdentifierType.EGN);
        request.setCitizenIdentifierNumber("11111111111111111111111111111111111111111111111111111111111111");
        request.setDeviceId(UUID.fromString("bc9f97f8-b004-4b61-ac85-7d1a7cb05f14"));
        return request;
    }

    public static CitizenProfileDTO createCitizenProfileDTO(DeskApplicationRequest request) {
        CitizenProfileDTO dto = new CitizenProfileDTO();
        dto.setId(UUID.randomUUID());
//        dto.setEmail(request.getEmail());
//        dto.setPhoneNumber(request.getPhoneNumber());
        dto.setLastName(request.getLastName());
        dto.setSecondName(request.getSecondName());
        dto.setFirstName(request.getFirstName());
        dto.setId(UUID.fromString("484462cb-55cb-4dd6-92de-61057cc642b4"));
        return dto;
    }
    
    public static EjbcaSearchCertificateResponse searchCertificates() {
        EjbcaSearchCertificateDTO dto = new EjbcaSearchCertificateDTO();
        dto.setCertificate(CERTIFICATE);
        dto.setCertificateChain(List.of(CERTIFICATE));
        dto.setCertificateProfile("mock-profile");
        dto.setEndEntityProfile("mock-entity-profile");
        EjbcaSearchCertificateResponse response = new EjbcaSearchCertificateResponse();
        response.setCertificates(List.of(dto));
        return response;
    }
    
    public static CitizenCertificateSummaryDTO createCitizenCertificateSummaryDTO(CertificateStatus status) {
    	CitizenCertificateSummaryDTO dto = new CitizenCertificateSummaryDTO();
        dto.setIssuerDN("CN=RapidSSL TLS RSA CA G1,OU=www.digicert.com,O=DigiCert Inc,C=US");
        dto.setId(CERTIFICATE_ID);
        dto.setStatus(status);
        dto.setSerialNumber("123123");
        return dto;
    }
    
    public static AbstractApplication createAbstractApplication() {
        AbstractApplication application = new AbstractApplication();
        application.setId(UUID.randomUUID());
        application.setStatus(ApplicationStatus.SUBMITTED);
        application.setApplicationType(ApplicationType.ISSUE_EID);
//        application.setApplicationNumber(new ApplicationNumber());
        application.setCitizenIdentifierType(IdentifierType.EGN);
        application.setCitizenIdentifierNumber("0551178590");
        application.setFirstName("Иван");
        application.setSecondName("Георгиев");
        application.setLastName("Крумов");
        application.setDeviceId(UUID.fromString("bc9f97f8-b004-4b61-ac85-7d1a7cb05f14"));
        application.setSubmissionType(ApplicationSubmissionType.DESK);
        return application;
    }
    
    public static AbstractApplication createAbstractApplication2() {
        AbstractApplication application = new AbstractApplication();
        application.setId(UUID.randomUUID());
        application.setStatus(ApplicationStatus.SUBMITTED);
        application.setApplicationType(ApplicationType.ISSUE_EID);
//        application.setApplicationNumber(new ApplicationNumber());
        application.setCitizenIdentifierType(IdentifierType.EGN);
        application.setCitizenIdentifierNumber("0551178590");
        application.setCitizenship("Българка");
        application.setFirstName("Фатима");
        application.setSecondName("Мохамедова");
        application.setLastName("Фатимова");
        application.setFirstNameLatin("Fatima");
        application.setSecondNameLatin("Mohamed");
        application.setLastNameLatin("Fatimova");
        application.setDeviceId(UUID.fromString("bc9f97f8-b004-4b61-ac85-7d1a7cb05f14"));
        return application;
    }
    
    public static PunCarrierDTO createPunCarrier() {
		PunCarrierDTO carrier = new PunCarrierDTO();
		carrier.setSerialNumber("123");
		carrier.setDeviceType(DeviceType.CHIP_CARD.name());
//		carrier.setPunDeviceId(UUID.fromString("bc9f97f8-b004-4b61-ac85-7d1a7cb05f14"));
		carrier.setCertificateId(UUID.fromString("952c8d8f-260f-4ec4-a156-793e64efbfdf"));
		return carrier;
    }
    
//    public static Map<String, Object> createApplicationExport(UUID applicationId) {
//    	ApplicationExport applicationExport = new ApplicationExport();
//        applicationExport.setPersonalIdIssueDate(LocalDate.now());
//        applicationExport.setPersonalIdValidityToDate(LocalDate.now());
//        applicationExport.setPersonalIdIssuer("MVR");
//        applicationExport.setPersonalIdNumber("12312312321312");
//        applicationExport.setPersonalIdType("Passport");
//    	
//        Map<String, Object> payload = new HashMap<>();
//        payload.put("applicationId", applicationId);
//        payload.put("applicationExport", applicationExport);
//        
//        return payload;
//    }
    
    public static List<GuardianDetails> createGuardianDetails() {
    	List<GuardianDetails> guardianDetailsList = new ArrayList<>();
    	
    	GuardianDetails guardianDetails1 = new GuardianDetails();
    	guardianDetails1.setFirstName("Влад");
    	guardianDetails1.setSecondName("Петър");
    	guardianDetails1.setLastName("Чехов");
    	guardianDetails1.setCitizenIdentifierNumber("9231231321");
    	guardianDetails1.setCitizenIdentifierType(IdentifierType.EGN);
    	
		guardianDetails1.setPersonalIdentityDocument(createPersonalIdentityDocument());
		
    	guardianDetailsList.add(guardianDetails1);
    	
    	return guardianDetailsList;
    }
    
    public static PersonalIdentityDocument createPersonalIdentityDocument() {
    	PersonalIdentityDocument personalIdentityDocument = new PersonalIdentityDocument();
    	personalIdentityDocument.setIdentityIssueDate(CURRENT_LOCAL_DATE);
    	personalIdentityDocument.setIdentityValidityToDate(CURRENT_LOCAL_DATE);
		personalIdentityDocument.setIdentityNumber("12312323123");
		personalIdentityDocument.setIdentityType("Паспорт");
		
		return personalIdentityDocument;
    }
    
    public static ClientRegistration clientRegistration() {
    	return  ClientRegistration.withRegistrationId("test")
    			.clientId("test")
    			.clientName("test")
    			.clientSecret("test")
    			.authorizationGrantType(AuthorizationGrantType.CLIENT_CREDENTIALS)
    			.tokenUri("asasfasf")
    			.build()
    			;
    }

    public static EidAdministratorDTO createEidAdministrator() {
        EidAdministratorDTO eidAdministrator = new EidAdministratorDTO();
        eidAdministrator.setId(UUID.fromString("194a90a0-3b9d-47f5-865a-ad8bcf2c3acc"));
        eidAdministrator.setName("МВР");
        eidAdministrator.setNameLatin("MVR");
        eidAdministrator.setEidManagerFrontOfficeIds(List.of(UUID.fromString("af7c1fe6-d669-414e-b066-e9733f0de7a8")));
        return eidAdministrator;
    }

    public static EidManagerFrontOfficeDTO createEidAdministratorFrontOffice() {
    	EidManagerFrontOfficeDTO office = new EidManagerFrontOfficeDTO();
        office.setId(UUID.fromString("af7c1fe6-d669-414e-b066-e9733f0de7a8"));
        office.setName("MVR");
        return office;
    }

    public static DeviceDTO createDeviceDTO() {
        DeviceDTO dto = new DeviceDTO();
        dto.setId(UUID.randomUUID());
        dto.setType(DeviceType.CHIP_CARD);
        return dto;
    }
}
