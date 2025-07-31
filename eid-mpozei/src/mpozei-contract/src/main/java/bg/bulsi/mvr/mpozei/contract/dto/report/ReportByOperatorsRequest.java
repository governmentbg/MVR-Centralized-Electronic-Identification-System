package bg.bulsi.mvr.mpozei.contract.dto.report;

import lombok.AllArgsConstructor;
import lombok.Data;

import java.io.Serializable;
import java.time.LocalDate;
import java.util.List;

@Data
@AllArgsConstructor
public class ReportByOperatorsRequest implements Serializable {
    private List<String> operators;
    private LocalDate from;
    private LocalDate to;
}