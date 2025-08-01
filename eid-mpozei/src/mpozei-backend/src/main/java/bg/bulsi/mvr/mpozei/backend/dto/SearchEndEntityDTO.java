package bg.bulsi.mvr.mpozei.backend.dto;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

@Data
public class SearchEndEntityDTO {
    private String username;
    private String dn;
    private String email;
    private String status;
    private String token;

    @JsonProperty("subject_alt_name")
    private String subjectAltName;
}
