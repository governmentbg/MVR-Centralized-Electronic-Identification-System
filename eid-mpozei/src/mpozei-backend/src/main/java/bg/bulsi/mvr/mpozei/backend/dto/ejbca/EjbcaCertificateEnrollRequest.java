package bg.bulsi.mvr.mpozei.backend.dto.ejbca;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

@Data
public class EjbcaCertificateEnrollRequest {
    @JsonProperty("certificate_request")
    private String certificateRequest;

    @JsonProperty("certificate_profile_name")
    private String certificateProfileName;

    @JsonProperty("end_entity_profile_name")
    private String endEntityProfileName;

    @JsonProperty("certificate_authority_name")
    private String certificateAuthorityName;

    @JsonProperty("account_binding_id")
    private String accountBindingId;

    @JsonProperty("include_chain")
    private boolean includeChain = true;

    private String username;
    private String password;
    private String email;
}
