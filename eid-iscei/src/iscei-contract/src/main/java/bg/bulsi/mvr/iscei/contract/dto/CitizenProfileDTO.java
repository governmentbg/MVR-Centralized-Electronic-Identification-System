package bg.bulsi.mvr.iscei.contract.dto;

import lombok.Data;

import java.util.UUID;

@Data
public class CitizenProfileDTO {
    private UUID id;
    private String firstName;
    private String secondName;
    private String lastName;
    private String firstNameLatin;
    private String secondNameLatin;
    private String lastNameLatin;
    private UUID eidentityId;
    private String citizenIdentifierNumber;
    private IdentifierTypeDTO citizenIdentifierType;
    private String email;
    private String phoneNumber;
    private String mobileApplicationInstanceId;
    private String firebaseId;
    private ProfileStatusDTO status;
    private Boolean is2FaEnabled;
}
