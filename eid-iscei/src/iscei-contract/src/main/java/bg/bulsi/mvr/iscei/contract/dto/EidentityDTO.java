package bg.bulsi.mvr.iscei.contract.dto;

import java.util.UUID;

import lombok.Data;

@Data
public class EidentityDTO {

	private UUID id;

	private Boolean active;

	private String firstName;

	private String secondName;

	private String lastName;

	private String citizenIdentifierNumber;

	private IdentifierTypeDTO citizenIdentifierType;
}
