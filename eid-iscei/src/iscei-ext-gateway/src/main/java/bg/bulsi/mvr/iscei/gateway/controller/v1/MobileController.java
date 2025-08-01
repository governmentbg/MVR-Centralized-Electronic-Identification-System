package bg.bulsi.mvr.iscei.gateway.controller.v1;

import java.util.Objects;
import java.util.UUID;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.security.oauth2.client.OAuth2ClientProperties.Provider;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.security.oauth2.core.AuthorizationGrantType;
import org.springframework.util.LinkedMultiValueMap;
import org.springframework.util.MultiValueMap;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.reactive.function.client.WebClient;
import org.springframework.web.util.UriComponentsBuilder;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.dto.AuditData;
import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.audit_logger.model.MessageType;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.iscei.contract.dto.CertificateLoginResultDto;
import bg.bulsi.mvr.iscei.contract.dto.CitizenProfileDTO;
import bg.bulsi.mvr.iscei.contract.dto.MobileSignedChallenge;
import bg.bulsi.mvr.iscei.gateway.client.RueiClient;
import bg.bulsi.mvr.iscei.gateway.service.X509CertificateLogin;
import bg.bulsi.mvr.iscei.model.AcrScope;
import bg.bulsi.mvr.iscei.model.service.AuthenticationStatisticsService;
import bg.bulsi.mvr.iscei.pan.EventRegistratorImpl;
import bg.bulsi.mvr.pan_client.EventRegistrator;
import bg.bulsi.mvr.pan_client.NotificationSender;
import bg.bulsi.mvr.pan_client.EventRegistrator.Event;
import jakarta.servlet.http.HttpServletRequest;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@RestController
@RequestMapping("/api/v1/auth")
public class MobileController {
	
	@Autowired
	private X509CertificateLogin x509CertificateLogin;
	
	@Autowired
	private Provider keycloakExternal;
	
	@Autowired
	private RueiClient rueiClient;
	
	@Autowired
	private BaseAuditLogger auditLogger;
	
	@Autowired
	private NotificationSender notificationSender;

	@Autowired
	private AuthenticationStatisticsService statisticsService;
	
    private Event authEvent;
	
	public MobileController(EventRegistrator eventRegistrator) {
		this.authEvent = eventRegistrator.getEvent(EventRegistratorImpl.ISCEI_SUCCESSFUL_AUTHENTICATION_WITH_EID);
	}
	
	@PostMapping(path = "/mobile/certificate-login",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public ResponseEntity<String> mobileX509CertificateLogin(
			@RequestBody MobileSignedChallenge mobileSignedChallenge,
			HttpServletRequest httpRequest
			//@RegisteredOAuth2AuthorizedClient(WebSecurityConfig.ISCEI_CLIENT_REGISTRATION_ID) OAuth2AuthorizedClient isceiClient
			) {

		log.info(".mobileX509CertificateLogin() [mobileSignedChallenge={}]", mobileSignedChallenge);
		
		UserContext userContext = UserContextHolder.getFromServletContext();
		httpRequest.setAttribute(AuditEventType.AUDIT_EVENT_TYPE_KEY, AuditEventType.AUTH_MOBILE_X509_CERTIFICATE);
		
		CertificateLoginResultDto certificateLoginResult = this.x509CertificateLogin.validateX509CertificateLogin(mobileSignedChallenge.getSignedChallenge());
		
		CitizenProfileDTO citizenProfile = this.rueiClient.getCitizenProfileByEidentityId(certificateLoginResult.getEidentityId());
		String citizenProfileId = citizenProfile.getId().toString();
		String clientId = mobileSignedChallenge.getClientId();
		
		EventPayload eventPayloadReq = new EventPayload();
		eventPayloadReq.setRequesterUid(certificateLoginResult.getCitizenIdentifier());
		eventPayloadReq.setRequesterUidType(certificateLoginResult.getCitizenIdentifierType().name());
		eventPayloadReq.setTargetUid(certificateLoginResult.getCitizenIdentifier());
		eventPayloadReq.setTargetUidType(certificateLoginResult.getCitizenIdentifierType().name());
		
		AuditData reqAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(AuditEventType.AUTH_MOBILE_X509_CERTIFICATE)
				.messageType(MessageType.REQUEST)
				.payload(eventPayloadReq)
				.requesterUserId(citizenProfileId)
				.requesterSystemId(clientId)
				.requesterSystemName(clientId)
				.targetUserId(citizenProfileId)
				.build();
		
		this.auditLogger.auditEvent(reqAuditEvent);	
		
		this.statisticsService.createAuthenticationRequestEid(mobileSignedChallenge.getClientId(), citizenProfile.getId(), null, certificateLoginResult, httpRequest);
		
		String state = UUID.randomUUID().toString();

		MultiValueMap<String, String> requestBodyAuth = new LinkedMultiValueMap<>();
		requestBodyAuth.add("username", citizenProfile.getId().toString());

		requestBodyAuth.add("client_id", mobileSignedChallenge.getClientId());
		requestBodyAuth.add("scope", AcrScope.EID_LOA_HIGH.getAcrScopeName());
		requestBodyAuth.add("state", state);
		requestBodyAuth.add("redirect_uri", "http://localhost:8889/index");
		requestBodyAuth.add("response_type", "code");

		//TODO; not need for Code Auth flow, use the same logic as PG
		// registering authentication request in keycloak
		var authResponse = WebClient.builder().build().post()
		        .uri(keycloakExternal.getAuthorizationUri())
				.contentType(MediaType.APPLICATION_FORM_URLENCODED).bodyValue(requestBodyAuth).retrieve()
				.toEntity(String.class)
				.block();

		String locationHeader = authResponse.getHeaders().get("Location").get(0);

		var uriComponents = UriComponentsBuilder.fromUriString(locationHeader).build();
		String code = uriComponents.getQueryParams().get("code").get(0);

		MultiValueMap<String, String> requestBodyToken = new LinkedMultiValueMap<>();
		requestBodyToken.add("client_id", mobileSignedChallenge.getClientId());
		requestBodyToken.add("state", state);
		requestBodyToken.add("redirect_uri", "http://localhost:8889/index");
		requestBodyToken.add("grant_type", AuthorizationGrantType.AUTHORIZATION_CODE.getValue());
		requestBodyToken.add("code", code);

		ResponseEntity<String> response = WebClient.builder().build().post()
		        .uri(keycloakExternal.getTokenUri())
				.contentType(MediaType.APPLICATION_FORM_URLENCODED).bodyValue(requestBodyToken).retrieve()
				.toEntity(String.class)
				.block();
		
		AuditData respAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(AuditEventType.AUTH_MOBILE_X509_CERTIFICATE)
				.messageType(MessageType.SUCCESS)
				.payload(eventPayloadReq)
				.requesterUserId(citizenProfileId)
				.requesterSystemId(clientId)
				.requesterSystemName(clientId)
				.targetUserId(citizenProfileId)
				.build();
		
		this.auditLogger.auditEvent(respAuditEvent);	
		
		this.statisticsService.createAuthenticationResultEid(mobileSignedChallenge.getClientId(), citizenProfile.getId(), null, certificateLoginResult, httpRequest);
		
		this.notificationSender.send(authEvent.code(), citizenProfile.getEidentityId());
		
		return response;
	}
}
