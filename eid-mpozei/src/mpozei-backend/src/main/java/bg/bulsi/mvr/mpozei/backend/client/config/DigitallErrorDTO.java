package bg.bulsi.mvr.mpozei.backend.client.config;

import lombok.Data;

import java.util.List;
import java.util.Map;

@Data
public class DigitallErrorDTO {
    private String error;
    private Map<String, List<String>> errors;
}
