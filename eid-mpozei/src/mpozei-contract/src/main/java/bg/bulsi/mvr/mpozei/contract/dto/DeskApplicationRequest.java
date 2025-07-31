package bg.bulsi.mvr.mpozei.contract.dto;

import jakarta.validation.constraints.NotNull;
import lombok.Data;
import lombok.EqualsAndHashCode;

import java.io.Serial;
import java.util.UUID;

import bg.bulsi.mvr.common.exception.ErrorCode;

@Data
@EqualsAndHashCode(callSuper = true)
public class DeskApplicationRequest extends BaseApplicationRequest {
    @Serial
    private static final long serialVersionUID = 7685005519137714657L;

    @NotNull(message = ErrorCode.Fields.DEVICE_ID_CANNOT_BE_NULL)
    private UUID deviceId;

    private UUID reasonId;
    private String reasonText;
    private UUID certificateId;
}
