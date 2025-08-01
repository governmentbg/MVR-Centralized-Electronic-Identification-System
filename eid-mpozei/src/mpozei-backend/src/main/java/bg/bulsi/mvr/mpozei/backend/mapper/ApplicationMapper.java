package bg.bulsi.mvr.mpozei.backend.mapper;

import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.crypto.CertificateProcessor;
import bg.bulsi.mvr.common.crypto.CryptoTimestampProcessor;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.FaultMVRException;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.service.FileFormatService;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.backend.client.MisepClient;
import bg.bulsi.mvr.mpozei.backend.client.RaeiceiClient;
import bg.bulsi.mvr.mpozei.backend.dto.*;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.*;
import bg.bulsi.mvr.mpozei.backend.dto.misep.MisepClientResponse;
import bg.bulsi.mvr.mpozei.backend.dto.misep.MisepPaymentRequest;
import bg.bulsi.mvr.mpozei.backend.dto.mock.MockCitizenCertificateDTO;
import bg.bulsi.mvr.mpozei.backend.dto.mock.MockCitizenCertificateValidateDTO;
import bg.bulsi.mvr.mpozei.backend.service.RaeiceiService;
import bg.bulsi.mvr.mpozei.backend.service.impl.SsevNotificationServiceImpl;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.contract.dto.edelivery.EDeliveryMessageRequest;
import bg.bulsi.mvr.mpozei.contract.dto.edelivery.EDeliveryProfileRequest;
import bg.bulsi.mvr.mpozei.contract.dto.xml.EidApplicationXml;
import bg.bulsi.mvr.mpozei.contract.dto.xml.GuardianDetailsXml;
import bg.bulsi.mvr.mpozei.model.application.*;
import bg.bulsi.mvr.mpozei.model.id_generator.id.ApplicationNumber;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceType;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.extern.slf4j.Slf4j;
import org.apache.commons.lang3.StringUtils;
import org.bouncycastle.asn1.x500.X500Name;
import org.bouncycastle.asn1.x500.style.BCStyle;
import org.bouncycastle.cms.CMSException;
import org.bouncycastle.cms.CMSSignedData;
import org.bouncycastle.pkcs.PKCS10CertificationRequest;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.MappingTarget;
import org.mapstruct.NullValueCheckStrategy;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Component;

import java.io.IOException;
import java.security.NoSuchProviderException;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import java.text.ParseException;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.time.format.DateTimeFormatter;
import java.util.*;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.stream.Collectors;

import static bg.bulsi.mvr.common.exception.ErrorCode.DEVICE_TYPE_NOT_RECOGNIZED;
import static bg.bulsi.mvr.common.exception.ErrorCode.PERSONAL_DETAILS_ARE_INCORRECT;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.*;

@Slf4j
@Component
@Mapper(componentModel = "spring",
        nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS,
        imports = {IdentifierTypeDTO.class})
public abstract class ApplicationMapper {

	public static final String X509_CERT_SUBJ_SN_PREFIX = "PI:BG-";
	
	public static final DateTimeFormatter SIGNATURE_INFO_DATE_FORMATTER = DateTimeFormatter.ofPattern("dd.MM.yyyy");
	
    @Value("${ejbca.certificate-profile-name}")
    protected String ejbcaCertificateProfileName;

    @Value("${ejbca.end-entity-profile-name}")
    protected String ejbcaEndEntityProfileName;

    @Value("${ejbca.account-binding-id}")
    protected String ejbcaAccountBindingId;

    @Autowired
    protected ObjectMapper objectMapper;
    
    @Value("${certificate-creation.dev}")
    protected boolean mockCertificateCreation;
    
    @Autowired
    protected FileFormatService fileFormatService;

    @Autowired
    protected RaeiceiService raeiceiService;
    
    @Autowired
    protected CertificateProcessor certificateProcessor;

    @Autowired
    protected CryptoTimestampProcessor cryptoTimestampProcessor;
    
    @Autowired
    protected RaeiceiClient raeiceiClient;

    @Autowired
    protected MisepClient misepClient;

    public abstract List<GuardianDetailsXml> mapToGuardianDetailsXml(List<GuardianDetails> guardianDetails);
    
    public abstract CitizenDataDTO mapToCitizenDataDTO(AbstractApplication application);

    public abstract CitizenProfileAttachDTO mapToCitizenProfileAttachDTO(AbstractApplication application);

    public CitizenCertificateValidateDTO mapToCitizenCertificateValidateDTO(AbstractApplication application) {
        if (application == null) {
            return null;
        }
        CitizenCertificateValidateDTO dto = new CitizenCertificateValidateDTO();
        dto.setCertificate(application.getParams().getClientCertificate());
        return dto;
    }

    public MockCitizenCertificateValidateDTO mapToMockCitizenCertificateValidateDTO(AbstractApplication application) {
        if (application == null) {
            return null;
        }
        MockCitizenCertificateValidateDTO dto = new MockCitizenCertificateValidateDTO();
        dto.setCertificate(application.getParams().getClientCertificate());
        dto.setMockCertificateId(application.getParams().getCertificateId().toString());
        return dto;
    }

    public CitizenCertificateUpdateDTO mapToCitizenCertificateUpdateDTO(AbstractApplication application, CertificateStatusDTO status) {
        if (application == null
        		|| application.getParams().getClientCertificate() == null
        		|| application.getParams().getClientCertificate().isBlank()) {
            return null;
        }

		X509Certificate parsedCertificate;
		try {
			parsedCertificate = certificateProcessor
					.extractCertificate(application.getParams().getClientCertificate().getBytes());
		} catch (CertificateException | NoSuchProviderException e) {
			log.info("Cannot parse input to X509 Certificate");
			log.error(e.toString());

			throw new FaultMVRException("Certificate could not be extracted", ErrorCode.INTERNAL_SERVER_ERROR);
		}
		
        CitizenCertificateUpdateDTO dto = new CitizenCertificateUpdateDTO();
        dto.setEidentityId(application.getEidentityId());
        dto.setStatus(status);
        dto.setIssuerDN(parsedCertificate.getIssuerX500Principal().getName());
        dto.setSerialNumber(parsedCertificate.getSerialNumber().toString());
        dto.setLastModifiedApplicationId(application.getId());
        return dto;
    }

    public CitizenCertificateDTO mapToCitizenCertificateDTO(AbstractApplication application) {
        if (application == null) {
            return null;
        }
        CitizenCertificateDTO dto = new CitizenCertificateDTO();
        dto.setEidentityId(application.getEidentityId());
        dto.setCertificate(application.getParams().getClientCertificate());
        dto.setCertificateCA(application.getParams().getClientCertificateChain());
        dto.setLastModifiedApplicationId(application.getId());
        dto.setEidAdministratorId(application.getEidAdministratorId());
        dto.setEidAdministratorOfficeId(application.getAdministratorFrontOfficeId());
        dto.setLevelOfAssurance(application.getParams().getLevelOfAssurance());
        DeviceDTO deviceType = raeiceiService.getDeviceById(application.getDeviceId());
        dto.setDeviceId(deviceType.getId());
        return dto;
    }

    public MockCitizenCertificateDTO mapToMockCitizenCertificateDTO(AbstractApplication application) {
        if (application == null) {
            return null;
        }
        MockCitizenCertificateDTO dto = new MockCitizenCertificateDTO();
        dto.setEidentityId(application.getEidentityId());
        dto.setCertificate(application.getParams().getClientCertificate());
        dto.setCertificateCA(application.getParams().getClientCertificateChain());
        dto.setLastModifiedApplicationId(application.getId());
        dto.setEidAdministratorId(application.getEidAdministratorId());
        dto.setEidAdministratorOfficeId(application.getAdministratorFrontOfficeId());
        dto.setLevelOfAssurance(application.getParams().getLevelOfAssurance());
        dto.setMockSerialNumber(application.getParams().getCertificateSerialNumber());
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        dto.setDeviceId(device.getId());
        return dto;
    }

    public ApplicationResponse mapToApplicationResponse(AbstractApplication application) {
        if (application == null) {
            return null;
        }
        ApplicationResponse response = new ApplicationResponse();
        response.setId(application.getId());
        response.setStatus(application.getStatus());
        response.setApplicationNumber(application.getApplicationNumber().getId());
        EidAdministratorDTO administrator = raeiceiService.getEidAdministratorById(application.getEidAdministratorId());
        response.setEidAdministratorName(administrator.getName());
        response.setFee(application.getParams().getFee());
        response.setFeeCurrency(application.getParams().getFeeCurrency());
        response.setSecondaryFee(application.getParams().getSecondaryFee());
        response.setSecondaryFeeCurrency(application.getParams().getSecondaryFeeCurrency());
        return response;
    }

    public OnlineApplicationResponse mapToOnlineApplicationResponse(AbstractApplication application) {
        if (application == null) {
            return null;
        }
        OnlineApplicationResponse response = new OnlineApplicationResponse();
        response.setId(application.getId());
        response.setStatus(application.getStatus());
        EidAdministratorDTO administrator = raeiceiService.getEidAdministratorById(application.getEidAdministratorId());
		response.setEidAdministratorName(administrator.getName());
        response.setFee(application.getParams().getFee());
        response.setFeeCurrency(application.getParams().getFeeCurrency());
        response.setSecondaryFee(application.getParams().getSecondaryFee());
        response.setSecondaryFeeCurrency(application.getParams().getSecondaryFeeCurrency());
        response.setPaymentAccessCode(application.getParams().getPaymentAccessCode());
        boolean isPaymentRequired = application.getParams().getFee() != 0 && MVRConstants.MVR_ADMINISTRATOR_ID.equals(application.getEidAdministratorId());
        response.setIsPaymentRequired(isPaymentRequired);
        
        return response;
    }

    public AbstractApplication mapToPersoCentreApplication(PersoCentreApplicationRequest request) {
        IssueEidApplication application = new IssueEidApplication();
        application.setFirstName(request.getFirstName());
        application.setSecondName(request.getSecondName());
        application.setLastName(request.getLastName());
        application.setFirstNameLatin(request.getFirstNameLatin());
        application.setSecondNameLatin(request.getSecondNameLatin());
        application.setLastNameLatin(request.getLastNameLatin());
        application.setCitizenship(request.getCitizenship());
        application.setCitizenIdentifierNumber(request.getCitizenIdentifierNumber());
        application.setCitizenIdentifierType(switch (request.getPersonalIdentifierType()) {
        case  EGN-> IdentifierType.EGN;
        case LNCh -> IdentifierType.LNCh;
        //TODO
        default -> IdentifierType.EGN; //throw new ValidationMVRException(PERSONAL_DETAILS_ARE_INCORRECT);
    });
        
		/*
		 * application.setCitizenIdentifierType(switch
		 * (request.getPersonalIdentityDocument().getDocumentType()) { case "ID" ->
		 * IdentifierType.EGN; case "IX" -> IdentifierType.LNCh; case "IT" ->
		 * IdentifierType.FP; default -> throw new
		 * ValidationMVRException(PERSONAL_DETAILS_ARE_INCORRECT); });
		 */
        application.getTemporaryData().setPersonalIdentityDocument(map(request.getPersonalIdentityDocument()));
        application.setApplicationType(request.getApplicationType());
        application.setCitizenIdentifierNumber(request.getCitizenIdentifierNumber());
        application.getParams().setNumForm(request.getNumForm());
        return application;
    }

    @Mapping(source = "documentNumber", target = "identityNumber")
    @Mapping(source = "documentType", target = "identityType")
    @Mapping(source = "documentIssueDate", target = "identityIssueDate")
    @Mapping(source = "documentValidityToDate", target = "identityValidityToDate")
    public abstract PersonalIdentityDocument map(PersonalIdentityDocumentV2 document);

    public AbstractApplication mapToApplication(BaseApplicationRequest request) {
        return switch(request.getApplicationType()) {
            case ISSUE_EID -> mapToIssueEidApplication(request);
            case RESUME_EID -> mapToResumeEidApplication(request);
            case REVOKE_EID -> mapToRevokeEidApplication(request);
            case STOP_EID -> mapToStopEidApplication(request);
        };
    }

    public void mapAbstractApplication(AbstractApplication application, BaseApplicationRequest request) {
        if (request == null || application == null) {
            return;
        }

        application.setFirstName(request.getFirstName());
        application.setSecondName(request.getSecondName());
        application.setLastName(request.getLastName());
        application.setFirstNameLatin(request.getFirstNameLatin());
        application.setSecondNameLatin(request.getSecondNameLatin());
        application.setLastNameLatin(request.getLastNameLatin());
        application.setCitizenship(request.getCitizenship());
        application.setCitizenIdentifierNumber(request.getCitizenIdentifierNumber());
        application.setCitizenIdentifierType(request.getCitizenIdentifierType());
        application.getTemporaryData().setPersonalIdentityDocument(request.getPersonalIdentityDocument());
        application.setApplicationType(request.getApplicationType());
//        application.setSignedDetails(request.getSignedDetails());
    }

    public IssueEidApplication mapToIssueEidApplication(BaseApplicationRequest request) {
        if (request == null) {
            return null;
        }
        IssueEidApplication application = new IssueEidApplication();
        mapAbstractApplication(application, request);
        application.setCitizenIdentifierNumber(request.getCitizenIdentifierNumber());
        application.setCitizenIdentifierType(request.getCitizenIdentifierType());
        return application;
    }

    public ResumeEidApplication mapToResumeEidApplication(BaseApplicationRequest request) {
        if (request == null) {
            return null;
        }
        ResumeEidApplication application = new ResumeEidApplication();
        mapAbstractApplication(application, request);

        return application;
    }

    public RevokeEidApplication mapToRevokeEidApplication(BaseApplicationRequest request) {
        if (request == null) {
            return null;
        }
        RevokeEidApplication application = new RevokeEidApplication();
        mapAbstractApplication(application, request);
        return application;
    }

    public StopEidApplication mapToStopEidApplication(BaseApplicationRequest request) {
        if (request == null) {
            return null;
        }
        StopEidApplication application = new StopEidApplication();
        mapAbstractApplication(application, request);
        return application;
    }

    public EjbcaEndEntityRequest mapToEjbcaEndEntityRequest(AbstractApplication application) {
        if (application == null) {
            return null;
        }
        EjbcaEndEntityRequest request = new EjbcaEndEntityRequest();
        request.setUsername(application.getEidentityId().toString());
        request.setPassword(application.getEidentityId().toString());
        String cn = "CN=" + application.getFirstName() + " " + application.getLastName();
        String serialNumber = "SN=PI:BG-"+application.getEidentityId().toString();
        String surname = "SURNAME=" + application.getLastName();
        String givenName = "GIVENNAME=" + application.getFirstName();
        String country = "C=BG";
        request.setSubjectDn(String.join(",", cn, serialNumber, surname, givenName, country));
//        request.setSubjectAltName("rfc822Name=" + application.getParams(EMAIL));
//        request.setEmail(application.getParam(EMAIL));
        request.setCaName(application.getParams().getCertificateCaName());
        request.setCertificateProfileName(ejbcaCertificateProfileName);
        request.setEndEntityProfileName(ejbcaEndEntityProfileName);
//        request.setToken("P12");
        request.setToken("USERGENERATED");
        request.setAccountBindingId(ejbcaAccountBindingId);
        return request;
    }

    public EjbcaCertificateEnrollRequest mapToEjbcaCertificateEnrollRequest(AbstractApplication application) {
        if (application == null) {
            return null;
        }
        EjbcaCertificateEnrollRequest request = new EjbcaCertificateEnrollRequest();

        PKCS10CertificationRequest csr = null;
        try {
        	csr = this.certificateProcessor.extractCsr(
        		Base64.getDecoder().decode(application.getParams().getCertificateSigningRequest()));
		} catch (Exception e) {
        	throw new ValidationMVRException(ErrorCode.CERTICATE_SIGNING_REQUEST_DATA_NOT_VALID, "data");
		}

        validateCSR(application, csr);
        //EJBCA works with PEM CSR
        try {
        	request.setCertificateRequest(this.certificateProcessor.convertCsrToPEM(csr));
		} catch (IOException e) {
        	throw new ValidationMVRException(ErrorCode.CERTICATE_SIGNING_REQUEST_DATA_NOT_VALID, csr.getClass().getSimpleName());
		}
        
        log.info("CSR: {}", request.getCertificateRequest());

        request.setCertificateProfileName(ejbcaCertificateProfileName);
        request.setEndEntityProfileName(ejbcaEndEntityProfileName);
        request.setCertificateAuthorityName(application.getParams().getCertificateCaName());
        request.setUsername(application.getEidentityId().toString());
        request.setPassword(application.getEidentityId().toString());
        request.setAccountBindingId(ejbcaAccountBindingId);
        request.setIncludeChain(true);
        request.setEmail(application.getParams().getEmail());
        return request;
    }

	public void validateCSR(AbstractApplication application, PKCS10CertificationRequest csr) throws  ValidationMVRException{
		X500Name x500Name = csr.getSubject();
        String serialNumber = this.certificateProcessor.getFirstRdnValue(x500Name, BCStyle.SERIALNUMBER);
        String surname = this.certificateProcessor.getFirstRdnValue(x500Name, BCStyle.SURNAME);
        String givenName = this.certificateProcessor.getFirstRdnValue(x500Name, BCStyle.GIVENNAME);
        String commonName = this.certificateProcessor.getFirstRdnValue(x500Name, BCStyle.CN);
        String country = this.certificateProcessor.getFirstRdnValue(x500Name, BCStyle.C);

		String applicationNames = Arrays.asList(application.getFirstName(), application.getSecondName(), application.getLastName())
				.stream()
				.filter(e -> e != null)
				.collect(Collectors.joining(" "));

		if (!StringUtils.equalsIgnoreCase(applicationNames, commonName)) {
			throw new ValidationMVRException(ErrorCode.CERTICATE_SIGNING_REQUEST_DATA_NOT_VALID, "commonName");
		}

        if(surname == null || surname.isBlank()) {
        	throw new ValidationMVRException(ErrorCode.CERTICATE_SIGNING_REQUEST_DATA_NOT_VALID, "surname");
        }

        if(!StringUtils.equalsIgnoreCase(givenName, application.getFirstNameLatin())) {
        	throw new ValidationMVRException(ErrorCode.CERTICATE_SIGNING_REQUEST_DATA_NOT_VALID, "givenName");
        }

        //PERSO_CENTRE might be missing serialNumber
        if(ApplicationSubmissionType.PERSO_CENTRE != application.getSubmissionType()) {
	        if(serialNumber != null && !serialNumber.isBlank()) {
        	String subjectEid = serialNumber.substring(X509_CERT_SUBJ_SN_PREFIX.length());
        	if(!StringUtils.equals(subjectEid, application.getEidentityId().toString())) {
        		throw new ValidationMVRException(ErrorCode.CERTICATE_SIGNING_REQUEST_DATA_NOT_VALID, "serialNumber");
        	}
	        } else {
        		throw new ValidationMVRException(ErrorCode.CERTICATE_SIGNING_REQUEST_DATA_NOT_VALID, "serialNumber");
	        }
        }

        if(country == null || country.isBlank()) {
        	throw new ValidationMVRException(ErrorCode.CERTICATE_SIGNING_REQUEST_DATA_NOT_VALID, "country");
        }

	}

    public Map<String, Object> mapToKeyValueMap(AbstractApplication application) {
        if (application == null) {
            return Collections.emptyMap();
        }
        Map<String,Object> keyValueMap = new HashMap<>();
        
		String fullName = Arrays.asList(application.getFirstName(), application.getSecondName(), application.getLastName())
				.stream()
				.filter(e -> e != null)
				.collect(Collectors.joining(" "));

		String fullNameLatin = Arrays.asList(application.getFirstNameLatin(), application.getSecondNameLatin(), application.getLastNameLatin())
				.stream()
				.filter(e -> e != null)
				.collect(Collectors.joining(" "));
		
        keyValueMap.put("names", fullName);
        keyValueMap.put("namesLatin", fullNameLatin);
        keyValueMap.put("citizenship", application.getCitizenship());
        keyValueMap.put("citizenIdentifierNumber", application.getCitizenIdentifierNumber());
        keyValueMap.put("applicationNumber", application.getApplicationNumber().getId());
        keyValueMap.put("phone", application.getParams().getPhoneNumber());
        keyValueMap.put("email", application.getParams().getEmail());
        keyValueMap.put("applicationType", application.getApplicationType());
        keyValueMap.put("requireGuardians", application.getParams().getRequireGuardians());
        
        if (List.of(BASE_PROFILE, EID).contains(application.getSubmissionType())){
        	byte[] detachedSignature = Base64.getDecoder().decode(application.getDetachedSignature().getBytes());
        	CMSSignedData cmsSignedData = null;
        	Date createDate = null;
			try {
				cmsSignedData = this.cryptoTimestampProcessor.parseCMSSignedData(detachedSignature);
				createDate = this.cryptoTimestampProcessor.getSigningTime(cmsSignedData);
			} catch (CMSException | ParseException e) {
				log.error(".process() ", "Could not parse CMSSignedData Application with id: {}, Exreption: {}", application.getId(), e);
				
				createDate = Date.from(application.getCreateDate().toInstant(ZoneOffset.UTC));
			}
			
			String createDateString = createDate.toInstant().atZone(ZoneOffset.UTC).toLocalDateTime().format(SIGNATURE_INFO_DATE_FORMATTER);
			
        	keyValueMap.put("signatureInfo", "Подписано на " + createDateString);
        }
        
        EidApplicationXml eidApplicationXml = fileFormatService.createObjectFromXmlString(application.getApplicationXml(), EidApplicationXml.class);
        keyValueMap.put("identityNumber", eidApplicationXml.getIdentityNumber());
        keyValueMap.put("identityIssueDate", LocalDate.parse(eidApplicationXml.getIdentityIssueDate()));
        keyValueMap.put("identityValidityToDate", LocalDate.parse(eidApplicationXml.getIdentityValidityToDate()));

        String formType = switch (application.getApplicationType()) {
        case ISSUE_EID -> "издаване";
        case RESUME_EID -> "възобновяване";
        case REVOKE_EID -> "прекратяване";
        case STOP_EID -> "спиране";
        default -> "";
        };
        keyValueMap.put("formType", formType);
        
        String citizenIdentifierType = switch (application.getCitizenIdentifierType()) {
        case EGN -> "ЕГН";
        case LNCh -> "ЛНЧ";
        default -> "";
        };
        keyValueMap.put("citizenIdentifierType", citizenIdentifierType);

        keyValueMap.put("currentDate", LocalDate.now());
        keyValueMap.put("dateOfBirth", application.getParams().getDateOfBirth());
        
        if(application.getApplicationType() != ApplicationType.ISSUE_EID) {
            DeviceDTO deviceType = raeiceiService.getDeviceById(application.getDeviceId());
            keyValueMap.put("deviceType", deviceType.getType());
            keyValueMap.put("deviceSerialNumber", application.getParams().getDeviceSerialNumber());
            keyValueMap.put("eidentityId", application.getEidentityId());
            keyValueMap.put("certificateSerialNumber", application.getParams().getCertificateSerialNumber());
            EidAdministratorDTO administrator = raeiceiService.getEidAdministratorById(application.getEidAdministratorId());
            keyValueMap.put("eidAdministratorName", administrator.getName());
            if (Objects.nonNull(application.getReason())) {
                keyValueMap.put("reason", StringUtils.isNotBlank(application.getReasonText()) ? application.getReasonText() : application.getReason().getDescription());
            }
        }


        if (eidApplicationXml.getGuardians() != null && !eidApplicationXml.getGuardians().isEmpty()) {
            keyValueMap.putAll(mapGuardiansToKeyValueMap(eidApplicationXml.getGuardians()));
        }
        
        return keyValueMap;
    }

    public Map<String, Object> mapGuardiansToKeyValueMap(List<GuardianDetailsXml> guardians) {
        Map<String,Object> keyValueMap = new HashMap<>();
        AtomicInteger i = new AtomicInteger(1);
        guardians.forEach(guardian -> {
            keyValueMap.put("guardianNames" + i, guardian.getFirstName() + " " + guardian.getSecondName() + " " + guardian.getLastName());
            keyValueMap.put("guardianEgn" + i, guardian.getCitizenIdentifierNumber());
            keyValueMap.put("guardianIdentityNumber" + i, guardian.getPersonalIdentityDocument().getIdentityNumber());
            keyValueMap.put("guardianIssueDate" + i, guardian.getPersonalIdentityDocument().getIdentityIssueDate());
            i.getAndIncrement();
        });
        return keyValueMap;
    }

    /**
     * Takes data from Front-end to populate the {@link Map}
     */
    public Map<String, Object> mapToKeyValueMapFromFE() {
        Map<String,Object> keyValueMap = new HashMap<>();
        
        //This is expected to be called only from EID Administrators
        UserContext userContext = UserContextHolder.getFromServletContext();
        keyValueMap.put("idAdminOfficerName", userContext.getName());
        
        return keyValueMap;
    }

    @Mapping(expression = "java(raeiceiService.getEidAdministratorById(application.getEidAdministratorId()).getName())", target = "eidAdministratorName")
    @Mapping(source = "params.paymentAccessCode", target = "paymentAccessCode")
    @Mapping(source = "applicationNumber.id", target = "applicationNumber")
    @Mapping(source = "administratorFrontOfficeId", target = "eidAdministratorFrontOfficeId")
    public abstract ApplicationDTO mapToApplicationDTO(AbstractApplication application);

    public abstract DbApplicationFilter mapToDbApplicationFilter(ApplicationFilter applicationFilter);
    
    public Page<ApplicationDTO> mapToApplicationDTOPage(Page<AbstractApplication> applications) {
        return applications.map(this::mapToApplicationDTO);
    }

    public String[] mapToArray(ApplicationDTO dto) {
        return new String[] {
        		dto.getApplicationNumber(),
        		dto.getApplicationType().getValue(),
        		dto.getEidAdministratorFrontOfficeId().toString(),
        		dto.getSubmissionType().getValue(),
        		dto.getCreateDate().toString(),
        		dto.getDeviceId().toString(),
        		dto.getEidAdministratorName(),
        		dto.getStatus().getValue()
            };
    };

    
    public OffsetDateTime map(LocalDateTime dateTime) {
        return dateTime.atOffset(ZoneOffset.ofHours(0));
    }
    
    public LocalDateTime map(OffsetDateTime dateTime) {
        return dateTime.atZoneSameInstant(ZoneOffset.UTC).toLocalDateTime();
    }

    public PunCreateCarrierRequest mapToPunCarrierRequest(AbstractApplication application) {
        if (application == null) {
            return null;
        }

        PunCreateCarrierRequest request = new PunCreateCarrierRequest();
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        request.setPunDeviceId(device.getId());
        request.setEidentityId(application.getEidentityId());
        String serialNumber;
        if (MVRConstants.MVR_ADMINISTRATOR_ID.equals(application.getEidAdministratorId())) {
           serialNumber = switch (device.getType()) {
                case MOBILE -> application.getParams().getMobileApplicationInstanceId();
                case CHIP_CARD -> fileFormatService.createObjectFromXmlString(application.getApplicationXml(), EidApplicationXml.class).getIdentityNumber();
                default -> throw new ValidationMVRException(DEVICE_TYPE_NOT_RECOGNIZED);
            };
        } else {
            serialNumber = application.getParams().getDeviceSerialNumber();
        }

        request.setSerialNumber(serialNumber);
        request.setCertificateId(application.getParams().getCertificateId());
        return request;
    }

    public void map(@MappingTarget AbstractApplication application, ApplicationSignRequest request) {
        if (request == null) {
            return;
        }
        if (!StringUtils.isBlank(request.getEmail())) {
            application.getParams().setEmail(request.getEmail());
        }
        if (!StringUtils.isBlank(request.getPhoneNumber())) {
            application.getParams().setPhoneNumber(request.getPhoneNumber());
        }
        if(request.getGuardians() != null  && !request.getGuardians().isEmpty()) {
        	application.getTemporaryData().setGuardians(request.getGuardians());
        }
    }

    public void map(@MappingTarget AbstractApplication application, EnrollCertificateDTO dto) {
        if (dto == null) {
            return;
        }
        if (dto.getCertificateSigningRequest() != null) {
            application.getParams().setCertificateSigningRequest(dto.getCertificateSigningRequest());
        }
        if (dto.getCertificateAuthorityName() != null) {
            application.getParams().setCertificateCaName(dto.getCertificateAuthorityName());
        }
    }

    public void map(@MappingTarget AbstractApplication application, CertificateExtAdminRequest request) {
        if (request == null) {
            return;
        }
        if (request.getDeviceSerialNumber() != null) {
            application.getParams().setDeviceSerialNumber(request.getDeviceSerialNumber());
        }
        if (request.getClientCertificate() != null) {
            application.getParams().setClientCertificate(request.getClientCertificate());
        }
        
        if (request.getClientCertificateChain() != null) {
            application.getParams().setClientCertificateChain(request.getClientCertificateChain());
        }
    }

    public ApplicationDetailsExternalResponse mapToExtDetailsResponse(AbstractApplication application) {
        if (application == null) {
            return null;
        }
        ApplicationDetailsExternalResponse response = new ApplicationDetailsExternalResponse();
        response.setId(application.getId());
        response.setEmail(application.getParams().getEmail());
        response.setApplicationType(application.getApplicationType());
        response.setPhoneNumber(application.getParams().getPhoneNumber());
        response.setCreateDate(OffsetDateTime.of(application.getCreateDate(),ZoneOffset.ofHours(0)));
        response.setStatus(application.getStatus());
        response.setPaymentAccessCode(application.getParams().getPaymentAccessCode());
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        response.setDeviceId(device.getId());
        response.setEidentityId(application.getEidentityId());
        response.setReasonText(application.getReasonText());
        EidAdministratorDTO administrator = raeiceiService.getEidAdministratorById(application.getEidAdministratorId());
        response.setEidAdministratorName(administrator.getName());
        response.setSerialNumber(application.getParams().getCertificateSerialNumber());
        response.setXml(application.getApplicationXml());
        response.setApplicationNumber(application.getApplicationNumber().getId());
        EidApplicationXml xml = fileFormatService.createObjectFromXmlString(application.getApplicationXml(), EidApplicationXml.class);
        response.setFirstName(xml.getFirstName());
        response.setSecondName(xml.getSecondName());
        response.setLastName(xml.getLastName());
        response.setIdentityIssueDate(LocalDate.parse(xml.getIdentityIssueDate()));
        response.setIdentityNumber(xml.getIdentityNumber());
        response.setIdentityType(xml.getIdentityType());
        response.setIdentityValidityToDate(LocalDate.parse(xml.getIdentityValidityToDate()));
        response.setSubmissionType(application.getSubmissionType());
        if (Objects.nonNull(application.getReason())) {
            response.setReasonId(application.getReason().getId());
        }
        if (Objects.nonNull(application.getParams().getCertificateId())) {
            response.setCertificateId((application.getParams().getCertificateId()));
        }
        EidManagerFrontOfficeDTO office = raeiceiService.getOfficeById(application.getAdministratorFrontOfficeId());
        response.setEidAdministratorOfficeName(office.getName());
        return response;
    }

    public ApplicationDetailsResponse mapToDetailsResponse(AbstractApplication application) {
        if (application == null) {
            return null;
        }
        ApplicationDetailsResponse response = new ApplicationDetailsResponse();
        response.setId(application.getId());
        response.setEmail(application.getParams().getEmail());
        response.setApplicationType(application.getApplicationType());
        response.setPhoneNumber(application.getParams().getPhoneNumber());
        response.setCreateDate(OffsetDateTime.of(application.getCreateDate(),ZoneOffset.ofHours(0)));
        response.setStatus(application.getStatus());
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        response.setDeviceId(device.getId());
        response.setEidentityId(application.getEidentityId());
        response.setReasonText(application.getReasonText());
        EidAdministratorDTO administrator = raeiceiService.getEidAdministratorById(application.getEidAdministratorId());
        response.setEidAdministratorFrontOfficeId(application.getAdministratorFrontOfficeId());
        response.setEidAdministratorName(administrator.getName());
        response.setSerialNumber(application.getParams().getCertificateSerialNumber());
        response.setSubmissionType(application.getSubmissionType());
        response.setApplicationNumber(application.getApplicationNumber().getId());
        EidManagerFrontOfficeDTO office = raeiceiService.getOfficeById(application.getAdministratorFrontOfficeId());
        response.setEidAdministratorOfficeName(office.getName());
        if (Objects.nonNull(application.getReason())) {
            response.setReasonId(application.getReason().getId());
        }
        if (Objects.nonNull(application.getParams().getCertificateId())) {
            response.setCertificateId(application.getParams().getCertificateId());
        }
        response.setFirstName(application.getFirstName());
        response.setSecondName(application.getSecondName());
        response.setLastName(application.getLastName());
        response.setFirstNameLatin(application.getFirstNameLatin());
        response.setSecondNameLatin(application.getSecondNameLatin());
        response.setLastNameLatin(application.getLastNameLatin());
        EidApplicationXml xml = fileFormatService.createObjectFromXmlString(application.getApplicationXml(), EidApplicationXml.class);
        response.setIdentityIssueDate(LocalDate.parse(xml.getIdentityIssueDate()));
        response.setIdentityNumber(xml.getIdentityNumber());
        response.setIdentityType(xml.getIdentityType());
        response.setIdentityValidityToDate(LocalDate.parse(xml.getIdentityValidityToDate()));
        response.setCitizenship(application.getCitizenship());
        response.setFee(application.getParams().getFee());
        response.setFeeCurrency(application.getParams().getFeeCurrency());
        response.setSecondaryFee(application.getParams().getSecondaryFee());
        response.setSecondaryFeeCurrency(application.getParams().getSecondaryFeeCurrency());
        response.setOperatorUsername(application.getUpdatedBy());
        
        response.setNumForm(application.getParams().getNumForm());
        return response;
    }

    public EDeliveryMessageRequest mapToEDeliveryMessageRequest(SsevSendMessageDTO dto) {
        if (dto == null) {
            return null;
        }
        EDeliveryMessageRequest request = new EDeliveryMessageRequest();
        request.setSubject(SsevNotificationServiceImpl.createMessageTitle());
        request.setRecipientProfileIds(List.of(Long.parseLong(dto.getEDeliveryProfileId())));
        request.setContent(SsevNotificationServiceImpl.createMessageContent(dto.getApplicationType()));
        return request;
    }

    public abstract RueiVerifyProfileDTO map(SignOnlineApplicationRequest request);

    public abstract RueiVerifyProfileDTO map(ProfileVerifyLoginRequest request);

    public abstract RueiVerifyProfileDTO map(SignOnlineApplicationEidRequest request);

    public abstract EnrollCertificateDTO map(CertificateRequest request);

    public abstract EnrollCertificateDTO map(MobileCertificateEidRequest request);

    public abstract EnrollCertificateDTO map(MobileCertificateBasicProfileRequest request);

    public PersoCentreApplicationResponse map(AbstractApplication application) {
        if (application == null) {
            return null;
        }
        PersoCentreApplicationResponse response = new PersoCentreApplicationResponse();
        response.setApplicationId(application.getId());
        response.setCertificate(application.getParams().getClientCertificate());
        response.setCertificateCA(application.getParams().getClientCertificateChain());
        response.setCertificateId(application.getParams().getCertificateId());
        return response;
    }

    public CalculateTariffRequest mapToTariffRequest(AbstractApplication application) {
        ProvidedServiceResponseDTO providedService = raeiceiClient.getProvidedServiceByApplicationType(application.getApplicationType());
        CalculateTariffRequest request = new CalculateTariffRequest();
        //TODO: this will throw exception if we call /mpozei/api/v1/applications?regixAvailability=false
        int age = LocalDate.now().getYear() - LocalDate.parse(application.getParams().getDateOfBirth()).getYear();
        request.setAge(age);
        request.setCurrentDate(LocalDate.now());
        request.setDisability(false);
        request.setEidManagerId(application.getEidAdministratorId());
        request.setProvidedServiceId(providedService.getId());
        request.setDeviceId(application.getDeviceId());
        return request;
    }

    public MisepPaymentRequest mapToMisepPaymentRequest(AbstractApplication application) {
        EidAdministratorDTO eidAdministrator = raeiceiService.getEidAdministratorById(application.getEidAdministratorId());
        MisepClientResponse clientsResponse = misepClient.getClientsByEik(eidAdministrator.getEikNumber());
//        assertTrue(clientsResponse.getClients().size() == 1, WRONG_NUMBER_OF_CLIENTS_FOR_EIK);
        //TODO: which one to get .get(0). Check the logic here when we have correct test data
        MisepClientResponse.Unite unite = clientsResponse.getUnites().get(1);

        MisepPaymentRequest request = new MisepPaymentRequest();
        MisepPaymentRequest.Actor actor = new MisepPaymentRequest.Actor();
        request.getRequest().getActors().add(actor);

        MisepPaymentRequest.Contact contact = new MisepPaymentRequest.Contact();
        actor.getInfo().setContacts(contact);

        request.setCitizenProfileId(application.getCitizenProfileId());
        actor.getUid().setType(application.getCitizenIdentifierType());
        actor.getUid().setValue(application.getCitizenIdentifierNumber());
        actor.setName(application.getFullLatinName());
        contact.setPhone(application.getParams().getPhoneNumber());
        contact.setEmail(application.getParams().getEmail());
        contact.setAddress(new MisepPaymentRequest.Address());
        // TODO: 8/23/2024 address is unknown
        actor.getInfo().setBankAccount(unite.getInfo().getBankAccount());

        UUID mpozeiPaymentId = UUID.randomUUID();
        application.getParams().setMpozeiPaymentId(mpozeiPaymentId);
        request.getRequest().getPaymentData().setReferenceNumber(application.getId().toString());
        request.getRequest().getPaymentData().setReferenceDate(application.getCreateDate().toString());
        request.getRequest().getPaymentData().setReason(application.getApplicationType().name());
        request.getRequest().getPaymentData().setReferenceType(application.getApplicationType().name());
        request.getRequest().getPaymentData().setPaymentId(mpozeiPaymentId.toString());
        request.getRequest().getPaymentData().setCurrency(application.getParams().getFeeCurrency());
        request.getRequest().getPaymentData().setAmount(application.getParams().getFee());
        request.getRequest().getPaymentData().setCreateDate(LocalDateTime.now(ZoneOffset.UTC).format(DateTimeFormatter.ISO_LOCAL_DATE));
        request.getRequest().getPaymentData().setExpirationDate(LocalDateTime.now(ZoneOffset.UTC).plusDays(7).format(DateTimeFormatter.ISO_LOCAL_DATE));
        return request;
    }

    public List<GuardianDetails> map(List<GuardianDetailsXml> guardians) {
        List<GuardianDetails> result = new ArrayList<>();
        guardians.forEach(e -> result.add(map(e)));
        return result;
    }

    public abstract GuardianDetails map(GuardianDetailsXml e);

    public EDeliveryProfileRequest mapToEDeliveryProfileRequest(SsevSendMessageDTO dto) {
        if (dto == null) {
            return null;
        }
        EDeliveryProfileRequest request = new EDeliveryProfileRequest();
        request.setEmail(dto.getEmail());
        request.setPhone(dto.getPhoneNumber());
        request.setIdentifier(dto.getCitizenIdentifierNumber());
        request.setFirstName(dto.getFirstName());
        request.setMiddleName(dto.getSecondName());
        request.setLastName(dto.getLastName());
        request.getAddress().setResidence("Служебна регистрация на МВР");
        return request;
    }

    public SsevSendMessageDTO mapToSsevRequest(AbstractApplication application) {
        if (application == null) {
            return null;
        }

        SsevSendMessageDTO dto = new SsevSendMessageDTO();
        dto.setEmail(application.getParams().getEmail());
        dto.setPhoneNumber(application.getParams().getPhoneNumber());
        dto.setCitizenIdentifierNumber(application.getCitizenIdentifierNumber());
        dto.setEidentityId(application.getEidentityId());
        dto.setFirstName(application.getFirstName());
        dto.setSecondName(application.getSecondName());
        dto.setLastName(application.getLastName());
        dto.setIssuerDN(application.getParams().getIssuerDn());
        dto.setCertificateSerialNumber(application.getParams().getCertificateSerialNumber());
        dto.setApplicationType(application.getApplicationType());
        return dto;
    }

	public EjbcaCertificateRequestRestRequest mapToEjbcaCertificateRequest(AbstractApplication application) {
        if (application == null) {
            return null;
        }
        EjbcaCertificateRequestRestRequest request = new EjbcaCertificateRequestRestRequest();

        PKCS10CertificationRequest csr = null;
        try {
        	csr = this.certificateProcessor.extractCsr(
        		Base64.getDecoder().decode(application.getParams().getCertificateSigningRequest()));
		} catch (Exception e) {
        	throw new ValidationMVRException(ErrorCode.CERTICATE_SIGNING_REQUEST_DATA_NOT_VALID, "data");
		}

        validateCSR(application, csr);
        //EJBCA works with PEM CSR
        try {
        	request.setCertificateRequest(this.certificateProcessor.convertCsrToPEM(csr));
		} catch (IOException e) {
        	throw new ValidationMVRException(ErrorCode.CERTICATE_SIGNING_REQUEST_DATA_NOT_VALID, csr.getClass().getSimpleName());
		}
        
        log.info("CSR: {}", request.getCertificateRequest());

        request.setCertificateAuthorityName(application.getParams().getCertificateCaName());
        request.setUsername(application.getEidentityId().toString());
        request.setPassword(application.getEidentityId().toString());
        request.setIncludeChain(true);
        return request;
	}

    public EjbcaEndEntityStatusUpdateRequest map(SearchEndEntityDTO existing, UUID eidentityId) {
        if (existing == null || eidentityId == null) {
            return null;
        }
        EjbcaEndEntityStatusUpdateRequest request = new EjbcaEndEntityStatusUpdateRequest();

        //request.setToken(existing.getToken());
        request.setToken("USERGENERATED");
        request.setStatus("NEW");
        request.setPassword(eidentityId.toString());
        return request;
    }
}
