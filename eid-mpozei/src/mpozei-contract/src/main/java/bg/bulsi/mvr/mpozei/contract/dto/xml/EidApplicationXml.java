

package bg.bulsi.mvr.mpozei.contract.dto.xml;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Pattern;
import jakarta.validation.constraints.Size;
import jakarta.xml.bind.annotation.*;
import jakarta.xml.bind.annotation.adapters.XmlJavaTypeAdapter;
import lombok.Data;

import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;

import bg.bulsi.mvr.common.dto.ExportModel;


@Data
@XmlType
@XmlRootElement(name = "signedDetails")
@XmlAccessorType(XmlAccessType.FIELD)
public class EidApplicationXml implements ExportModel {

    @XmlElement
    @NotBlank(message = "firstName cannot be empty")
    @Size(min = 1, max = 255, message = "First Name must have between 1 and 255 characters")
    @Pattern(regexp = "^([А-Я])([А-Яа-я]+)([-\\s]([А-Яа-я]*))*$", message = "First Name must be written in cyrillic and the first letter should be capitalized")
    private String firstName;

    @XmlElement
    @NotBlank(message = "firstNameLatin cannot be empty")
    @Size(min = 1, max = 255, message = "First Name Latin must have between 1 and 255 characters")
    @Pattern(regexp = "^([A-Z])([A-Za-z]+)([-\\s]([A-Za-z]*))*$", message = "First Name Latin must be written in latin and the first letter should be capitalized")
    private String firstNameLatin;

    @XmlElement
    @NotBlank(message = "secondName cannot be empty")
    @Size(min = 1, max = 255, message = "Second Name must have between 1 and 255 characters")
    @Pattern(regexp = "^([А-Яа-я]+)([-\\s]([А-Яа-я]*))*$", message = "Second Name must be written in cyrillic and the first letter should be capitalized")
    private String secondName;

    @XmlElement
    @NotBlank(message = "secondNameLatin cannot be empty")
    @Size(min = 1, max = 255, message = "Second Name Latin must have between 1 and 255 characters")
    @Pattern(regexp = "^([A-Za-z]+)([-\\s]([A-Za-z]*))*$", message = "Second Name Latin must be written in latin and the first letter should be capitalized")
    private String secondNameLatin;

    @XmlElement
    @NotBlank(message = "lastName cannot be empty")
    @Size(min = 1, max = 255, message = "Last Name must have between 1 and 255 characters")
    @Pattern(regexp = "^([А-Яа-я]+)([-\\s]([А-Яа-я]*))*$", message = "Last Name must be written in cyrillic and the first letter should be capitalized")
    private String lastName;

    @XmlElement
    @NotBlank(message = "lastNameLatin cannot be empty")
    @Size(min = 1, max = 255, message = "Last Name Latin must have between 1 and 255 characters")
    @Pattern(regexp = "^([A-Za-z]+)([-\\s]([A-Za-z]*))*$", message = "Last Name Latin must be written in latin and the first letter should be capitalized")
    private String lastNameLatin;

    @XmlElement
    @NotBlank(message = "Citizen Identifier Number cannot be empty")
    private String citizenIdentifierNumber;

    @XmlElement
    @NotBlank(message = "Citizen Identifier Type cannot be empty")
    private String citizenIdentifierType;

    @XmlElement
    private String createDate;

    @XmlElement
    @NotBlank(message = "Citizenship cannot be empty")
    private String citizenship;

    @XmlElement
    @NotBlank(message = "Identity Number cannot be blank")
    private String identityNumber;

    @XmlElement
    @NotBlank(message = "Identity Type cannot be blank")
    private String identityType;

    @XmlElement
    @NotBlank(message = "Identity Issue Date cannot be blank")
    private String identityIssueDate;

    @XmlElement
    @NotBlank(message = "identityValidityToDate cannot be blank")
    private String identityValidityToDate;

    @XmlElement
    @NotBlank(message = "Eid Administrator Office Id cannot be empty")
    private String eidAdministratorOfficeId;

    @XmlElement
    @NotBlank(message = "Eid Administrator Id cannot be empty")
    private String eidAdministratorId;

    @XmlElement
    @NotBlank(message = "Application Id cannot be empty")
    private String applicationId;

    @XmlElement
    private String email;

    @XmlElement
    private String phoneNumber;

    @XmlElement
    @NotNull(message = "Device Id cannot be null")
    private UUID deviceId;

    @XmlElement
    @NotBlank(message = "Application Type cannot be empty")
    private String applicationType;

    @XmlElement
    @NotBlank(message = "Date Of Birth cannot be empty")
    private String dateOfBirth;

    @XmlElement
    @NotBlank(message = "Citizen Profile Id cannot be empty")
    private String citizenProfileId;

    @XmlElement
    private String eidentityId;

    @XmlElement
    private String reasonId;

    @XmlElement
    private String reasonText;

    @XmlElement
    private String certificateId;

    @XmlElement
    @NotBlank(message = "Level Of Assurance cannot be empty")
    private String levelOfAssurance;

    private List<GuardianDetailsXml> guardians;
}
