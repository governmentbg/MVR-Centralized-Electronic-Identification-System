package bg.bulsi.mvr.iscei.gateway.service;

import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.security.NoSuchProviderException;
import java.security.cert.CertificateException;
import java.security.cert.CertificateExpiredException;
import java.security.cert.CertificateNotYetValidException;
import java.security.cert.X509Certificate;
import java.util.Base64;
import java.util.Optional;
import java.util.UUID;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.iscei.contract.dto.CertificateLoginResultDto;
import bg.bulsi.mvr.iscei.contract.dto.CertificateStatusDTO;
import bg.bulsi.mvr.iscei.contract.dto.CitizenCertificateDetailsDTO;
import bg.bulsi.mvr.iscei.contract.dto.EidentityDTO;
import bg.bulsi.mvr.iscei.contract.dto.SignedChallenge;
import bg.bulsi.mvr.iscei.gateway.client.ReiClient;
import bg.bulsi.mvr.iscei.gateway.client.RueiClient;
import bg.bulsi.mvr.iscei.model.AuthenticationChallenge;
import bg.bulsi.mvr.iscei.model.repository.keypair.AuthChallengeRepository;
import bg.bulsi.mvr.iscei.pan.EventRegistratorImpl;
import bg.bulsi.mvr.pan_client.EventRegistrator;
import bg.bulsi.mvr.pan_client.NotificationSender;
import bg.bulsi.mvr.pan_client.EventRegistrator.Event;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Service
public class X509CertificateLogin {

	@Autowired
	private CertificateProcessor certificateProcessor;
	
	@Autowired
	private AuthChallengeRepository authChallengeRepository;
	
	@Autowired
	private ReiClient reiClient;
	
	@Autowired
	private RueiClient rueiClient;
	
	@Autowired
	private NotificationSender notificationSender;
	
    private Event eidAttemptAuthEvent;
    
	public X509CertificateLogin(EventRegistrator eventRegistrator) {
		this.eidAttemptAuthEvent = eventRegistrator.getEvent(EventRegistratorImpl.ISCEI_ATTEMPT_AUTHENTICATION_WITH_EID);
	}
	
//	public ResponseEntity<String> executeAuthEndpoint(String username,String redirectUri, OAuth2AuthorizedClient isceiClient) {
//		String state = UUID.randomUUID().toString();
//		
//        MultiValueMap<String, String> requestBodyAuth = new LinkedMultiValueMap<>();
//        requestBodyAuth.add("username", username);
//        
//        requestBodyAuth.add("client_id", isceiClient.getClientRegistration().getClientId());
//        requestBodyAuth.add("client_secret", isceiClient.getClientRegistration().getClientSecret());
//        requestBodyAuth.add("state", state);
//        requestBodyAuth.add("redirect_uri", redirectUri);
//        //requestBodyAuth.add("redirect_uri", "http://localhost:8889/index");
//        requestBodyAuth.add("response_type", OAuth2AuthorizationResponseType.CODE.getValue());
//        
//        //TODO: Check if we need to return all Keycloak headers to frontend
//		//registering authentication request in keycloak		
//        var authResponse = WebClient
//        .builder()
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
//        .bodyValue(requestBodyAuth)
//        .retrieve()
//        .toEntity(String.class)
//        .doOnSuccess(c-> {
//        	System.out.println("============> c.getHeaders() = " + c.getHeaders());	
//        })
//        .block();
//        
//        return authResponse;
//	}
//	
//	public ResponseEntity<String> executeTokenEndpoint(String state, String code, String redirectUri, OAuth2AuthorizedClient isceiClient) {
//		
//        MultiValueMap<String, String> requestBodyToken = new LinkedMultiValueMap<>();
//        requestBodyToken.add("client_id", isceiClient.getClientRegistration().getClientId());
//        requestBodyToken.add("client_secret", isceiClient.getClientRegistration().getClientSecret());
//        requestBodyToken.add("state", state);
//        requestBodyToken.add("redirect_uri", redirectUri);
//        //requestBodyToken.add("redirect_uri", "http://localhost:8889/index");
//        requestBodyToken.add("grant_type", AuthorizationGrantType.AUTHORIZATION_CODE.getValue());
//        requestBodyToken.add("code", code);
//
//        return WebClient
//        .builder()
//        .build()
//        .post()
//        .uri(builder -> builder
//        		.scheme("https")
//        		.host("mvreid-keycloak.local")
//        		.path("/realms/dev/protocol/openid-connect/token")
//        		.port("8443")
//        		.build()
//        		)
//        .contentType(MediaType.APPLICATION_FORM_URLENCODED)
//        .bodyValue(requestBodyToken)
//        .retrieve()
//        .toEntity(String.class)
//        .block();
//	}
	/**
	 * <p>This method verifies the {@link SignedChallenge} against the following checks:
     * <ul>
     * 	 <li>Challenge is registered in the system</li>
	 *   <li>Structure of the {@link X509Certificate}</li>
	 *   <li>Validated the Certificate Chain and OCSP/CRL</li>
	 *   <li>Certificate validity - if the current date and time are within the validity period given in the certificate</li>
	 *   <li>Signature validity - verify that Signature corresponds to challenge using the provided {@link X509Certificate}</li>
	 *   <li>The provided {@link X509Certificate} is the same as the one from RUEI</li>
	 *   <li>{@link X509Certificate} is ACTIVE - call to RUEI</li>
	 * </ul>
	 */
	public CertificateLoginResultDto validateX509CertificateLogin(SignedChallenge signedChallenge) {
		UUID authChallengeId = UUID.fromString(signedChallenge.getChallenge());
	    Optional<AuthenticationChallenge> authenticationRequest = this.authChallengeRepository.findById(authChallengeId);
	    if(authenticationRequest.isEmpty()) {
        	throw new ValidationMVRException(ErrorCode.AUTHENTICATION_REQUEST_NOT_FOUND);
        } else {
    		this.authChallengeRepository.deleteById(authChallengeId);
        }
        
      //extracting the public certificate from the {@link SignedChallenge}
        X509Certificate x509Certificate = null;
	    try {
			x509Certificate = this.certificateProcessor.extractCertificate(signedChallenge.getCertificate().getBytes());
		} catch (CertificateException | IOException | NoSuchProviderException e) {
			log.error(".validateX509CertificateLogin() Error extracting X509 Certificate ", e);
			
			throw new ValidationMVRException(ErrorCode.CERTIFICATE_IS_NOT_VALID);
		}
        
	    //Extract EidentityId from {@link X509Certificate} 
        UUID eid = UUID.fromString(this.certificateProcessor.getSubjectEid(x509Certificate));
		this.notificationSender.send(eidAttemptAuthEvent.code(), eid);

		log.info(".validateX509CertificateLogin() [eid={}]", eid);
	    
		//Will throws exception is eidentity is not found or  eidentity is not active
		EidentityDTO eidentityDTO = this.reiClient.getEidentityById(eid);
		
		//Validate Certificate Chain and OCSP/CRL
		try{
			this.certificateProcessor.validateCertificateChain(eid, x509Certificate, signedChallenge.getCertificateChain());
		} catch(Exception e) {
			log.error(".validateX509CertificateLogin() Unable to validate X509 Certificate Chain and OSCP/CRL", e);

			throw new ValidationMVRException(ErrorCode.CERTIFICATE_CHAIN_GENERAL_FAILURE);
		}
		
	    //CERTIFICATE_IS_EXPIRED_OR_INVALID
	    try {
			x509Certificate.checkValidity();
		} catch (CertificateExpiredException | CertificateNotYetValidException e) {
			log.error(".validateX509CertificateLogin() X509 Certificate is expired or invalid ", e);
			
			throw new ValidationMVRException(ErrorCode.CERTIFICATE_IS_EXPIRED_OR_INVALID);
		}
	    
		//validate that the Signature is signed properly
        boolean isSignatureValid = this.certificateProcessor.verifySignature(
        		signedChallenge.getChallenge().getBytes(StandardCharsets.UTF_8),
        		Base64.getDecoder().decode(signedChallenge.getSignature().getBytes(StandardCharsets.UTF_8)), 
        		x509Certificate);

        log.info(".validateX509CertificateLogin() [isSignatureValid={}]", isSignatureValid);

        if(!isSignatureValid) {
        	throw new ValidationMVRException(ErrorCode.SIGNATURE_DOES_NOT_MATCH);
        }
        
		String issuerDn = x509Certificate.getIssuerX500Principal().getName();
		String serialNumber = x509Certificate.getSerialNumber().toString();
		
		//RUEI will throw exception if certificate is missing
		CitizenCertificateDetailsDTO citizenCertificateDetailsDTO = rueiClient.getCitizenCertificateByIssuerAndSN(issuerDn, serialNumber);
		
        log.info(".validateX509CertificateLogin() [citizenCertificateDetailsDTO={}]", citizenCertificateDetailsDTO);
		
		X509Certificate rueiX509Certificate = null;
		try {
		   rueiX509Certificate = this.certificateProcessor.extractCertificate(citizenCertificateDetailsDTO.getCertificate().getBytes());
		} catch (Exception e) {
			log.error(".validateX509CertificateLogin() Error extracting X509 Certificate from RUEI ", e);
			
			throw new ValidationMVRException(ErrorCode.CERTIFICATE_IS_NOT_VALID);
		}
		
		boolean areCertsSame = this.certificateProcessor.compareCertificate(x509Certificate, rueiX509Certificate);
        if(!areCertsSame) {
        	throw new ValidationMVRException(ErrorCode.CERTIFICATES_DO_NOT_MATCH);
        }
        
        if(citizenCertificateDetailsDTO.getStatus() != CertificateStatusDTO.ACTIVE) {
        	throw new ValidationMVRException(ErrorCode.CERTIFICATE_IS_NOT_ACTIVE);
        }
       
        return CertificateLoginResultDto.builder()
        		.id(citizenCertificateDetailsDTO.getId())
        		.eidentityId(eid)
        		.citizenIdentifier(eidentityDTO.getCitizenIdentifierNumber())
        		.citizenIdentifierType(eidentityDTO.getCitizenIdentifierType())
        		.x509CertificateSn(serialNumber)
        		.x509CertificateIssuerDn(issuerDn)
        		.levelOfAssurance(citizenCertificateDetailsDTO.getLevelOfAssurance())
        		.deviceId(citizenCertificateDetailsDTO.getDeviceId())
        		.build();
	}
}
