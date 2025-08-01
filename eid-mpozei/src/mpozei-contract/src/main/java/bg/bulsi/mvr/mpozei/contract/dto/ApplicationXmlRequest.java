package bg.bulsi.mvr.mpozei.contract.dto;

import com.fasterxml.jackson.annotation.JsonFormat;

import bg.bulsi.mvr.common.exception.ErrorCode;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import lombok.Data;
import lombok.EqualsAndHashCode;

import java.io.Serial;
import java.time.LocalDate;
import java.util.UUID;

@Data
@EqualsAndHashCode(callSuper = true)
public class ApplicationXmlRequest extends BaseApplicationRequest {
    @Serial
    private static final long serialVersionUID = -2709812388663732426L;

    @NotNull(message = ErrorCode.Fields.DEVICE_ID_CANNOT_BE_NULL)
    private UUID deviceId;

    @NotNull(message =  ErrorCode.Fields.EID_ADMINISTRATOR_ID_CANNOT_BE_NULL)
    private UUID eidAdministratorId;

    @NotNull(message =  ErrorCode.Fields.ADMINISTRATOR_FRONT_OFFICE_ID_CANNOT_BE_NULL)
    private UUID eidAdministratorOfficeId;

    @NotNull(message =  ErrorCode.Fields.BIRTH_DATE_CANNOT_BE_NULL)
    @JsonFormat(pattern = "yyyy-MM-dd")
    private LocalDate dateOfBirth;
    
    private UUID reasonId;
    
    private String reasonText;
    
    private UUID certificateId;
}