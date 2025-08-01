package bg.bulsi.mvr.mpozei.contract.dto.report;

import lombok.AllArgsConstructor;
import lombok.Data;

import java.io.Serializable;
import java.time.LocalDate;
import java.util.UUID;

@Data
@AllArgsConstructor
public class ReportByOfficesRequest implements Serializable {
    private UUID eidAdministratorId;
    private LocalDate from;
    private LocalDate to;
}