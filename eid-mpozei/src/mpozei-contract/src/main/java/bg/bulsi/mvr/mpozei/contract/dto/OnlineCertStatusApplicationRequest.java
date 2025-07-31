package bg.bulsi.mvr.mpozei.contract.dto;

import jakarta.validation.constraints.NotNull;
import lombok.Data;
import lombok.EqualsAndHashCode;

import java.io.Serial;
import java.util.UUID;

import bg.bulsi.mvr.common.exception.ErrorCode;

@Data
@EqualsAndHashCode(callSuper = true)
public class OnlineCertStatusApplicationRequest extends BaseApplicationRequest {
    @Serial
    private static final long serialVersionUID = -205817373713077259L;

    private UUID reasonId;

    private String reasonText;

    @NotNull(message = ErrorCode.Fields.CERTIFICATE_ID_CANNOT_BE_NULL)
    private UUID certificateId;
}
