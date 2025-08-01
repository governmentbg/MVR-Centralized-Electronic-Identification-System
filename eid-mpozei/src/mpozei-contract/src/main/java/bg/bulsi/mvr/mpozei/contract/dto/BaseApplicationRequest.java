package bg.bulsi.mvr.mpozei.contract.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Pattern;
import jakarta.validation.constraints.Size;
import lombok.Data;

import java.io.Serial;
import java.io.Serializable;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.util.RegexUtil;

@Data
public class BaseApplicationRequest implements Serializable {
    @Serial
    private static final long serialVersionUID = 1L;

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

    @NotNull(message = ErrorCode.Fields.CITIZEN_IDENTIFIER_TYPE_CANNOT_BE_NULL)
    private IdentifierType citizenIdentifierType;

    @NotNull(message = ErrorCode.Fields.PERSONAL_IDENTITY_DOCUMENT_CANNOT_BE_NULL)
    private PersonalIdentityDocument personalIdentityDocument;
    
}
