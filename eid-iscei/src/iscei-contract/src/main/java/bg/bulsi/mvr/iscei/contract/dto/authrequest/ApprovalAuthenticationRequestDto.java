package bg.bulsi.mvr.iscei.contract.dto.authrequest;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.util.RegexUtil;
import bg.bulsi.mvr.iscei.contract.dto.IdentifierTypeDTO;
import bg.bulsi.mvr.iscei.contract.dto.LevelOfAssurance;
import bg.bulsi.mvr.iscei.contract.dto.RequestFromDto;
import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Pattern;
import lombok.Data;

/**
 * Initial request for approval authentication from the frontend
 */
@Valid
@Data
public class ApprovalAuthenticationRequestDto extends AuthenticationRequestDto {

    @NotBlank(message =ErrorCode.Fields.CITIZEN_IDENTIFIER_NUMBER_CANNOT_BE_NULL)
	private String citizenNumber;
    @NotNull(message = ErrorCode.Fields.CITIZEN_IDENTIFIER_TYPE_CANNOT_BE_NULL)
	private IdentifierTypeDTO type;

    @Pattern(regexp = RegexUtil.PHONE_NUMBER_REGEX, message = ErrorCode.Fields.PHONE_NUMBER_NOT_VALID) 
    private String phoneNumber;
    
    //example: НАП, портал за граждани, ЕАФТ
	private RequestFromDto requestFrom;
	private LevelOfAssurance levelOfAssurance;
}
