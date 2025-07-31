package bg.bulsi.mvr.iscei.contract.dto;

import java.util.UUID;

import jakarta.validation.constraints.NotNull;
import lombok.Data;

@Data
public class GenerateOtpDto {

    @NotNull
	private UUID sessionId;
}
