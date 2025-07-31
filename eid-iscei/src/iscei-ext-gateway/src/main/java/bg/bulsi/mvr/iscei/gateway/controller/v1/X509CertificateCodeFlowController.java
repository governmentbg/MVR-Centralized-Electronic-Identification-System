package bg.bulsi.mvr.iscei.gateway.controller.v1;

import java.util.Map;
import java.util.Set;
import java.util.UUID;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.security.oauth2.client.OAuth2ClientProperties.Provider;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.util.LinkedMultiValueMap;
import org.springframework.util.MultiValueMap;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.reactive.function.client.WebClient;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.dto.AuditData;
import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.audit_logger.model.MessageType;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.iscei.contract.dto.CertificateLoginResultDto;
import bg.bulsi.mvr.iscei.contract.dto.CitizenProfileDTO;
import bg.bulsi.mvr.iscei.contract.dto.EidentityDTO;
import bg.bulsi.mvr.iscei.contract.dto.ExternalProviderLoginResultDto;
import bg.bulsi.mvr.iscei.contract.dto.SignedChallenge;
import bg.bulsi.mvr.iscei.contract.dto.SupportedGrantType;
import bg.bulsi.mvr.iscei.gateway.client.ReiClient;
import bg.bulsi.mvr.iscei.gateway.client.RueiClient;
import bg.bulsi.mvr.iscei.gateway.service.X509CertificateLogin;
import bg.bulsi.mvr.iscei.model.AcrScope;
import bg.bulsi.mvr.iscei.model.service.AuthenticationStatisticsService;
import bg.bulsi.mvr.iscei.gateway.service.ExternalProviderLogin;
import bg.bulsi.mvr.iscei.gateway.service.ProxiedClientService;
import bg.bulsi.mvr.iscei.pan.EventRegistratorImpl;
import bg.bulsi.mvr.pan_client.EventRegistrator;
import bg.bulsi.mvr.pan_client.NotificationSender;
import bg.bulsi.mvr.pan_client.EventRegistrator.Event;
import jakarta.servlet.http.HttpServletRequest;
import lombok.extern.slf4j.Slf4j;



/**
 * This only used from the Browser (PG) for Local Card X509 login.
 * Should be standard OAuth 2 FLow
 */
@Slf4j
//@Tag(name = "Products API")
@RestController
@RequestMapping("/api/v1")
public class X509CertificateCodeFlowController extends BaseAuthorizationController {

	@Autowired
	private ReiClient reiClient;
	
	@Autowired
	private RueiClient rueiClient;
	
	@Autowired
	private X509CertificateLogin x509CertificateLogin;
	
	@Autowired
	private ExternalProviderLogin externalProviderLogin;
	
	@Autowired
	private Provider keycloakExternal;
	
	@Autowired
	private ProxiedClientService proxiedClientService;
	
	@Autowired
	private BaseAuditLogger auditLogger;
	
	@Autowired
	private NotificationSender notificationSender;

	@Autowired
	private AuthenticationStatisticsService statisticsService;
	
    private Event eidSuccessAuthEvent;
	
	public X509CertificateCodeFlowController(EventRegistrator eventRegistrator) {
		this.eidSuccessAuthEvent = eventRegistrator.getEvent(EventRegistratorImpl.ISCEI_SUCCESSFUL_AUTHENTICATION_WITH_EID);
	}
	
	@PostMapping(path = "/code-flow/auth",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public ResponseEntity<String> codeFlowAuth(
			@RequestParam("client_id") String clientId,
			@RequestParam("response_type") String responseType,
			@RequestParam String state,
			@RequestParam(required = false, defaultValue = "") Set<String> scope,
			@RequestParam("redirect_uri") String redirectUri,
			@RequestParam("code_challenge") String codeChallenge,
			@RequestParam("code_challenge_method") String codeChallengeMethod,
			@RequestParam(required = false) Map<String, String> allParams,
			@RequestBody SignedChallenge signedChallenge,
			HttpServletRequest httpRequest
			){
	
		log.info(".codeFlowAuth() [signedChallenge={}]", signedChallenge);
		
		UserContext userContext = UserContextHolder.getFromServletContext();
		httpRequest.setAttribute(AuditEventType.AUDIT_EVENT_TYPE_KEY, AuditEventType.AUTH_ONLINE_X509_CERTIFICATE);
		
		this.filterSupportedScopes(scope);
		
		CertificateLoginResultDto certificateLoginResult = this.x509CertificateLogin.validateX509CertificateLogin(signedChallenge);
		
		//TODO: have to get if user is active
		CitizenProfileDTO citizenProfile = this.rueiClient.getCitizenProfileByEidentityId(certificateLoginResult.getEidentityId());
		String citizenProfileId = citizenProfile.getId().toString();
		
		EventPayload eventPayloadReq = new EventPayload();
		eventPayloadReq.setRequesterUid(certificateLoginResult.getCitizenIdentifier());
		eventPayloadReq.setRequesterUidType(certificateLoginResult.getCitizenIdentifierType().name());
		eventPayloadReq.setTargetUid(certificateLoginResult.getCitizenIdentifier());
		eventPayloadReq.setTargetUidType(certificateLoginResult.getCitizenIdentifierType().name());
		
		AuditData reqAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(AuditEventType.AUTH_ONLINE_X509_CERTIFICATE)
				.messageType(MessageType.REQUEST)
				.payload(eventPayloadReq)
				.requesterUserId(citizenProfileId)
				.requesterSystemId(clientId)
				.requesterSystemName(clientId)
				.targetUserId(citizenProfileId)
				.build();
		
		auditLogger.auditEvent(reqAuditEvent);	
		
        MultiValueMap<String, String> requestBody = new LinkedMultiValueMap<>();
        
        ExternalProviderLoginResultDto providerLoginResultDto = this.externalProviderLogin.verifyLogin(certificateLoginResult.getCitizenIdentifier(), certificateLoginResult.getCitizenIdentifierType(), scope, allParams, requestBody);

		this.statisticsService.createAuthenticationRequestEid(clientId, citizenProfile.getId(), providerLoginResultDto, certificateLoginResult, httpRequest);
        
        //TODO: do we need this here
        this.proxiedClientService.evaluateClient(clientId, requestBody);
        
        requestBody.add("username", citizenProfile.getId().toString());
        requestBody.add("client_id", clientId);
        requestBody.add("state", state);
        //TODO: which certificate is high vs which is substential
        scope =  this.copySet(scope);
        //TODO: check for mutually exclusive scopes
        scope.add(AcrScope.EID_LOA_HIGH.getAcrScopeName());
        requestBody.add("scope", this.joinSet(scope));
        requestBody.add("redirect_uri", redirectUri);
        requestBody.add("response_type", responseType);
        requestBody.add("code_challenge", codeChallenge);
        requestBody.add("code_challenge_method", codeChallengeMethod);

		log.info(".codeFlowAuth() [requestBody={}]", requestBody);

        //TODO: Check if we need to return all data (headers and etc) from Keycloak
		//registering authentication request in keycloak		
        ResponseEntity<String> response = WebClient
        .builder()
        .build()
        .post()
        .uri(keycloakExternal.getAuthorizationUri())
        .contentType(MediaType.APPLICATION_FORM_URLENCODED)
        .bodyValue(requestBody)
        .retrieve()
        .toEntity(String.class)
        .block();
        
		AuditData respAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(AuditEventType.AUTH_ONLINE_X509_CERTIFICATE)
				.messageType(MessageType.SUCCESS)
				.payload(eventPayloadReq)
				.requesterUserId(citizenProfileId)
				.requesterSystemId(clientId)
				.requesterSystemName(clientId)
				.targetUserId(citizenProfileId)
				.build();
		
		this.auditLogger.auditEvent(respAuditEvent);	
        
		this.statisticsService.createAuthenticationResultEid(clientId, citizenProfile.getId(), providerLoginResultDto, certificateLoginResult, httpRequest);
		
		this.notificationSender.send(eidSuccessAuthEvent.code(), certificateLoginResult.getEidentityId());
		
        return response;
	}
	
	@GetMapping(path = "/code-flow/token",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public ResponseEntity<String> codeFlowToken(
            @RequestParam("client_id") String clientId,
			@RequestParam("grant_type") SupportedGrantType grantType,
			@RequestParam(value = "code", required = false) String code,
			//@RequestParam String state,
			@RequestParam(value = "redirect_uri", required = false) String redirectUri,
			@RequestParam(value = "code_verifier", required = false) String codeVerifier,
			@RequestParam(value = "refresh_token", required = false) String refreshToken,
			HttpServletRequest httpRequest
			){
	
		log.info(".codeFlowToken()");
		
		UserContext userContext = UserContextHolder.getFromServletContext();
		httpRequest.setAttribute(AuditEventType.AUDIT_EVENT_TYPE_KEY, AuditEventType.AUTH_BASIC);
		
		AuditData reqAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(AuditEventType.AUTH_BASIC)
				.messageType(MessageType.REQUEST)
				.payload(null)
				.requesterSystemId(clientId)
				.requesterSystemName(clientId)
				.build();
		
		auditLogger.auditEvent(reqAuditEvent);	
		
        MultiValueMap<String, String> requestBody = new LinkedMultiValueMap<>();
        
        this.proxiedClientService.evaluateClient(clientId, requestBody);
        
		switch (grantType) {
		case REFRESH_TOKEN: {
	        requestBody.add("refresh_token", refreshToken);
		}
			break;
		case AUTHORIZATION_CODE: {
			requestBody.add("code", code);
            requestBody.add("code_verifier", codeVerifier);
            requestBody.add("redirect_uri", redirectUri);
		}
			break;
		default: {
			throw new ValidationMVRException(ErrorCode.AUTHENTICATION_REQUEST_NOT_FOUND);
		  }
		};

        requestBody.add("client_id", clientId);
        requestBody.add("grant_type", grantType.getValue());
//        requestBody.add("client_secret", isceiClient.getClientRegistration().getClientSecret());
       // requestBody.add("state", state);

		log.info(".codeFlowToken() [requestBody={}]", requestBody);
        
        ResponseEntity<String> response = WebClient
        .builder()
        .build()
        .post()
        .uri(keycloakExternal.getTokenUri())
        .contentType(MediaType.APPLICATION_FORM_URLENCODED)
        .bodyValue(requestBody)
        .retrieve()
        .toEntity(String.class)
        .block();
        
		AuditData respAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(AuditEventType.AUTH_TOKEN)
				.messageType(MessageType.SUCCESS)
				.payload(null)
				.requesterSystemId(clientId)
				.requesterSystemName(clientId)
				.build();
		
		auditLogger.auditEvent(respAuditEvent);	
        
        return response;
	}
	

//	@GetMapping("/auth")
//	public Mono<Void> auth(@RequestParam("scope") List<String> scopes, ServerWebExchange exchange) {
//		System.out.println("====================> /auth" );
//		
//		
//		List<String> commonScopes = new ArrayList<>(scopes);
//		
//		commonScopes.retainAll(loaScopes);
//		
//		ServerHttpRequest request = exchange.getRequest();
//		ServerHttpResponse response = exchange.getResponse();
//		List<String> acr = request.getQueryParams().get("acr_values");
//		//List<String> scopes = Arrays.asList(request.getQueryParams().get("scope").split("\\s*,\\s*")));
//		
//		UriComponentsBuilder location = UriComponentsBuilder
//				.fromUriString("https://mvreid-keycloak.local:8443/realms/dev/protocol/openid-connect/auth")
//				.query(request.getURI().getQuery());
////		if(acr != null && !acr.contains("certificate")) {
////			location.queryParam("scope", "email");
////		}
//
//		String clientCertificate = request.getHeaders().getFirst("SSL_CLIENT_CERT");
//		String authorizationHeader = request.getHeaders().getFirst(HttpHeaders.AUTHORIZATION);
//		
///////////////////////////////////////////////////////////////////////////////////
//		
//		if (commonScopes.size() != 1) {
//			response.setStatusCode(HttpStatus.BAD_REQUEST);
//
//			return response.setComplete();
//		}
//
//		String currentScope = commonScopes.get(0);
//		if (currentScope.equals("eid_low") && clientCertificate != null) {
//			response.setStatusCode(HttpStatus.BAD_REQUEST);
//
//			return response.setComplete();
//		}
//
//		if ((currentScope.equals("eid_high") || currentScope.equals("eid_substantial"))
//				&& authorizationHeader != null) {
//			response.setStatusCode(HttpStatus.BAD_REQUEST);
//
//			return response.setComplete();
//		}
//		
//		X509Certificate certificate = X509CertificateUtil.extractCertificate(clientCertificate);
//		
//		if ((currentScope.equals("eid_high") || currentScope.equals("eid_substantial"))
//				&& authorizationHeader != null) {
//			response.setStatusCode(HttpStatus.BAD_REQUEST);
//
//			return response.setComplete();
//		}
//		
//		String subjectPrinciple = certificate.getSubjectX500Principal().getName();
//		if(currentScope.equals("eid_high") && subjectPrinciple.equals("CN=mvreid.person.2")) {
//			response.setStatusCode(HttpStatus.BAD_REQUEST);
//
//			return response.setComplete();
//		}
//		
//		if(currentScope.equals("eid_substantial") && subjectPrinciple.equals("CN=mvreid.person")) {
//			response.setStatusCode(HttpStatus.BAD_REQUEST);
//
//			return response.setComplete();
//		}
//		
//		System.out.println("====================> /test3 " + subjectPrinciple);
//		
///////////////////////////////////////////////////////////////////////////////////
//		
//        response.setStatusCode(HttpStatus.FOUND);
//		System.out.println("====================> /test3;  redirect URI = " +  request.getURI().getPort());
//		System.out.println("====================> /test3;  Location = " +  ("https://mvreid-keycloak.local:8099/realms/dev/protocol/openid-connect/auth?"+ request.getQueryParams()));
//		
//		
//		System.out.println("====================> /test3;   = " + request.getURI().getFragment());
//		System.out.println("====================> /test3;   = " + request.getURI().getRawQuery());
//		System.out.println("====================> /test3;   = " + request.getURI().getQuery());
//		System.out.println("====================> /test3;   = " + request.getURI());
//
//		//response.getHeaders().setLocation(URI.create("https://mvreid-keycloak.local:8443/realms/dev/protocol/openid-connect/auth?" + request.getURI().getRawQuery()));
//		
//		response.getHeaders().setLocation(URI.create(location.toUriString()));
//        
//        return response.setComplete();
//	}
}
