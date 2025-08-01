//package bg.bulsi.mvr.iscei.gateway.controller.v2;
//
//import java.io.IOException;
//import java.time.OffsetDateTime;
//import java.util.Collections;
//import java.util.HashSet;
//import java.util.Map;
//import java.util.Optional;
//import java.util.Set;
//import java.util.UUID;
//import java.util.stream.Collectors;
//import java.util.stream.Stream;
//
//import org.springframework.beans.factory.annotation.Autowired;
//import org.springframework.boot.autoconfigure.security.oauth2.client.OAuth2ClientProperties.Provider;
//import org.springframework.http.MediaType;
//import org.springframework.http.ResponseEntity;
//import org.springframework.security.core.Authentication;
//import org.springframework.security.core.context.SecurityContext;
//import org.springframework.security.core.context.SecurityContextHolder;
//import org.springframework.security.core.context.SecurityContextHolderStrategy;
//import org.springframework.security.web.authentication.preauth.PreAuthenticatedAuthenticationToken;
//import org.springframework.security.web.context.HttpSessionSecurityContextRepository;
//import org.springframework.security.web.context.SecurityContextRepository;
//import org.springframework.security.web.savedrequest.RequestCache;
//import org.springframework.security.web.savedrequest.SavedRequest;
//import org.springframework.util.LinkedMultiValueMap;
//import org.springframework.util.MultiValueMap;
//import org.springframework.web.bind.annotation.*;
//import org.springframework.web.reactive.function.client.WebClient;
//import org.springframework.web.servlet.ModelAndView;
//
//import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
//import bg.bulsi.mvr.audit_logger.dto.AuditData;
//import bg.bulsi.mvr.audit_logger.model.AuditEventType;
//import bg.bulsi.mvr.audit_logger.model.MessageType;
//import bg.bulsi.mvr.iscei.gateway.client.RueiClient;
//import bg.bulsi.mvr.iscei.gateway.config.WebSecurityConfig;
//import bg.bulsi.mvr.iscei.gateway.dto.CitizenProfileDTO;
//import bg.bulsi.mvr.iscei.gateway.dto.LevelOfAssurance;
//import bg.bulsi.mvr.iscei.gateway.entity.AcrScope;
//import bg.bulsi.mvr.iscei.gateway.entity.AuthenticationChallenge;
//import bg.bulsi.mvr.iscei.gateway.repository.AuthChallengeRepository;
//import bg.bulsi.mvr.iscei.gateway.service.ProxiedClientService;
//import bg.bulsi.mvr.iscei.gateway.service.X509CertificateLogin;
//import bg.bulsi.mvr.iscei.pan.EventRegistratorImpl;
//import bg.bulsi.mvr.pan_client.EventRegistrator;
//import bg.bulsi.mvr.pan_client.NotificationSender;
//import bg.bulsi.mvr.pan_client.EventRegistrator.Event;
//import jakarta.servlet.http.HttpServletRequest;
//import jakarta.servlet.http.HttpServletResponse;
//import jakarta.servlet.http.HttpSession;
//import lombok.extern.slf4j.Slf4j;
//
///**
// * @see https://docs.spring.io/spring-security/reference/servlet/authentication/session-management.html
// */
//@Slf4j
//@RestController
//@RequestMapping("/api/v2/oauth2")
//public class AuthorizationController {
//
//	private static final String EIDENTITY_ID = "eidentityId";
//	private static final String SCOPE = "scope";
//	
//	private SecurityContextRepository securityContextRepository
//	      =  new HttpSessionSecurityContextRepository(); 
//	
//	@Autowired
//	private X509CertificateLogin x509CertificateLogin;
//	
//	@Autowired
//	private Provider keycloakExternal;
//	
//	@Autowired
//	private RequestCache specificRequestCache;
//	
//	@Autowired
//	private BaseAuditLogger auditLogger;
//	
//	@Autowired
//	private NotificationSender notificationSender;
//	
//	@Autowired
//	private RueiClient rueiClient;
//	
//	@Autowired
//	private ProxiedClientService proxiedClientService;
//	
//	private SecurityContextHolderStrategy securityContextHolderStrategy = SecurityContextHolder
//			.getContextHolderStrategy();
//
//	
//    private Event eidSuccessAuthEvent;
//
//	@Autowired
//	private AuthChallengeRepository authChallengeRepository;
//    
//	public AuthorizationController(EventRegistrator eventRegistrator) {
//		this.eidSuccessAuthEvent = eventRegistrator.getEvent(EventRegistratorImpl.ISCEI_SUCCESSFUL_AUTHENTICATION_WITH_EID);
//	}
//	
//	@GetMapping(path = "/login", produces = MediaType.TEXT_HTML_VALUE)
//	public ModelAndView loginPage(HttpServletRequest request, HttpServletResponse response) {
//	    ModelAndView modelAndView = new ModelAndView();
//	    modelAndView.setViewName("pdeau_login.html");
//	    return modelAndView;
//	}
//	
//	@PostMapping(path = "/login", produces = MediaType.APPLICATION_JSON_VALUE)
//	public String login(@RequestBody PdeauLogin pdeauLogin, HttpServletRequest request, HttpServletResponse response) throws IOException {
//		log.info(".login()");
//		
//		AuthenticationChallenge authenticationChallenge = new AuthenticationChallenge();
//		authenticationChallenge.setExpiration(2400l);
//		authenticationChallenge.setLevelOfAssurance(LevelOfAssurance.HIGH);
//		authenticationChallenge.setRequestFrom("TEST");
//		authenticationChallenge.setId(UUID.fromString(pdeauLogin.getChallenge()));
//		
//		this.authChallengeRepository.save(authenticationChallenge).getId();
//
//		UUID eid = this.x509CertificateLogin.validateX509CertificateLogin(pdeauLogin);
//		
//		request.getSession().setAttribute(EIDENTITY_ID, eid);
////		request.getSession().setAttribute(SCOPE, pdeauLogin.getScope());
//		
//		Authentication authentication = new PreAuthenticatedAuthenticationToken(eid, "Do not show");
//		authentication.setAuthenticated(true);
//
//	    SecurityContext context = securityContextHolderStrategy.createEmptyContext();
//	    context.setAuthentication(authentication); 
//	    securityContextHolderStrategy.setContext(context);
//	    securityContextRepository.saveContext(context, request, response); 
//		
//		SavedRequest savedRequest = specificRequestCache.getRequest(request, response);
//		 if (savedRequest != null) {
//			 log.info(".login() savedRequest != null");
//	            // Redirect to the original URL
//	            response.sendRedirect(savedRequest.getRedirectUrl());
//	        }
//		 else {
//			 log.info(".login() savedRequest == null");
//			 
////	            // Redirect to the default home page if no saved request exists
////	            response.sendRedirect(WebSecurityConfig.v2AuthorizeEndpoint.getPattern());
//	        }
//		
//		return OffsetDateTime.now().toString();
//	}
//	
//  @GetMapping(path = "/authorize", 
//		  produces = MediaType.APPLICATION_JSON_VALUE)
//  public ResponseEntity<String>  authorize(
//		    @RequestParam("client_id") String clientId,
//			@RequestParam("response_type") String responseType,
//			@RequestParam String state,
//			@RequestParam Set<String> scope,
//			@RequestParam("redirect_uri") String redirectUri,
//			@RequestParam("code_challenge") String codeChallenge,
//			@RequestParam("code_challenge_method") String codeChallengeMethod,
//			@RequestParam Map<String, String> allParams,
//			HttpServletRequest httpRequest,
//			HttpServletResponse httpResponse
//		  ) throws IOException {
//		log.info(".authorize() [securityContextRepository={}]", securityContextRepository);
//		
//		if(httpRequest.getSession().getAttribute(EIDENTITY_ID) == null) {
//			httpResponse.sendRedirect("/api/v2/oauth2/login");
//			//return ResponseEntity.badRequest().build();
//		}
//		
//		AuditData reqAuditEvent = AuditData.builder()
//				.correlationId(httpRequest.getRequestId())
//				.eventType(AuditEventType.AUTH_ONLINE_X509_CERTIFICATE)
//				.messageType(MessageType.REQUEST)
//				.payload(null)
//				.requesterSystemId(clientId)
//				.requesterSystemName(clientId)
//				.build();
//		
//		auditLogger.auditEvent(reqAuditEvent);	
//	
//	var securityContext = securityContextRepository.loadDeferredContext(httpRequest);	
//	
//     log.info(".authorize() [securityContextRepository.loadDeferredContext(httpRequest).get()={}]", securityContext.get());
//
//     HttpSession httpSession = httpRequest.getSession();
//     log.info(".authorize() [httpSession={}]", httpSession);
//		
//     //TODO: invalidate session and add timeout
//	  UUID eid = (UUID) httpRequest.getSession().getAttribute(EIDENTITY_ID);
//	  
//	  log.info(".authorize() [eid={}]", eid);
//	  
//		//TODO: have to get if user is active
//	  CitizenProfileDTO citizenProfile = this.rueiClient.getCitizenProfileByEidentityId(eid);
//		
//      MultiValueMap<String, String> requestBody = new LinkedMultiValueMap<>();
//      
//      this.proxiedClientService.evaluateClient(clientId, requestBody);
//      
//      scope =  this.copySet(scope);
//      scope.add(AcrScope.EID_LOA_HIGH.getAcrScopeName());
//      
//      requestBody.add("username", citizenProfile.getId().toString());
//      requestBody.add("client_id", clientId);
////      requestBody.add("client_secret", isceiClient.getClientRegistration().getClientSecret());
//      requestBody.add("state", state);
//      //TODO: which certificate is high vs which is substential
//      log.info(".authorize() this.joinSet(scope) = " + this.joinSet(scope));
//      requestBody.add(SCOPE, this.joinSet(scope));
//      requestBody.add("redirect_uri", redirectUri);
//      requestBody.add("response_type", responseType);
//      requestBody.add("code_challenge", codeChallenge);
//      requestBody.add("code_challenge_method", codeChallengeMethod);
//      
//      for(Map.Entry<String, String> param: allParams.entrySet()) {
//	      if(!requestBody.containsKey(param.getKey())) {
//	    	  requestBody.add(param.getKey(), param.getValue());
//	      }
//      }
//      
//	  log.info(".codeFlowAuth() [requestBody={}]", requestBody);
//      
//      //TODO: Check if we need to return all data (headers and etc) from Keycloak
//		//registering authentication request in keycloak		
//      ResponseEntity<String> response = WebClient
//      .builder()
////      .clientConnector(new ReactorClientHttpConnector(
////              HttpClient.create().followRedirect(true)
////      ))
//      .build()
//      .post()
////      .uri(keycloakExternal.getAuthorizationUri())
//      .uri("https://localhost:8444/realms/master/protocol/openid-connect/auth")
//
////      .header(HttpHeaders.CONTENT_TYPE, MediaType.APPLICATION_FORM_URLENCODED_VALUE)
////      .attributes(ServerOAuth2AuthorizedClientExchangeFilterFunction.oauth2AuthorizedClient(isceiClient))
//      .contentType(MediaType.APPLICATION_FORM_URLENCODED)
//      .accept(MediaType.APPLICATION_JSON)
//      .bodyValue(requestBody)
//      .retrieve()
//      .toEntity(String.class)
//      .block();
//      
//		AuditData respAuditEvent = AuditData.builder()
//				.correlationId(httpRequest.getRequestId())
//				.eventType(AuditEventType.AUTH_ONLINE_X509_CERTIFICATE)
//				.messageType(MessageType.SUCCESS)
//				.payload(null)
//				.requesterSystemId(clientId)
//				.requesterSystemName(clientId)
//				.build();
//		
//		auditLogger.auditEvent(respAuditEvent);	
//      
//      this.notificationSender.send(eidSuccessAuthEvent.code(), eid);
//		
//      return response;
//	}
//	
//	private Set<String> copySet(Set<String> existingSet) {
//		return Optional.ofNullable(existingSet).orElse(Collections.emptySet());
//	}
//	
//	private String joinSet(Set<String> existingSet) {
//		return existingSet.stream()
//                .filter(e -> e != null)
//                .collect(Collectors.joining(" "));
//	}
//	
////    private final OAuth2AuthorizationService authorizationService;
////    private final OAuth2TokenGenerator<OAuth2AccessToken> tokenGenerator;
////
////    public AuthorizationController(OAuth2AuthorizationService authorizationService,
////                                   OAuth2TokenGenerator<OAuth2AccessToken> tokenGenerator) {
////        this.authorizationService = authorizationService;
////        this.tokenGenerator = tokenGenerator;
////    }
////
////    @PostMapping("/authorize")
////    public OAuth2Authorization authorize(@RequestBody OAuth2AuthorizationRequest authorizationRequest) {
////    	
////    	
//////        Authentication authentication = SecurityContextHolder.getContext().getAuthentication();
//////        OAuth2AuthorizationCodeRequestAuthenticationToken authRequest =
//////                new OAuth2AuthorizationCodeRequestAuthenticationToken(
//////                        authorizationRequest.getAuthorizationUri(),
//////                        authorizationRequest.getClientId(),
//////                        authentication,
//////                        authorizationRequest.getScopes(),
//////                        authorizationRequest.getState(),
//////                        authorizationRequest.getRedirectUri());
//////
//////        OAuth2Authorization authorization = OAuth2Authorization.from(authRequest).build();
//////        authorizationService.save(authorization);
////        return null;
////    }
//
////    @PostMapping("/token")
////    public OAuth2AccessToken getToken(@RequestBody OAuth2TokenEndpointAuthenticationToken tokenRequest) {
////        Authentication authentication = SecurityContextHolder.getContext().getAuthentication();
////        OAuth2AccessToken accessToken = tokenGenerator.generate(OAuth2TokenType.ACCESS_TOKEN, authentication);
////        return accessToken;
////    }
//}
