package bg.bulsi.mvr.mpozei.backend.dto;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

import java.util.UUID;

@Data
public class PunCreateCarrierRequest {
    @JsonProperty("type")
    private UUID punDeviceId;

    @JsonProperty("eId")
    private UUID eidentityId;

    private String serialNumber;
    private UUID certificateId;
}
