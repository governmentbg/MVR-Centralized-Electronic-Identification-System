package bg.bulsi.mvr.mpozei.contract.dto.report;

import lombok.AllArgsConstructor;
import lombok.Data;

import java.io.Serializable;
import java.time.LocalDate;

@Data
@AllArgsConstructor
public class ReportTotalRequest implements Serializable {
    private LocalDate from;
    private LocalDate to;
}