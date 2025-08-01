package bg.bulsi.mvr.mpozei.backend.dto;

import bg.bulsi.mvr.mpozei.contract.dto.IdentifierType;
import lombok.Data;

import java.util.UUID;

@Data
public class EidentityDTO {
    private UUID id;
    private Boolean active;
    private String firstName;
    private String secondName;
    private String lastName;
    private String citizenIdentifierNumber;
    private IdentifierType citizenIdentifierType;
}
