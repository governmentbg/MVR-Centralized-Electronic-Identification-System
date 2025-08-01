package bg.bulsi.mvr.iscei.contract.dto;

import java.util.UUID;

import lombok.Data;

@Data
public class ProviderEmployeeInfo {
	private String uid;
	private IdentifierTypeDTO uidType;
	private UUID providerId;
	private String providerName;
	private Boolean isAdministrator;
}
