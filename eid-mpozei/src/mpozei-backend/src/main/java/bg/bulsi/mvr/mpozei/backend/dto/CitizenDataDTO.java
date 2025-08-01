package bg.bulsi.mvr.mpozei.backend.dto;

import lombok.Data;

@Data
public class CitizenDataDTO {
    private String firstName;
    private String secondName;
    private String lastName;
    private String citizenIdentifierNumber;
    private IdentifierTypeDTO citizenIdentifierType;
}
