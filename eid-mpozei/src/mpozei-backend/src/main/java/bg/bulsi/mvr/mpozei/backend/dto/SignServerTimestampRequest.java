package bg.bulsi.mvr.mpozei.backend.dto;

import lombok.Data;

@Data
public class SignServerTimestampRequest {
    private String data;
    private String encoding = "BASE64";

    public SignServerTimestampRequest(String data) {
        this.data = data;
    }
}
