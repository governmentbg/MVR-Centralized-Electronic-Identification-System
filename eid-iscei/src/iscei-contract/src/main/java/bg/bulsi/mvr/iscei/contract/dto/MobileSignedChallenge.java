package bg.bulsi.mvr.iscei.contract.dto;

import com.fasterxml.jackson.annotation.JsonProperty;

import lombok.Data;

@Data
public class MobileSignedChallenge {

	@JsonProperty("client_id")
	private String clientId;

	private SignedChallenge signedChallenge;

}
