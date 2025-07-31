package bg.bulsi.mvr.iscei.contract.dto.approvalrequest;

import bg.bulsi.mvr.iscei.contract.dto.SignedChallenge;
import lombok.Data;

@Data
public class RequestOutcome {
	
	private SignedChallenge signedChallenge;
	
	private String clientId;
	
	private ApprovalRequestStatus approvalRequestStatus;
}
