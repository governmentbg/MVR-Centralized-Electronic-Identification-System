package bg.bulsi.mvr.mpozei.backend.dto;

import lombok.Data;

@Data
public class OtpCodeDTO {
    private String secret;
    private String issuer;
}
