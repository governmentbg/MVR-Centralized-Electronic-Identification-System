package bg.bulsi.mvr.mpozei.backend.dto;

import lombok.Data;

@Data
public class SignServerTimestampResponse {
    private String data;
    private String requestId;
    private String archiveId;
    private String signerCertificate;
}
