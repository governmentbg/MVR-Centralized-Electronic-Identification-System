package bg.bulsi.mvr.raeicei.contract.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import org.springframework.data.domain.Pageable;

import java.io.Serial;
import java.io.Serializable;
import java.util.UUID;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class EidApplicationFilter implements Serializable {
    @Serial
    private static final long serialVersionUID = -6053100102922614051L;

    private String applicationNumber;
    private String eidName;
    private ApplicationType applicationType;
    private ApplicationStatus status;
    private UUID applicantEid;
    private UUID eidManagerId;
    private String eikNumber;
    private Pageable pageable;
}
