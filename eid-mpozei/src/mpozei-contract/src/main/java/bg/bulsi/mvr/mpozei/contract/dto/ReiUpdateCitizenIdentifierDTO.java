package bg.bulsi.mvr.mpozei.contract.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class ReiUpdateCitizenIdentifierDTO {
    private String oldCitizenIdentifierNumber;
    private IdentifierType oldCitizenIdentifierType;
    private String newCitizenIdentifierNumber;
    private IdentifierType newCitizenIdentifierType;
}
