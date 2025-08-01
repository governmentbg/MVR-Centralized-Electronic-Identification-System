package bg.bulsi.mvr.mpozei.contract.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import org.springframework.data.domain.Pageable;

import java.io.Serial;
import java.io.Serializable;
import java.time.LocalDate;
import java.util.List;
import java.util.UUID;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class CitizenCertificateFilter implements Serializable {

    @Serial
    private static final long serialVersionUID = 5540609219683534989L;

    private UUID eidentityId;
    private UUID citizenProfileId;
    private String id;
    private String serialNumber;
    private List<CertificateStatus> statuses;
    private LocalDate validityFrom;
    private LocalDate validityUntil;
    private List<UUID> deviceIds;
    private String alias;
    private boolean publicApi;
    private Pageable pageable;

    public CitizenCertificateFilter(UUID eidentityId, String id, String serialNumber, List<CertificateStatus> statuses, LocalDate validityFrom, LocalDate validityUntil, List<UUID> deviceIds, String alias, boolean publicApi, Pageable pageable) {
        this.eidentityId = eidentityId;
        this.id = id;
        this.serialNumber = serialNumber;
        this.statuses = statuses;
        this.validityFrom = validityFrom;
        this.validityUntil = validityUntil;
        this.deviceIds = deviceIds;
        this.publicApi = publicApi;
        this.alias = alias;
        this.pageable = pageable;
    }
}
