package bg.bulsi.mvr.iscei.contract.dto.approvalrequest;

import lombok.Data;

@Data
public class ApprovalRequestAuthResponse {

	private String auth_req_id;
	
	private long expires_in;
	
	private int interval;
}
