package bg.bulsi.mvr.mpozei.backend.dto.ejbca;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

import java.util.List;

@Data
public class EjbcaSearchCertificateDTO {
    @JsonProperty("serial_number")
    private String serialNumber;

    @JsonProperty("certificate_chain")
    private List<String> certificateChain;

    @JsonProperty("response_format")
    private String responseFormat;

    private String certificate;

    @JsonProperty("certificate_profile")
    private String certificateProfile;

    @JsonProperty("end_entity_profile")
    private String endEntityProfile;
}
