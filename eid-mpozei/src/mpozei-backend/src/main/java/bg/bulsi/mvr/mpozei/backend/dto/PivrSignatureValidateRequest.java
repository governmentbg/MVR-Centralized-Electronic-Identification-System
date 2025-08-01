package bg.bulsi.mvr.mpozei.backend.dto;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

@Data
public class PivrSignatureValidateRequest {
    @JsonProperty("uid")
    private String citizenIdentifierNumber;
    @JsonProperty("uidType")
    private String citizenIdentifierType;
    private String originalFile;
    private String detachedSignature;
    private String signatureProvider;
}
