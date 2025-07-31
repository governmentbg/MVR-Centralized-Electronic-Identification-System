package bg.bulsi.mvr.iscei.contract.dto;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.util.RegexUtil;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Pattern;
import lombok.Data;
import lombok.EqualsAndHashCode;
import lombok.ToString;

@Data
public class BasicLoginRequestDto {

	private String client_id;
	
    @NotBlank(message = ErrorCode.Fields.EMAIL_CANNOT_BE_BLANK)
    @Pattern(regexp = RegexUtil.EMAIL_REGEX, message = ErrorCode.Fields.EMAIL_NOT_VALID) 
	private String email;
	
    @ToString.Exclude
    @EqualsAndHashCode.Exclude
    @NotBlank(message = ErrorCode.Fields.PASSWORD_CANNOT_BE_BLANK)
//    @Pattern(regexp = RegexUtil.PASSWORD_REGEX, message = ErrorCode.Fields.PASSWORD_NOT_VALID) 
	private String password;
}
