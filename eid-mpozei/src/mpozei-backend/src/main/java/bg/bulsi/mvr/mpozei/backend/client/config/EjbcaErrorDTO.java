package bg.bulsi.mvr.mpozei.backend.client.config;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

@Data
public class EjbcaErrorDTO {
    @JsonProperty("error_code")
    private Integer errorCode;

    @JsonProperty("error_message")
    private String errorMessage;
}
