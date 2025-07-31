package bg.bulsi.mvr.mpozei.contract.dto;

import bg.bulsi.mvr.common.util.RegexUtil;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Pattern;
import lombok.Data;
import lombok.EqualsAndHashCode;

import java.io.Serial;
import java.io.Serializable;
import java.util.UUID;

import bg.bulsi.mvr.common.exception.ErrorCode;

@EqualsAndHashCode
@Data
public class PersoCentreApplicationRequest implements Serializable {
    @Serial
    private static final long serialVersionUID = 5554805573686813945L;

    @NotNull(message = ErrorCode.Fields.DEVICE_ID_CANNOT_BE_NULL)
    private UUID deviceId;

    @NotNull(message = ErrorCode.Fields.EID_ADMINISTRATOR_ID_CANNOT_BE_NULL)
    private UUID eidAdministratorId;

    @NotNull(message = ErrorCode.Fields.ADMINISTRATOR_FRONT_OFFICE_ID_CANNOT_BE_NULL)
    private UUID eidAdministratorOfficeId;

    @NotBlank(message = ErrorCode.Fields.CERTICATE_SIGNING_REQUEST_CANNOT_BE_BLANK)
    private String certificateSigningRequest;

    @NotBlank(message = ErrorCode.Fields.CERTICATE_CA_NAME_CANNOT_BE_BLANK)
    private String certificateAuthorityName;

    @NotBlank(message = ErrorCode.Fields.FIRST_NAME_CANNOT_BE_BLANK)
    @Pattern(regexp = RegexUtil.FIRST_OR_SECOND_NAME_REGEX, message = ErrorCode.Fields.FIRST_NAME_NOT_VALID)
    private String firstName;

    @Pattern(regexp =  RegexUtil.FIRST_OR_SECOND_NAME_REGEX, message = ErrorCode.Fields.SECOND_NAME_NOT_VALID)
    private String secondName;

    @Pattern(regexp = RegexUtil.LAST_NAME_REGEX, message = ErrorCode.Fields.LAST_NAME_NOT_VALID)
    private String lastName;

    @NotBlank(message = ErrorCode.Fields.FIRST_NAME_LATIN_CANNOT_BE_BLANK)
    @Pattern(regexp = RegexUtil.FIRST_OR_SECOND_NAME_LATIN_REGEX, message = ErrorCode.Fields.FIRST_NAME_LATIN_NOT_VALID)
    private String firstNameLatin;

    @Pattern(regexp = RegexUtil.FIRST_OR_SECOND_NAME_LATIN_REGEX,message = ErrorCode.Fields.SECOND_NAME_LATIN_NOT_VALID)
    private String secondNameLatin;

    @Pattern(regexp = RegexUtil.LAST_NAME_LATIN_REGEX, message = ErrorCode.Fields.LAST_NAME_LATIN_NOT_VALID)
    private String lastNameLatin;

    @NotNull(message = ErrorCode.Fields.APPLICATION_TYPE_CANNOT_BE_NULL)
    private ApplicationType applicationType;

    @NotBlank(message = ErrorCode.Fields.CITIZENSHIP_CANNOT_BE_NULL)
    private String citizenship;

    @NotBlank(message =ErrorCode.Fields.CITIZEN_IDENTIFIER_NUMBER_CANNOT_BE_NULL)
    private String citizenIdentifierNumber;

    @NotNull(message = ErrorCode.Fields.PERSONAL_IDENTITY_DOCUMENT_CANNOT_BE_NULL)
    private PersonalIdentityDocumentV2 personalIdentityDocument;
    
    private NaifIdentifierType personalIdentifierType;

    @NotBlank(message = ErrorCode.Fields.NAIF_APPLICATION_NR_CANNOT_BE_NULL)
    private String numForm;
    
}
