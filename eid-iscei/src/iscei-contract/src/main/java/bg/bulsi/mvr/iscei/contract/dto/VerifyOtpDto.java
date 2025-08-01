package bg.bulsi.mvr.iscei.contract.dto;

import java.util.UUID;

import bg.bulsi.mvr.common.exception.ErrorCode;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import lombok.Data;
import lombok.EqualsAndHashCode;
import lombok.ToString;

@Data
public class VerifyOtpDto {
	
    @NotNull
	private UUID sessionId;
	
    @ToString.Exclude
    @EqualsAndHashCode.Exclude
    @NotBlank(message = ErrorCode.Fields.OTP_CODE_IS_NOT_VALID)
	private String otp;
}
