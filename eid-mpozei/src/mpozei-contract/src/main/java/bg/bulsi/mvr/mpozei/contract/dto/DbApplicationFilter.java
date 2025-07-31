package bg.bulsi.mvr.mpozei.contract.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import org.springframework.data.domain.Pageable;

import java.io.Serial;
import java.io.Serializable;
import java.time.LocalDateTime;
import java.time.OffsetDateTime;
import java.util.List;
import java.util.UUID;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class DbApplicationFilter implements Serializable {

	@Serial
	private static final long serialVersionUID = 8701297303094078475L;

    private UUID eidentityId;
    private UUID citizenProfileId;
    private String applicationId;
    private String applicationNumber;
    private LocalDateTime createdDateFrom;
    private LocalDateTime createdDateTo;
    private List<UUID> deviceIds;
    private List<ApplicationStatus> statuses;
    private List<ApplicationType> applicationTypes;
    private UUID eidAdministratorId;
    private List<UUID> eidAdministratorFrontOfficeId;
    private List<ApplicationSubmissionType> submissionTypes;
    private Pageable pageable;

    public DbApplicationFilter(UUID eidentityId, String applicationId, String applicationNumber, LocalDateTime createdDateFrom, LocalDateTime createdDateTo, List<UUID> deviceIds, List<ApplicationStatus> statuses, List<ApplicationSubmissionType> submissionTypes, Pageable pageable) {
        this.eidentityId = eidentityId;
        this.applicationId = applicationId;
        this.createdDateFrom = createdDateFrom;
        this.createdDateTo = createdDateTo;
        this.deviceIds = deviceIds;
        this.statuses = statuses;
        this.submissionTypes = submissionTypes;
        this.pageable = pageable;
        this.applicationNumber = applicationNumber;
    }

    public DbApplicationFilter(UUID eidentityId, String applicationId, LocalDateTime createdDateFrom, LocalDateTime createdDateTo, List<UUID> deviceIds, List<ApplicationStatus> statuses, List<ApplicationType> applicationTypes, List<ApplicationSubmissionType> submissionTypes, UUID administratorId, List<UUID> eidAdministratorFrontOfficeId, Pageable pageable) {
        this.eidentityId = eidentityId;
        this.applicationId = applicationId;
        this.createdDateFrom = createdDateFrom;
        this.createdDateTo = createdDateTo;
        this.deviceIds = deviceIds;
        this.statuses = statuses;
        this.applicationTypes = applicationTypes;
        this.eidAdministratorId = administratorId;
        this.eidAdministratorFrontOfficeId = eidAdministratorFrontOfficeId;
        this.submissionTypes = submissionTypes;
        this.pageable = pageable;
    }
}
