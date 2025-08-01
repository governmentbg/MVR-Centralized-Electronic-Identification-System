package bg.bulsi.mvr.iscei.contract.dto;

import java.util.UUID;

import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class ExternalProviderLoginResultDto {

	private UUID systemId;
    private String systemName;
    private String systemType;
    private String sectorId;
}
