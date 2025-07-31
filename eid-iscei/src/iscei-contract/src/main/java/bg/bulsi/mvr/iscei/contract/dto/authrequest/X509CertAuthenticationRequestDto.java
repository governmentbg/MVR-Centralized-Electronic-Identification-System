package bg.bulsi.mvr.iscei.contract.dto.authrequest;

import bg.bulsi.mvr.iscei.contract.dto.LevelOfAssurance;
import lombok.Data;

/**
 * Initial request for authentication from the frontend
 */
@Data
public class X509CertAuthenticationRequestDto extends AuthenticationRequestDto {

	//example: НАП, портал за граждани, ЕАФТ
	private String requestFrom;
	private LevelOfAssurance levelOfAssurance;
}
