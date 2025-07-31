package bg.bulsi.mvr.iscei.contract.dto;

import java.util.List;

import lombok.Data;

@Data
public class SignedChallenge {
	
	//Base64 encoded.
	private String signature;
	
	//private UUID authChallengeId;
	
	//The original challenge that was passed for signing.
	private String challenge;
	
	//Base64 encoded content of X509 Certificate
	private String certificate;
	
	//Base64 encoded content of X509 Certificate
	private List<String> certificateChain;
}
