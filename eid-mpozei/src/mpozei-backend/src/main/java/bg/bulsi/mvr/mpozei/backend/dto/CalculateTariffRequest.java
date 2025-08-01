package bg.bulsi.mvr.mpozei.backend.dto;

import lombok.Data;

import java.time.LocalDate;
import java.util.UUID;

@Data
public class CalculateTariffRequest {
    private UUID eidManagerId;
    private UUID deviceId;
    private LocalDate currentDate;
    private Integer age;
    private boolean disability;
    private UUID providedServiceId;
}
