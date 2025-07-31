package bg.bulsi.mvr.mpozei.backend.dto;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

import java.time.OffsetDateTime;
import java.util.UUID;

@Data
public class PunCarrierDTO {
    @JsonProperty("type")
    private String deviceType;

    @JsonProperty("eId")
    private UUID eidentityId;

    private UUID id;
    private String serialNumber;
    private UUID certificateId;
    private UUID userId;
    private OffsetDateTime modifiedOn;
    private String modifiedBy;
}
