package bg.bulsi.mvr.mpozei.backend.dto;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class EidentityRequestDTO {
    private String citizenIdentifierNumber;
    private IdentifierTypeDTO citizenIdentifierType;
}
