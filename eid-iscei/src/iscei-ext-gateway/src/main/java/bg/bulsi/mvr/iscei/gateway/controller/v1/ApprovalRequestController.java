package bg.bulsi.mvr.iscei.gateway.controller.v1;

import java.time.OffsetDateTime;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.Set;
import java.util.UUID;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.autoconfigure.security.oauth2.client.OAuth2ClientProperties.Provider;
import org.springframework.data.domain.Sort;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.security.oauth2.server.resource.authentication.JwtAuthenticationToken;
import org.springframework.util.LinkedMultiValueMap;
import org.springframework.util.MultiValueMap;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.reactive.function.client.WebClient;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.dto.AuditData;
import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.audit_logger.model.MessageType;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.FaultMVRException;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.iscei.contract.dto.CertificateLoginResultDto;
import bg.bulsi.mvr.iscei.contract.dto.CitizenProfileDTO;
import bg.bulsi.mvr.iscei.contract.dto.EidentityDTO;
import bg.bulsi.mvr.iscei.contract.dto.ExternalProviderLoginResultDto;
import bg.bulsi.mvr.iscei.contract.dto.IdentifierTypeDTO;
import bg.bulsi.mvr.iscei.contract.dto.LevelOfAssurance;
import bg.bulsi.mvr.iscei.contract.dto.ProxiedClient;
import bg.bulsi.mvr.iscei.contract.dto.approvalrequest.ApprovalRequestAuthResponse;
import bg.bulsi.mvr.iscei.contract.dto.approvalrequest.ApprovalRequestResponse;
import bg.bulsi.mvr.iscei.contract.dto.approvalrequest.ApprovalRequestStatus;
import bg.bulsi.mvr.iscei.contract.dto.approvalrequest.ApprovalRequestToken;
import bg.bulsi.mvr.iscei.contract.dto.approvalrequest.RelyPartyRequest;
import bg.bulsi.mvr.iscei.contract.dto.approvalrequest.RequestOutcome;
import bg.bulsi.mvr.iscei.contract.dto.authrequest.ApprovalAuthenticationRequestDto;
import bg.bulsi.mvr.iscei.gateway.client.ReiClient;
import bg.bulsi.mvr.iscei.gateway.client.RueiClient;
import bg.bulsi.mvr.iscei.gateway.mapper.ApprovalRequestMapper;
import bg.bulsi.mvr.iscei.gateway.service.X509CertificateLogin;
import bg.bulsi.mvr.iscei.model.AcrScope;
import bg.bulsi.mvr.iscei.model.AuthApprovalRequest;
import bg.bulsi.mvr.iscei.model.repository.keypair.AuthApprovalRequestRepository;
import bg.bulsi.mvr.iscei.model.service.AuthenticationStatisticsService;
import bg.bulsi.mvr.iscei.pan.EventRegistratorImpl;
import bg.bulsi.mvr.iscei.gateway.service.ExternalProviderLogin;
import bg.bulsi.mvr.iscei.gateway.service.ProxiedClientService;
import bg.bulsi.mvr.pan_client.EventRegistrator;
import bg.bulsi.mvr.pan_client.NotificationSender;
import bg.bulsi.mvr.pan_client.EventRegistrator.Event;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.validation.Valid;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@RestController
@RequestMapping("/api/v1/approval-request")
public class ApprovalRequestController extends BaseAuthorizationController {
	
	private static final String CIBA_GRANT_TYPE = "urn:openid:params:grant-type:ciba";

	@Value("${mvr.ciba-authorization-uri}")
	private String cibaAuthorizationUri;
	
	@Value("${mvr.ciba-authorization-callback-uri}")
	private String cibaAuthorizationCallbackUri;
	
	@Autowired
	private ObjectMapper objectMapper;

	@Autowired
	private AuthApprovalRequestRepository authApprovalRequestRepository;

	@Autowired
	private RueiClient rueiClient;
	
	@Autowired
	private ReiClient reiClient;

	@Autowired
	private X509CertificateLogin x509CertificateLogin;
	
	@Autowired
	private ExternalProviderLogin externalProviderLogin;
	
	@Autowired
	private Provider keycloakExternal;
	
	@Autowired
	private ProxiedClientService proxiedClientService;
	
	@Autowired
	private AuthenticationStatisticsService statisticsService;
	
	@Autowired
	private ApprovalRequestMapper approvalRequestMapper;
	
	@Autowired
	private BaseAuditLogger auditLogger;

	@Autowired
	private NotificationSender notificationSender;

    private Event authEidEvent;
    private Event authNewApprovalRequest;
	
	public ApprovalRequestController(EventRegistrator eventRegistrator) {
		this.authEidEvent = eventRegistrator.getEvent(EventRegistratorImpl.ISCEI_SUCCESSFUL_AUTHENTICATION_WITH_EID);
		this.authNewApprovalRequest = eventRegistrator.getEvent(EventRegistratorImpl.ISCEI_NEW_APPROVAL_REQUEST);
	}

	
//	@Autowired
//	private ClientRegistrationRepository clientRegistrationRepository;


//	@GetMapping("applications")
//	public Mono<List<AuthApprovalRequest>> getApplications(@RequestParam String username, ServerWebExchange exchange){
//		
//		return Mono.just(this.authApprovalRequestRepository.findAllByUsername(username));
//	}

	// the endpoint will be called from the browser when the citizen wants to
	// authenticate using the mobile application
	// as a result the citizen will get an authentication request id and will do a
	// poling request to the server to check if the authentication request is
	// approved
	// additional information should be provided from the browser that specifies
	// from where and for what resource the request is for
	// CIBA Ext Auth
	@PostMapping(path = "/auth/citizen",
			 produces = MediaType.APPLICATION_JSON_VALUE)
	public Object approvalRequestAuth(
			@RequestParam String clientId,
			@RequestParam(required = false, defaultValue = "") Set<String> scope,
			@RequestParam(required = false) Map<String, String> allParams,
			@Valid @RequestBody ApprovalAuthenticationRequestDto authenticationRequest,
			HttpServletRequest httpRequest
			//@Parameter(hidden = true) @RegisteredOAuth2AuthorizedClient(EID_ISCEI_EXT_CIBA_M2M) OAuth2AuthorizedClient isceiClient
			) {
        //log.info(".createDeskApplication() [result={}]", result);
		log.info(".approvalRequestAuth()");

		UserContext userContext = UserContextHolder.getFromServletContext();
		httpRequest.setAttribute(AuditEventType.AUDIT_EVENT_TYPE_KEY, AuditEventType.APPROVAL_REQUEST_AUTH);
		
		String citizenNumber = authenticationRequest.getCitizenNumber();
		IdentifierTypeDTO type = authenticationRequest.getType();
		
		// validate authenticationRequest.RequestFrom is valid

		// Extract user's profile from RUEI by EGN
		// validate that there is a user with the given EGN
		// if not return 404
		// if there is a user, get it's username (keycloak user name) and proceed with
		// the CIBA flow

		//TODO: validate scopes, provide whitelisting from YAML
		this.filterSupportedScopes(scope);
		
		EidentityDTO eidentityDTO = reiClient.findEidentityByNumberAndType(citizenNumber,  type);
		
		CitizenProfileDTO citizenProfileDTO = rueiClient.getCitizenProfileByEidentityId(eidentityDTO.getId());
		String citizenProfileId = citizenProfileDTO.getId().toString();
		
		EventPayload eventPayloadReq = new EventPayload();
		eventPayloadReq.setRequesterUid(citizenNumber);
		eventPayloadReq.setRequesterUidType(type.toString());
		eventPayloadReq.setTargetUid(citizenNumber);
		eventPayloadReq.setTargetUidType(type.toString());
		
		AuditData requestAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(AuditEventType.APPROVAL_REQUEST_AUTH)
				.messageType(MessageType.REQUEST)
				.payload(eventPayloadReq)
				.requesterUserId(citizenProfileId)
				.requesterSystemId(clientId)
				.requesterSystemName(clientId)
				.targetUserId(citizenProfileId)
				.build();
		
		this.auditLogger.auditEvent(requestAuditEvent);
		
        log.info(".approvalRequestAuth() [citizenProfileId={}]", citizenProfileId);
		
		MultiValueMap<String, String> requestBody = new LinkedMultiValueMap<>();
		ProxiedClient proxiedClient = this.proxiedClientService.evaluateClient(clientId, requestBody);
		if(proxiedClient == null) {
	       	throw new FaultMVRException(ErrorCode.AUTHENTICATION_REQUEST_NOT_FOUND);
		}
       
		ExternalProviderLoginResultDto providerLoginResultDto = this.externalProviderLogin.verifyLogin(citizenNumber, type, scope, allParams, requestBody);
		
		requestBody.add("client_id", clientId);

		// !!!somehow pass the authenticationRequest.RequestFrom/host
		requestBody.add("login_hint", citizenProfileId);// the username of the user in the keycloak extracted from theUEI
		//TODO: should be binding_message be sent from frontend
		String bindingMessage = UUID.randomUUID().toString();
		requestBody.add("binding_message", bindingMessage);
		
		requestBody.add("scope", AcrScope.EID_LOA_HIGH.getAcrScopeName());
		
		//requestBody.add("acr_values", authenticationRequest.getLevelOfAssurance().name());

//        for(Map.Entry<String, String> param: allParams.entrySet()) {
//    	      if(!requestBody.containsKey(param.getKey())) {
//    	    	  requestBody.add(param.getKey(), param.getValue());
//    	      }
//          }
		
		log.info(".approvalRequestAuth() [requestBody={}]", requestBody);
		
		// registering authentication request in keycloak
		ApprovalRequestAuthResponse keycloakResponse = WebClient.builder().build().post()
				.uri(this.cibaAuthorizationUri)
				.contentType(MediaType.APPLICATION_FORM_URLENCODED)
				.bodyValue(requestBody)
				.retrieve()
				.toEntity(ApprovalRequestAuthResponse.class)
				.block()
				.getBody();

		if(keycloakResponse != null) {
			log.info(".approvalRequestAuth() [keycloakResponse={}]", keycloakResponse);
			
			AuthApprovalRequest approvalRequest = this.authApprovalRequestRepository.findByUsernameAndBindingMessage(citizenProfileId, bindingMessage);
			if(approvalRequest == null) {
				log.error(".approvalRequestAuth() CIBA transaction must not be null");
				
		       	throw new FaultMVRException(ErrorCode.AUTHENTICATION_REQUEST_NOT_FOUND);
			}
			
			approvalRequest.setRequestFrom(authenticationRequest.getRequestFrom());
			approvalRequest.setMaxTtl(keycloakResponse.getExpires_in());
			approvalRequest.setExternalProviderLoginResult(providerLoginResultDto);
			
			this.authApprovalRequestRepository.save(approvalRequest);
			
			this.notificationSender.send(authNewApprovalRequest.code(), eidentityDTO.getId());
			
			log.info(".approvalRequestAuth() [approvalRequest={}]", approvalRequest);
		}

		AuditData respAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(AuditEventType.APPROVAL_REQUEST_AUTH)
				.messageType(MessageType.SUCCESS)
				.payload(eventPayloadReq)
				.requesterUserId(citizenProfileId)
				.requesterSystemId(clientId)
				.requesterSystemName(clientId)
				.targetUserId(citizenProfileId)
				.build();
		
		this.auditLogger.auditEvent(respAuditEvent);
		
		
		// authentication requestId is returned to the browser
		return keycloakResponse;
	}

	// the endpoint is called from keycloak to register the authentication request
	// CIBA Rely Party
	@PostMapping("/rely-party")
	public OffsetDateTime cibaRelyParty(HttpServletRequest request, HttpServletResponse response,
			@RequestBody RelyPartyRequest relyPartyRequest) {
		log.info(".cibaRelyParty() [relyPartyRequest={}]", relyPartyRequest);

		//TODO: Expose this endpoint only to Keycloak
	
		// call PAN to notify the user that there is a pending request for
		// authentication
		
		// the authentication request shoud be extended with information that specifies
		// from where and for what resource the request is for
		
		AuthApprovalRequest cibaTransaction = new AuthApprovalRequest();
		cibaTransaction.setUsername(relyPartyRequest.login_hint());
		cibaTransaction.setLevelOfAssurance(LevelOfAssurance.HIGH);
		cibaTransaction.setRelyPartyToken(request.getHeader(HttpHeaders.AUTHORIZATION));
		cibaTransaction.setBindingMessage(relyPartyRequest.binding_message());
		cibaTransaction.setCreateDate(OffsetDateTime.now());
		
		// registering the authentication request in redis/database
		this.authApprovalRequestRepository.save(cibaTransaction);

		log.info(".cibaRelyParty() [cibaTransaction={}]", cibaTransaction);
		
		response.setStatus(HttpStatus.CREATED.value());

		return OffsetDateTime.now();
	}

	// the endpoin is called from the mobile application to get the user's waiting
	// requests
	// the endpoint is protected and requires a token from the mobile application
	@GetMapping(path = "/user",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public List<ApprovalRequestResponse> getUserApprovalRequests(JwtAuthenticationToken principal) {
		
		log.info(".getUserApprovalRequests() [principal={}] [name={}]", principal, principal.getName());

		// get the waiting requests for the user based on the token
		// the requests should be filtered by the user that is trying to authenticate
		// the requests should be ordered by the expiration time
		// the requests should be ordered by the certificate level of assurance

		// returning the requests to the mobile application
		
		List<AuthApprovalRequest> userApprovalRequests = this.authApprovalRequestRepository.findAllByUsername(principal.getName(), Sort.by(Sort.Direction.DESC, "createDate"));
		return this.approvalRequestMapper.map(userApprovalRequests);
	}

	// the endpoint is called from the mobile application to approve or deny the
	// authentication request
	// the endpoint should be protected and require a token from the mobile
	// application
	// the mobile application should sign the waiting request with the card and
	// provide the signature
	@PostMapping(path = "/outcome",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public String evaluateRequestOutcome(@RequestParam UUID approvalRequestId, 
			@RequestBody RequestOutcome requestOutcome,
			HttpServletRequest httpRequest)
			throws JsonProcessingException {
		log.info(".evaluateRequestOutcome() [approvalRequestId={}] [requestOutcome={}]", approvalRequestId, requestOutcome);

		UserContext userContext = UserContextHolder.getFromServletContext();
		httpRequest.setAttribute(AuditEventType.AUDIT_EVENT_TYPE_KEY, AuditEventType.APPROVAL_REQUEST_OUTCOME);
		
		// validations:
		// extracting the public certificate from the signedByTheCardRequest
		// validate that the certificate is valid (bouncy castle check on CRL or OCSP)
		// => reusable logic for all certificates
		// validate that the targeted request is the same as the one that is signed
		// validate that the certificate is not temporary stopped in RUEI
		// validate that the certificate is for the user that is trying to authenticate
		// based on the provided token

		//TODO: this is also checked in bg.bulsi.mvr.iscei.gateway.service.X509CertificateLogin.validateX509CertificateLogin(SignedChallenge)
		AuthApprovalRequest authApprovalRequest = this.authApprovalRequestRepository.findById(approvalRequestId)
				.orElseThrow(() -> new ValidationMVRException(ErrorCode.AUTHENTICATION_REQUEST_NOT_FOUND));
		
		this.authApprovalRequestRepository.deleteById(approvalRequestId);
		
		CertificateLoginResultDto certificateLoginResult = null;
		//TODO: Do we need it when we decline approval request
		if(requestOutcome.getApprovalRequestStatus() == ApprovalRequestStatus.SUCCEED) {
			certificateLoginResult = this.x509CertificateLogin.validateX509CertificateLogin(requestOutcome.getSignedChallenge());
			
			log.info(".evaluateRequestOutcome() SUCCEED [approvalRequestId={}] [eid={}]", approvalRequestId, certificateLoginResult.getEidentityId());
		} else {
			log.info(".evaluateRequestOutcome() DECLINED [approvalRequestId={}]", approvalRequestId);
		}
		
		this.statisticsService.createAuthenticationRequestEid(requestOutcome.getClientId(), UUID.fromString(authApprovalRequest.getUsername()), authApprovalRequest.getExternalProviderLoginResult(), certificateLoginResult, httpRequest);
		
		String citizenProfileId = authApprovalRequest.getUsername();
		EventPayload eventPayloadReq = new EventPayload();
		eventPayloadReq.setRequesterUid(certificateLoginResult.getCitizenIdentifier());
		eventPayloadReq.setRequesterUidType(certificateLoginResult.getCitizenIdentifierType().name());
		eventPayloadReq.setTargetUid(certificateLoginResult.getCitizenIdentifier());
		eventPayloadReq.setTargetUidType(certificateLoginResult.getCitizenIdentifierType().name());
		
		AuditData requestAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(AuditEventType.APPROVAL_REQUEST_OUTCOME)
				.messageType(MessageType.REQUEST)
				.payload(eventPayloadReq)
				.requesterUserId(citizenProfileId)
				.requesterSystemId(requestOutcome.getClientId())
				.requesterSystemName(requestOutcome.getClientId())
				.targetUserId(citizenProfileId)
				.build();
		
		this.auditLogger.auditEvent(requestAuditEvent);
		
		String response = WebClient.builder().build().post()
				.uri(cibaAuthorizationCallbackUri)
				.header(HttpHeaders.AUTHORIZATION, authApprovalRequest.getRelyPartyToken())
//			        .attributes(ServerOAuth2AuthorizedClientExchangeFilterFunction.oauth2AuthorizedClient(isceiClient))
				.contentType(MediaType.APPLICATION_JSON)
				.bodyValue(this.objectMapper.writeValueAsString(Map.entry("status", requestOutcome.getApprovalRequestStatus())))
				.retrieve()
				.bodyToMono(String.class).block();
		
		AuditData respAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(AuditEventType.APPROVAL_REQUEST_OUTCOME)
				.messageType(MessageType.SUCCESS)
				.payload(eventPayloadReq)
				.requesterUserId(citizenProfileId)
				.requesterSystemId(requestOutcome.getClientId())
				.requesterSystemName(requestOutcome.getClientId())
				.targetUserId(citizenProfileId)
				.build();
		
		this.auditLogger.auditEvent(respAuditEvent);
		
		this.statisticsService.createAuthenticationResultEid(requestOutcome.getClientId(), UUID.fromString(authApprovalRequest.getUsername()), authApprovalRequest.getExternalProviderLoginResult(), certificateLoginResult, httpRequest);
		
		this.notificationSender.send(authEidEvent.code(), certificateLoginResult.getEidentityId());
		
		return response;
	}

	// the endpoint is called from the browser to get a token for the user when the
	// request is approved
	@PostMapping(path = "/token",
			produces = MediaType.APPLICATION_JSON_VALUE, consumes = MediaType.APPLICATION_JSON_VALUE)
	public String approvalRequestToken(
			@RequestParam String clientId, 
			@RequestBody ApprovalRequestToken authReqIdToken,
			HttpServletRequest httpRequest) {

		log.info(".approvalRequestToken() [authReqIdToken={}]", authReqIdToken);
		
//		AuditData reqAuditEvent = AuditData.builder()
//				.correlationId(httpRequest.getRequestId())
//				.eventType(AuditEventType.APPROVAL_REQUEST_TOKEN)
//				.messageType(MessageType.REQUEST)
//				.payload(null)
//				.requesterSystemId(clientId)
//				.requesterSystemName(clientId)
//				.build();
//		
//		auditLogger.auditEvent(reqAuditEvent);		
		
		MultiValueMap<String, String> bodyValues = new LinkedMultiValueMap<>();
		ProxiedClient proxiedClient = this.proxiedClientService.evaluateClient(clientId, bodyValues);
		if(proxiedClient == null) {
	       	throw new FaultMVRException(ErrorCode.AUTHENTICATION_REQUEST_NOT_FOUND);
		}
		
		bodyValues.add("client_id", clientId);
		bodyValues.add("grant_type", CIBA_GRANT_TYPE);
		bodyValues.add("auth_req_id", authReqIdToken.getAuth_req_id());

		//AuthApprovalRequest authRequest = this.authApprovalRequestRepository.findByUsername(username);

		log.info(".approvalRequestToken() [requestBody={}]", bodyValues);
		
		// getting Bearer access token and returning it to the citizen
		String response = WebClient.builder().build().post()
				.uri(keycloakExternal.getTokenUri())
				//.header(HttpHeaders.AUTHORIZATION, authRequest.getRelyPartyToken())
//        .attributes(ServerOAuth2AuthorizedClientExchangeFilterFunction.oauth2AuthorizedClient(isceiClient))
				.contentType(MediaType.APPLICATION_FORM_URLENCODED).bodyValue(bodyValues).retrieve()
				.bodyToMono(String.class).block();
		
//		AuditData respAuditEvent = AuditData.builder()
//				.correlationId(httpRequest.getRequestId())
//				.eventType(AuditEventType.APPROVAL_REQUEST_TOKEN)
//				.messageType(MessageType.SUCCESS)
//				.payload(null)
//				.requesterSystemId(clientId)
//				.requesterSystemName(clientId)
//				.build();
//		
//		auditLogger.auditEvent(respAuditEvent);	
	
		return response;
	}
	
//	// the endpoint will be called from anywhere when the citizen wants to
//	// authenticate using certificate with some reader (the mobile application is
//	// also a reader)
//	// CIBA Ext Auth
//	@PostMapping("/ciba/auth/certificate")
//	public ResponseEntity<String> cibaAuthUsingCertificate(
//			@RequestParam SignedChallenge signedChallenge, 
//			@RequestParam LevelOfAssurance levelOfAssurance, 
//			@RegisteredOAuth2AuthorizedClient(WebSecurityConfig.ISCEI_CLIENT_REGISTRATION_ID) OAuth2AuthorizedClient isceiClient,
//			HttpServletResponse httpResponse) {
//					log.info(".approvalRequestAuth() [keycloakResponse={}]", keycloakResponse);.println("====================>Request /ciba/auth" );
//		
//        			log.info(".approvalRequestAuth() [keycloakResponse={}]", keycloakResponse);.println("===========> " + isceiClient.getClientRegistration().getClientId());
//        			log.info(".approvalRequestAuth() [keycloakResponse={}]", keycloakResponse);.println("===========> " + isceiClient.getClientRegistration().getClientSecret());
//        
//        
//		//validations:
//		//validate the authentication requestFrom is valid
//		
//		//validate that the certificate is valid (bouncy castle check on CRL or OCSP) => reusable logic for all certificates
//        
//      //validate that the authentication request is still available in the redis
//        AuthenticationRequest authenticationRequest = this.authChallengeRepository.findById(UUID.fromString(signedChallenge.getChallenge())).get();
//        if(authenticationRequest == null) {
//        	throw new ValidationMVRException(ErrorCode.AUTHENTICATION_REQUEST_NOT_FOUND.getTitle(), null);
//        }
//        
//      //extracting the public certificate from the signedChalangeByTheCard
//        X509Certificate x509Certificate;
//		try {
//			x509Certificate = this.certificateProcessor.extractCertificate(signedChallenge.getCertificate());
//		} catch (CertificateException | IOException e) {
//        	throw new ValidationMVRException(ErrorCode.CERTIFICATE_IS_NOT_VALID.getTitle(), null);
//		}
//        
//		//validate that the signedChalangeByTheCard is signed properly
//        boolean isSignatureValid = this.certificateProcessor.verifySignature(
//        		signedChallenge.getChallenge().getBytes(), 
//        		signedChallenge.getSignature().getBytes(), 
//        		x509Certificate);
//
//					log.info(".approvalRequestAuth() [keycloakResponse={}]", keycloakResponse);.println("certificateProcessor.verifySignature isSignatureValid=" + isSignatureValid);
//
//        if(!isSignatureValid) {
//        	throw new ValidationMVRException(ErrorCode.SIGNATURE_DOES_NOT_MATCH.getTitle(), null);
//        }
//
//        CitizenCertificateDetailsDTO certificateDetailsDTO = rueiClient.getCitizenCertificateByIssuerAndSN(
//        		x509Certificate.getIssuerX500Principal().getName(), 
//        		x509Certificate.getSerialNumber().toString());
//        
//        
//      //validate that the certificate is not temporary stopped in RUEI
//        if(!CertificateStatusDTO.ACTIVE.equals(certificateDetailsDTO.getStatus())) {
//        	throw new ValidationMVRException(ErrorCode.CERTIFICATE_IS_NOT_ACTIVE.getTitle(), null);
//        }
//        
//        try {
//			if(certificateProcessor.compareCertificate(x509Certificate, certificateDetailsDTO.getCertificate())) {
//				throw new ValidationMVRException(ErrorCode.CERTIFICATES_DO_NOT_MATCH.getTitle(), null);
//			}
//		} catch (CertificateException | IOException e) {
//			e.printStackTrace();
//		}
//        
//        String username = "stanislav";
//        String redirectUri = "http://localhost:8889/index";
//        ResponseEntity<String> authResponseEntity = this.codeAuthFlowService.executeAuthEndpoint(username, redirectUri, isceiClient);
//        String locationHeader = authResponseEntity.getHeaders().get("Location").get(0);
//        
//        var uriComponents = UriComponentsBuilder.fromUriString(locationHeader).build();
//        String code = uriComponents.getQueryParams().get("code").get(0);
//        
//        ResponseEntity<String> tokenResponseEntity = this.codeAuthFlowService.executeTokenEndpoint(locationHeader, code, redirectUri, isceiClient);
//        
//        return tokenResponseEntity;
//        
//		//Extract user's profile from RUEI based on the public certificate that was extracted form the signedChalangeByTheCard
//		//validate that there is a user with the given EGN
//		//if not return 404
//		//if there is a user, get it's username (keycloak user name) and proceed with the CIBA flow
//	}

////====================================================================================================       
//        
//
//        MultiValueMap<String, String> requestBody = new LinkedMultiValueMap<>();
//        requestBody.add("username", "stanislav");
//        
//        requestBody.add("client_id", isceiClient.getClientRegistration().getClientId());
//        requestBody.add("client_secret", isceiClient.getClientRegistration().getClientSecret());
//        requestBody.add("state", UUID.randomUUID().toString());
//        requestBody.add("redirect_uri", redirectUri);
//        requestBody.add("response_type", responseType);
//        
//		//registering authentication request in keycloak		
//          WebClient
//        .builder()
////        .clientConnector(new ReactorClientHttpConnector(
////                HttpClient.create().followRedirect(true)
////        ))
//        .build()
//        .post()
//        .uri(builder -> builder
//        		.scheme("https")
//        		.host("mvreid-keycloak.local")
//        		.path("/realms/dev/protocol/openid-connect/auth")
//        		.port("8443")
//        		.build()
//        		)
//        .contentType(MediaType.APPLICATION_FORM_URLENCODED)
//        .bodyValue(requestBody)
//        .retrieve()
//        .toEntity(String.class)
//        .doOnSuccess(c-> 			log.info(".approvalRequestAuth() [keycloakResponse={}]", keycloakResponse);.println("============> c.getHeaders() = " + c.getHeaders()))
//        .block()
//		;
//        
//        
//
//        
//        
////====================================================================================================       
//        
//        
//        
//        
//        
//        
//        
//        
//        
//
//        MultiValueMap<String, String> requestBody = new LinkedMultiValueMap<>();
//        requestBody.add("client_id", isceiClient.getClientRegistration().getClientId());
//        requestBody.add("client_secret", isceiClient.getClientRegistration().getClientSecret());
//        
////        requestBody.add("login_hint", username);//the username of the user in the keycloak extracted from the RUEI
////        requestBody.add("binding_message", UUID.randomUUID().toString());
////        requestBody.add("scope", "email ldap_data");
////        requestBody.add("acr_values", levelOfAssurance.name());        
////        
////		//registering authentication request
////        var authenticationRequestId =  WebClient
////        .builder()
////        .build()
////        .post()
////        .uri("https://mvreid-keycloak.local:8443/realms/dev/protocol/openid-connect/ext/ciba/auth")
//////        .header(HttpHeaders.CONTENT_TYPE, MediaType.APPLICATION_FORM_URLENCODED_VALUE)
//////        .attributes(ServerOAuth2AuthorizedClientExchangeFilterFunction.oauth2AuthorizedClient(isceiClient))
////        .contentType(MediaType.APPLICATION_FORM_URLENCODED)
////        .bodyValue(requestBody)
////        .retrieve()
////        .bodyToMono(Object.class) ;
////
////
////		//getting the authentication request
////		AuthApprovalRequest authRequest = this.authApprovalRequestRepository.findById(authenticationRequestId).get();
////
////		//approving the authentication request
////		var approvalResponse = WebClient
////		.builder()
////		.build()
////		.post()
////		.uri("https://mvreid-keycloak.local:8443/realms/dev/protocol/openid-connect/ext/ciba/auth/callback")
////		.header(HttpHeaders.AUTHORIZATION, authRequest.getRelyPartyToken())
//////			        .attributes(ServerOAuth2AuthorizedClientExchangeFilterFunction.oauth2AuthorizedClient(isceiClient))
////		.contentType(MediaType.APPLICATION_JSON)
////		.bodyValue(this.objectMapper.writeValueAsString(Map.entry("status", "APPROVED")))
////		.retrieve()
////		.bodyToMono(String.class)
////		;
////
////
////		
////		MultiValueMap<String, String> bodyValues = new LinkedMultiValueMap<>();
////        bodyValues.add("client_id", isceiClient.getClientRegistration().getClientId());
////        bodyValues.add("client_secret", isceiClient.getClientRegistration().getClientSecret());
////        bodyValues.add("grant_type", "urn:openid:params:grant-type:ciba");
////        bodyValues.add("auth_req_id", authenticationRequestId);
////        
////		//getting the token and returning it to the citizen
////	   return WebClient
////        .builder()
////        .build()
////        .post()
////        .uri("https://mvreid-keycloak.local:8443/realms/dev/protocol/openid-connect/token")
////        .header(HttpHeaders.AUTHORIZATION, authRequest.getRelyPartyToken())
//////        .attributes(ServerOAuth2AuthorizedClientExchangeFilterFunction.oauth2AuthorizedClient(isceiClient))
////        .contentType(MediaType.APPLICATION_FORM_URLENCODED)
////        .bodyValue(bodyValues)
////        .retrieve()
////        .bodyToMono(String.class)
////        ;

}
