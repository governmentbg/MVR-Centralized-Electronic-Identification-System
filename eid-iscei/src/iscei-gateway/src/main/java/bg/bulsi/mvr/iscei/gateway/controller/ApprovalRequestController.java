package bg.bulsi.mvr.iscei.gateway.controller;

import java.time.OffsetDateTime;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import bg.bulsi.mvr.iscei.contract.dto.LevelOfAssurance;
import bg.bulsi.mvr.iscei.contract.dto.approvalrequest.RelyPartyRequest;
import bg.bulsi.mvr.iscei.model.AuthApprovalRequest;
import bg.bulsi.mvr.iscei.model.repository.keypair.AuthApprovalRequestRepository;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@RestController
@RequestMapping("/api/v1/approval-request")
public class ApprovalRequestController {
	
	@Autowired
	private AuthApprovalRequestRepository authApprovalRequestRepository;

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
}
