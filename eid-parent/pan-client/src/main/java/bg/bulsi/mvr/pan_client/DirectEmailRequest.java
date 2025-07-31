package bg.bulsi.mvr.pan_client;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;

@Builder
@Data
@AllArgsConstructor
public class DirectEmailRequest {
    private String language;
    private String subject;
    private String body;
    private String emailAddress;
}
