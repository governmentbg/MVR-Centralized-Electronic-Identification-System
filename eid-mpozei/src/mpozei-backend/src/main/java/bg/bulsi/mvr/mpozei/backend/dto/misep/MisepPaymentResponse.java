package bg.bulsi.mvr.mpozei.backend.dto.misep;

import lombok.Data;
import org.springframework.format.annotation.DateTimeFormat;

import java.time.LocalDateTime;
import java.util.UUID;

@Data
public class MisepPaymentResponse {
    private UUID id;
    @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME)
    private LocalDateTime paymentDeadline;
    private String accessCode;
}
