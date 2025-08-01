package bg.bulsi.mvr.mpozei.backend.dto.ejbca;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

import java.time.LocalDateTime;

@Data
public class EjbcaRevocationResponse {
    @JsonProperty("issuer_dn")
    private String issuerDn;

    @JsonProperty("serial_number")
    private String serialNumber;

    @JsonProperty("revocation_reason")
    private String revocationReason;

    @JsonProperty("revocation_date")
    private LocalDateTime revocationDate;

    @JsonProperty("invalidity_date")
    private LocalDateTime invalidityDate;
}
