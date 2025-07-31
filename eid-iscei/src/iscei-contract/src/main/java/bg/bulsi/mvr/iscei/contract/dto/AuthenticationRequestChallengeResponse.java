package bg.bulsi.mvr.iscei.contract.dto;

import lombok.AllArgsConstructor;
import lombok.Data;

@AllArgsConstructor
@Data
public class AuthenticationRequestChallengeResponse {

	private String challenge;
}
