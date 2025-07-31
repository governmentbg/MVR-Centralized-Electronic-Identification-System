package bg.bulsi.mvr.iscei.contract.dto;

import lombok.Data;

import java.util.UUID;

@Data
public class CitizenProfileAttachDTO {
    private String firstName;
    private String secondName;
    private String lastName;
    private UUID eidentityId;
    private UUID citizenProfileId;
	private String citizenIdentifierNumber;
	private IdentifierTypeDTO citizenIdentifierType;
}
