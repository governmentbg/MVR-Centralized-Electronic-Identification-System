package bg.bulsi.mvr.mpozei.backend.dto.ejbca;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

@Data
public class EjbcaEndEntityRequest {
    @JsonProperty("subject_dn")
    private String subjectDn;

    @JsonProperty("subject_alt_name")
    private String subjectAltName;

    @JsonProperty("ca_name")
    private String caName;

    @JsonProperty("certificate_profile_name")
    private String certificateProfileName;

    @JsonProperty("end_entity_profile_name")
    private String endEntityProfileName;

    @JsonProperty("account_binding_id")
    private String accountBindingId;

    private String email;
    private String token;
    private String username;
    private String password;
}
