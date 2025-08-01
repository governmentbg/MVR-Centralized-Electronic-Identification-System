package bg.bulsi.mvr.mpozei.contract.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import org.springframework.data.domain.Pageable;

import java.io.Serial;
import java.io.Serializable;
import java.time.OffsetDateTime;
import java.util.List;
import java.util.UUID;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class ApplicationFilter implements Serializable {
    @Serial
    private static final long serialVersionUID = 1058395449807620199L;

    private UUID eidentityId;
    private UUID citizenProfileId;
    private String applicationId;
    private String applicationNumber;
    private OffsetDateTime createdDateFrom;
    private OffsetDateTime createdDateTo;
    private List<UUID> deviceIds;
    private List<ApplicationStatus> statuses;
    private List<ApplicationType> applicationTypes;
    private UUID eidAdministratorId;
    private List<UUID> eidAdministratorFrontOfficeId;
    private List<ApplicationSubmissionType> submissionTypes;
    private Pageable pageable;

    // Used by external Eid Administrators/MVR
    public ApplicationFilter(UUID eidentityId, String applicationId, String applicationNumber, OffsetDateTime createdDateFrom, OffsetDateTime createdDateTo, List<UUID> deviceIds, List<UUID> eidAdministratorFrontOfficeId, List<ApplicationStatus> statuses, List<ApplicationSubmissionType> submissionTypes, List<ApplicationType> applicationTypes, Pageable pageable) {
        this.eidentityId = eidentityId;
        this.applicationId = applicationId;
        this.createdDateFrom = createdDateFrom;
        this.createdDateTo = createdDateTo;
        this.deviceIds = deviceIds;
        this.eidAdministratorFrontOfficeId = eidAdministratorFrontOfficeId;
        this.statuses = statuses;
        this.submissionTypes = submissionTypes;
        this.applicationTypes = applicationTypes;
        this.pageable = pageable;
        this.applicationNumber = applicationNumber;
    }

    //Used by MVR - superuser
    public ApplicationFilter(UUID eidentityId, String applicationId, String applicationNumber, OffsetDateTime createdDateFrom, OffsetDateTime createdDateTo, List<UUID> deviceIds, UUID eidAdministratorId, List<UUID> eidAdministratorFrontOfficeId, List<ApplicationStatus> statuses, List<ApplicationSubmissionType> submissionTypes, List<ApplicationType> applicationTypes, Pageable pageable) {
        this.eidentityId = eidentityId;
        this.applicationId = applicationId;
        this.createdDateFrom = createdDateFrom;
        this.createdDateTo = createdDateTo;
        this.deviceIds = deviceIds;
        this.submissionTypes = submissionTypes;
        this.statuses = statuses;
        this.pageable = pageable;
        this.applicationTypes = applicationTypes;
        this.eidAdministratorId = eidAdministratorId;
        this.eidAdministratorFrontOfficeId = eidAdministratorFrontOfficeId;
        this.applicationNumber = applicationNumber;
    }
}
