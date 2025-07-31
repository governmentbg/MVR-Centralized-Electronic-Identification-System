package bg.bulsi.mvr.iscei.contract.dto;

import java.util.UUID;

import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class CertificateLoginResultDto {

	private UUID id;
	private UUID eidentityId;
    private String citizenIdentifier;
    private IdentifierTypeDTO citizenIdentifierType;
    private String x509CertificateSn;
    private String x509CertificateIssuerDn;
    private LevelOfAssurance levelOfAssurance;
    private UUID deviceId;
}
