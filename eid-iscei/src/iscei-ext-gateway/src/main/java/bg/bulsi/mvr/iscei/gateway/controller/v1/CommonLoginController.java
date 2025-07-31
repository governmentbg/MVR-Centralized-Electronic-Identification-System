package bg.bulsi.mvr.iscei.gateway.controller.v1;

import java.io.IOException;
import java.text.ParseException;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;

import org.apache.commons.lang3.EnumUtils;
import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.autoconfigure.security.oauth2.client.OAuth2ClientProperties.Provider;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.security.oauth2.jwt.Jwt;
import org.springframework.security.oauth2.server.resource.authentication.JwtAuthenticationToken;
import org.springframework.util.LinkedMultiValueMap;
import org.springframework.util.MultiValueMap;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.reactive.function.client.WebClient;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.dto.AuditData;
import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.audit_logger.model.MessageType;
import bg.bulsi.mvr.common.config.GrantedAuthoritiesExtractor;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.dto.AcrClaim;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.iscei.contract.dto.AuthenticationRequestChallengeResponse;
import bg.bulsi.mvr.iscei.contract.dto.BasicLoginRequestDto;
import bg.bulsi.mvr.iscei.contract.dto.CitizenProfileAttachDTO;
import bg.bulsi.mvr.iscei.contract.dto.CitizenProfileDTO;
import bg.bulsi.mvr.iscei.contract.dto.EidentityDTO;
import bg.bulsi.mvr.iscei.contract.dto.ProfileStatusDTO;
import bg.bulsi.mvr.iscei.contract.dto.SignedChallenge;
import bg.bulsi.mvr.iscei.contract.dto.VerifyOtpDto;
import bg.bulsi.mvr.iscei.contract.dto.authrequest.X509CertAuthenticationRequestDto;
import bg.bulsi.mvr.iscei.gateway.client.ReiClient;
import bg.bulsi.mvr.iscei.gateway.client.RueiClient;
import bg.bulsi.mvr.iscei.gateway.service.OtpTokenService;
import bg.bulsi.mvr.iscei.gateway.service.ProxiedClientService;
import bg.bulsi.mvr.iscei.gateway.service.X509CertificateLogin;
import bg.bulsi.mvr.iscei.model.AcrScope;
import bg.bulsi.mvr.iscei.model.AuthenticationChallenge;
import bg.bulsi.mvr.iscei.model.OtpToken;
import bg.bulsi.mvr.iscei.model.repository.keypair.AuthChallengeRepository;
import bg.bulsi.mvr.iscei.model.service.AuthenticationStatisticsService;
import bg.bulsi.mvr.iscei.pan.EventRegistratorImpl;
import bg.bulsi.mvr.pan_client.DirectEmailRequest;
import bg.bulsi.mvr.pan_client.EventRegistrator;
import bg.bulsi.mvr.pan_client.EventRegistrator.Event;
import bg.bulsi.mvr.pan_client.NotificationSender;
import bg.bulsi.mvr.pan_client.EventRegistrator.Translation;
import io.micrometer.core.instrument.Counter;
import io.micrometer.core.instrument.MeterRegistry;
import jakarta.annotation.PostConstruct;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.validation.Valid;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@RestController
@RequestMapping("/api/v1/auth")
public class CommonLoginController {

	@Autowired
	private AuthChallengeRepository authChallengeRepository;

	@Autowired
	private BaseAuditLogger auditLogger;
	
	@Autowired
	private Provider keycloakExternal;
	
	@Autowired
	private ProxiedClientService proxiedClientService;
	
    @Autowired
    private MeterRegistry meterRegistry;

	@Autowired
	private NotificationSender notificationSender;
	
	@Autowired
	private ReiClient reiClient;
	
	@Autowired
	private RueiClient rueiClient;
	
	@Autowired
	private X509CertificateLogin x509CertificateLogin;

	@Autowired
	private OtpTokenService otpTokenService;
	
	@Autowired
	private AuthenticationStatisticsService statisticsService;
	
	@Value("${mvr.iscei_ui_base_url}")
	private String isceiUiBaseUrl;
	
	private String isceiUi2FaUrl;
	
	private Counter basicAuthCounter;
	
    private Map<String, Counter> basicLoginCounters = new ConcurrentHashMap<>();
    
    private Event baseSuccessAuthEvent;
//    private Event baseAttemptSuccessEvent;
    
	public CommonLoginController(MeterRegistry registry, EventRegistrator eventRegistrator) {
		this.baseSuccessAuthEvent = eventRegistrator.getEvent(EventRegistratorImpl.ISCEI_SUCCESSFUL_AUTHENTICATION_WITH_BASIC_PROFILE);
//		this.baseAttemptSuccessEvent = eventRegistrator.getEvent(EventRegistratorImpl.ISCEI_ATTEMPT_AUTHENTICATION_WITH_BASIC_PROFILE);

		this.basicAuthCounter = Counter.builder("basic_auth")
	    	      .description("Number of basic auth")
	    	      .register(registry);
	}
	
	@PostMapping(path = "/basic",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public ResponseEntity<JsonNode> basicLogin(
			@Valid @RequestBody BasicLoginRequestDto requestDto,
			HttpServletRequest httpRequest
			//@RegisteredOAuth2AuthorizedClient("iscei") OAuth2AuthorizedClient isceiClient
			) throws IOException, ParseException{
		
		log.info(".basicLogin()");
		
		basicAuthCounter.increment();
		
		this.getLoginCounter(requestDto.getClient_id()).increment();
		
		httpRequest.setAttribute(AuditEventType.AUDIT_EVENT_TYPE_KEY, AuditEventType.AUTH_BASIC);;
    	UserContext userContext = UserContextHolder.getFromServletContext();
		
		//TODO: move this so it has CitizenProfile
		//TODO: fix correlationId
		AuditData reqAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(AuditEventType.AUTH_BASIC)
				.messageType(MessageType.REQUEST)
				.payload(null)
				.requesterSystemId(requestDto.getClient_id())
				.requesterSystemName(requestDto.getClient_id())
				.build();
		
		auditLogger.auditEvent(reqAuditEvent);	
		
		this.statisticsService.createAuthenticationRequestBaseProfile(requestDto.getClient_id(), httpRequest);
		
        MultiValueMap<String, String> requestBody = new LinkedMultiValueMap<>();
        
        this.proxiedClientService.evaluateClient(requestDto.getClient_id(), requestBody);
        
        requestBody.add("client_id", requestDto.getClient_id());
       // requestBody.add("client_secret", isceiClient.getClientRegistration().getClientSecret());
        requestBody.add("grant_type", "password");
        requestBody.add("scope", AcrScope.EID_LOA_LOW.getAcrScopeName());
        requestBody.add("username", requestDto.getEmail());
        requestBody.add("password", requestDto.getPassword());

		log.info(".basicLogin() [requestBody={}]", requestBody);
        
        ResponseEntity<JsonNode> response = WebClient
        .builder()
        .build()
        .post()
        .uri(keycloakExternal.getTokenUri())
        .contentType(MediaType.APPLICATION_FORM_URLENCODED)
        .bodyValue(requestBody)
        .retrieve()
        .toEntity(JsonNode.class)
//        .toEntity(new ParameterizedTypeReference<HashMap<String, Object>>() {})
        .block();
        
		CitizenProfileDTO citizenProfileDTO = null;
		try{
			citizenProfileDTO = this.rueiClient.getCitizenProfileByEmail(requestDto.getEmail());
			
			//Send attempt authentication notification
//			Translation translationAttempt = baseAttemptSuccessEvent.translations().stream().filter(e -> StringUtils.equals(e.language(), language)).findFirst().get();
//			this.notificationSender.sendDirectEmail(new DirectEmailRequest(language, translationAttempt.shortDescription(), translationAttempt.description(), requestDto.getEmail()));
		} catch (Exception e) {
			log.error(".basicLogin() Email not found in RUEI [email={}]", requestDto.getEmail(), e);
		}
		
		if(citizenProfileDTO.getStatus() != ProfileStatusDTO.ENABLED) {
            throw new ValidationMVRException(ErrorCode.BASE_PROFILE_NOT_ACTIVE);
		}
        
        
		if(Boolean.TRUE.equals(citizenProfileDTO.getIs2FaEnabled())) {
			OtpToken otpToken = this.otpTokenService.createOtpToken(citizenProfileDTO.getEmail(), requestDto.getClient_id(), response.getBody().toString());

			this.otpTokenService.sendOtpToEmail(otpToken);
			
	        ObjectMapper mapper = new ObjectMapper();
	        ObjectNode objectNode = mapper.createObjectNode();
	        
	        // Optionally, add properties
	        objectNode.put("isceiUi2FaUrl", this.isceiUi2FaUrl);
	        objectNode.put("sessionId", otpToken.getId().toString());
	        objectNode.put("ttl", otpToken.getTtl());

	        return new ResponseEntity<>(objectNode, HttpStatus.OK);
		}
		
		String language = "bg";
		if(citizenProfileDTO.getEidentityId() != null) {
			this.notificationSender.send(baseSuccessAuthEvent.code(), citizenProfileDTO.getEidentityId());
		} else {
			//TODO: from we take preferedLanguage token or RUEI
			Translation translationSuccess = baseSuccessAuthEvent.translations().stream().filter(e -> StringUtils.equals(e.language(), language)).findFirst().get();
			this.notificationSender.sendDirectEmail(new DirectEmailRequest(language, translationSuccess.shortDescription(), translationSuccess.description(), citizenProfileDTO.getEmail()));
		}
		
		EventPayload eventPayloadReq = null;
		if(citizenProfileDTO.getCitizenIdentifierNumber() != null) {
			eventPayloadReq = new EventPayload();
			eventPayloadReq.setRequesterUid(citizenProfileDTO.getCitizenIdentifierNumber());
			eventPayloadReq.setRequesterUidType(citizenProfileDTO.getCitizenIdentifierType().name());
			eventPayloadReq.setTargetUid(citizenProfileDTO.getCitizenIdentifierNumber());
			eventPayloadReq.setTargetUidType(citizenProfileDTO.getCitizenIdentifierType().name());
		}
		
		AuditData respAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(AuditEventType.AUTH_BASIC)
				.messageType(MessageType.SUCCESS)
				.payload(eventPayloadReq)
				.requesterUserId(citizenProfileDTO.getId().toString())
				.requesterSystemId(requestDto.getClient_id())
				.requesterSystemName(requestDto.getClient_id())
				.targetUserId(citizenProfileDTO.getId().toString())
				.build();
		
		this.auditLogger.auditEvent(respAuditEvent);	
		
		this.statisticsService.createAuthenticationResultBaseProfile(requestDto.getClient_id(), citizenProfileDTO.getId(), citizenProfileDTO.getEidentityId(), httpRequest);
		
        return response;
	}
	
	// the endpoint will be called from anywhere when the citizen wants to
	// authenticate using certificate with some reader (the mobile application is
	// also a reader)
	// the response later will be signed from the card and the signature will be
	// sent to the server
	@PostMapping(path = "/generate-authentication-challenge",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public AuthenticationRequestChallengeResponse generateAuthenticationChallenge(
			@RequestBody X509CertAuthenticationRequestDto authenticationRequestDto) {
		//TODO: Verify level of assurance
		log.info(".generateAuthenticationChallenge()");
		
		AuthenticationChallenge authenticationChallenge = new AuthenticationChallenge();
		authenticationChallenge.setExpiration(2400l);
		authenticationChallenge.setLevelOfAssurance(authenticationRequestDto.getLevelOfAssurance());
		authenticationChallenge.setRequestFrom(authenticationRequestDto.getRequestFrom());
		
		UUID authRequestId = this.authChallengeRepository.save(authenticationChallenge).getId();

		// generate a challenge for the user
		var challengeResponse = new AuthenticationRequestChallengeResponse(authRequestId.toString());

		// do some validations
		// is the authenticationRequest.RequestFrom valid/allodwed
		// register the request in redis with timeout

		// this response will be signed from the card and the signature will be sent to
		// the server
		return challengeResponse;
	}
	
	@PostMapping(path = "/associate-profiles",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public void associateEidWithCitizenProfile(
			JwtAuthenticationToken principal,
			@RequestParam(name = "client_id") String clientId,
			@RequestBody SignedChallenge signedChallenge,
			HttpServletRequest httpRequest) {
		log.info(".associateEidWithCitizenProfile()");
		
		httpRequest.setAttribute(AuditEventType.AUDIT_EVENT_TYPE_KEY, AuditEventType.ASSOCIATE_PROFILES);
		
		Jwt userJwt = principal.getToken();
		AcrClaim acrClaim = EnumUtils.getEnumIgnoreCase(AcrClaim.class, userJwt.getClaim("acr"));
		//  "acr": "eid_low",
		if(acrClaim != AcrClaim.EID_LOW) {
			throw new ValidationMVRException(ErrorCode.BASE_PROFILE_AUTHENTICATION_REQUIRED);
		}
		
		UUID citizenProfileId = UUID.fromString(principal.getName());
		UUID eid = this.x509CertificateLogin.validateX509CertificateLogin(signedChallenge).getEidentityId();
		CitizenProfileDTO citizenProfileDTO = this.rueiClient.getCitizenProfileById(citizenProfileId);
		if(citizenProfileDTO.getEidentityId() != null) {
			throw new ValidationMVRException(ErrorCode.PROFILE_WITH_THAT_EIDENTITY_ID_EXISTS);
		}
		
		try {
			this.rueiClient.getCitizenProfileByEidentityId(eid);
			
			throw new ValidationMVRException(ErrorCode.PROFILE_WITH_THAT_EIDENTITY_ID_EXISTS);
		} catch (Exception ex) {
			log.info(".associateEidWithCitizenProfile() Expected Exception ", ex);
		}
		
		EidentityDTO eidentityDto = this.reiClient.getEidentityById(eid);
		
		String firstName = citizenProfileDTO.getFirstName();
		String middleName = citizenProfileDTO.getSecondName();
		String lastName = citizenProfileDTO.getLastName();
//		String firstNameToken = userJwt.getClaim("given_name_cyrillic");
//		String middleNameToken = userJwt.getClaim("middle_name_cyrillic");
//		String lastNameToken = userJwt.getClaim("family_name_cyrillic");
		
		if(!StringUtils.equalsAnyIgnoreCase(eidentityDto.getFirstName(), firstName)) {
			 throw new ValidationMVRException(ErrorCode.FIRST_NAME_IS_DIFFERENT_FROM_EXISTING_ONE);
		}
		
		if(!StringUtils.equalsAnyIgnoreCase(eidentityDto.getSecondName(), middleName)) {
			 throw new ValidationMVRException(ErrorCode.SECOND_NAME_IS_DIFFERENT_FROM_EXISTING_ONE);
		}
		
		if(!StringUtils.equalsAnyIgnoreCase(eidentityDto.getLastName(), lastName)){
			 throw new ValidationMVRException(ErrorCode.LAST_NAME_IS_DIFFERENT_FROM_EXISTING_ONE);
		}
		
		CitizenProfileAttachDTO attachDto = new CitizenProfileAttachDTO();
		attachDto.setFirstName(firstName);
		attachDto.setSecondName(middleName);
		attachDto.setLastName(lastName);
		attachDto.setCitizenProfileId(citizenProfileId);
		attachDto.setEidentityId(eid);
		attachDto.setCitizenIdentifierNumber(eidentityDto.getCitizenIdentifierNumber());
		attachDto.setCitizenIdentifierType(eidentityDto.getCitizenIdentifierType());
		
		this.rueiClient.attachCitizenProfile(attachDto);
		
    	EventPayload eventPayloadResp = new EventPayload();
		eventPayloadResp.setRequesterUid(attachDto.getCitizenIdentifierNumber());
		eventPayloadResp.setRequesterUidType(attachDto.getCitizenIdentifierType().name());
		eventPayloadResp.setRequesterName(attachDto.getFirstName(), attachDto.getSecondName(), attachDto.getLastName());
		eventPayloadResp.setTargetUid(attachDto.getCitizenIdentifierNumber());
		eventPayloadResp.setTargetUidType(attachDto.getCitizenIdentifierType().name());
		eventPayloadResp.setTargetName(attachDto.getFirstName(), attachDto.getSecondName(), attachDto.getLastName());
		eventPayloadResp.setEidentityId(attachDto.getEidentityId().toString());
		eventPayloadResp.setProfileId(attachDto.getCitizenProfileId().toString());
		
		UserContext userContext = UserContextHolder.getFromServletContext();
		
		AuditData reqAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(AuditEventType.ASSOCIATE_PROFILES)
				.messageType(MessageType.SUCCESS)
				.payload(eventPayloadResp)
				.requesterUserId(userContext.getRequesterUserId())
				.requesterSystemId(clientId)
				.requesterSystemName(clientId)
				.targetUserId(userContext.getTargetUserId())
				.build();
		
		auditLogger.auditEvent(reqAuditEvent);	
	}
	
//	@PostMapping(path = "/generate-otp",
//			produces = MediaType.APPLICATION_JSON_VALUE)
//	public void generateOtp(
//			@Valid @RequestBody GenerateOtpDto generateOtp,
//			HttpServletRequest httpRequest) {
//		log.info(".generateOtp()");
//
//		OtpToken otpToken = this.otpTokenService.generateOtp(generateOtp);
//		
//		this.otpTokenService.sendOtpToEmail(otpToken);
//	}
	
	@PostMapping(path = "/verify-otp",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public ResponseEntity<String> verifyOtp(
			@Valid @RequestBody VerifyOtpDto verifyOtpDto,
			HttpServletRequest httpRequest) {
		log.info(".verifyOtp()");
		
		OtpToken otpToken = this.otpTokenService.verifyOtp(verifyOtpDto);
		
		AuditData respAuditEvent = AuditData.builder()
				.correlationId(httpRequest.getRequestId())
				.eventType(AuditEventType.AUTH_BASIC)
				.messageType(MessageType.SUCCESS)
				.payload(null)
				//.requesterSystemId(requestDto.getClient_id())
				//.requesterSystemName(requestDto.getClient_id())
				.build();
		
		auditLogger.auditEvent(respAuditEvent);	
		
		this.statisticsService.createAuthenticationRequestBaseProfile(otpToken.getClientId(), httpRequest);
		
		return ResponseEntity.ok(otpToken.getAuthServerResp().toString());
	}
	
	@PostConstruct
	private void init() {
		this.isceiUi2FaUrl = isceiUiBaseUrl + "/login/2fa";
	}
	
    private Counter getLoginCounter(String clientId) {
        // Retrieve or create a Counter for the specific clientId
        return basicLoginCounters.computeIfAbsent(clientId, client -> 
            Counter.builder("client_id.basic_auth")
                   .description("Number of logins by clientId")
                   .tag("clientId", clientId)
                   .register(meterRegistry)
        );
    }
}
