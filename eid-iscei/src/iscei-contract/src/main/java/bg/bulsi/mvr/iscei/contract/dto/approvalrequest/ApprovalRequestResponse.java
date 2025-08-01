package bg.bulsi.mvr.iscei.contract.dto.approvalrequest;

import java.time.OffsetDateTime;
import java.util.UUID;

import bg.bulsi.mvr.iscei.contract.dto.LevelOfAssurance;
import bg.bulsi.mvr.iscei.contract.dto.RequestFromDto;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class ApprovalRequestResponse {

	private UUID id;
	
	private String username;
	
	private LevelOfAssurance levelOfAssurance;
	
	private RequestFromDto requestFrom;
	
	private OffsetDateTime createDate;
	
	private Long maxTtl;
	
	private Long expiresIn;
	//private boolean relyPartyCallbackCalled;
	
}
