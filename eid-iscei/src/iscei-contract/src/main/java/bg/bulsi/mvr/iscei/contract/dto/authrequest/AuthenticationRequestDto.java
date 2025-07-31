package bg.bulsi.mvr.iscei.contract.dto.authrequest;

import bg.bulsi.mvr.iscei.contract.dto.LevelOfAssurance;
import lombok.Data;

/**
 * Initial request for authentication from the frontend
 */
@Data
public class AuthenticationRequestDto {

	private LevelOfAssurance levelOfAssurance;
}
