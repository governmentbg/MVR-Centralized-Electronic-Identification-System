package bg.bulsi.mvr.raeicei.model.entity.application;

import bg.bulsi.mvr.common.pipeline.PipelineEntity;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.raeicei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.raeicei.model.AbstractAudit;
import bg.bulsi.mvr.raeicei.model.entity.*;
import bg.bulsi.mvr.raeicei.model.enums.ApplicationType;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotEmpty;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.AuditJoinTable;
import org.hibernate.envers.Audited;
import org.hibernate.envers.NotAudited;

import java.io.Serial;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import java.util.UUID;

@Getter
@Setter
@Entity
@Audited
@Table(name = "application")
@Inheritance(strategy = InheritanceType.SINGLE_TABLE)
@DiscriminatorColumn(discriminatorType = DiscriminatorType.STRING, name = "service_type")
public class AbstractApplication extends AbstractAudit<String> implements PipelineEntity {

    @Serial
    private static final long serialVersionUID = 8729312743054253661L;

    // Base application details
    @Id
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id = UUID.randomUUID();

    @Column(name = "application_type")
    @Enumerated(EnumType.STRING)
    private ApplicationType applicationType;

    @Column(name = "service_type", insertable = false, updatable = false)
    @Enumerated(EnumType.STRING)
    private ManagerType managerType;

    @Column
    @Enumerated(EnumType.STRING)
    private ApplicationStatus status = ApplicationStatus.IN_REVIEW;

    @Column
    @Enumerated(EnumType.STRING)
    private PipelineStatus pipelineStatus = PipelineStatus.INITIATED;

    @OneToOne(cascade = CascadeType.PERSIST, fetch = FetchType.EAGER)
    @JoinColumn(name = "application_number")
    @NotAudited
    //applicationType.name()
    private ApplicationNumber applicationNumber;

    @OneToMany(cascade = CascadeType.PERSIST, fetch = FetchType.LAZY)
    @JoinTable(
            name = "application_authorized_persons",
            joinColumns = @JoinColumn(name = "application_id"),
            inverseJoinColumns = @JoinColumn(name = "authorized_person_id"))
    @AuditJoinTable(name = "jt_application_authorized_persons_aud")
    private List<Contact> authorizedPersons;

    @ManyToOne(cascade = CascadeType.PERSIST, fetch = FetchType.LAZY)
    @JoinColumn(name = "applicant", referencedColumnName = "id")
    private Contact applicant;

    @OneToMany(cascade = CascadeType.PERSIST, fetch = FetchType.LAZY)
    @JoinTable(
            name = "application_emploees",
            joinColumns = @JoinColumn(name = "application_id"),
            inverseJoinColumns = @JoinColumn(name = "employee_id"))
    @AuditJoinTable(name = "jt_application_emploees_aud")
    private List<Employee> employees;

    @Column
    private String homePage;

    // Offices
    @OneToMany(cascade = CascadeType.ALL)
    @JoinTable(
            name = "application_offices",
            joinColumns = @JoinColumn(name = "application_id"),
            inverseJoinColumns = @JoinColumn(name = "office_id"))
    @AuditJoinTable(name = "jt_application_offices_aud")
    private List<EidManagerFrontOffice> eidManagerFrontOffices;

    // Attached documents
    @OneToMany(cascade = CascadeType.PERSIST, fetch = FetchType.LAZY)
    @JoinTable(
            name = "application_attachments",
            joinColumns = @JoinColumn(name = "application_id"),
            inverseJoinColumns = @JoinColumn(name = "atachment_id"))
    @AuditJoinTable(name = "jt_application_attachments_aud")
    private List<Document> attachments = new ArrayList<>();

    @OneToMany(cascade = CascadeType.PERSIST, fetch = FetchType.LAZY)
    @JoinTable(
            name = "application_notes",
            joinColumns = @JoinColumn(name = "application_id"),
            inverseJoinColumns = @JoinColumn(name = "note_id"))
    @AuditJoinTable(name = "jt_application_notes_aud")
    private List<Note> notes = new ArrayList<>();

    // Company details
    @Column
    @NotEmpty
    private String companyName;

    @Column
    private String companyNameLatin;

    @Column(nullable = false)
    private String eikNumber;

    @Column
    private String address;

    @Column
    private String email;

    @Column
    private String phone;

    @Column
    private String description;

    @Column
    private String logoUrl;

//	@Lob
//	private byte[] logo;

    @Column
    private String referenceNumber;

    @Column
    private String downloadUrl;

    @Override
    public int hashCode() {
        return Objects.hash(
                id, applicationType, status, pipelineStatus, applicationNumber, employees,
                eidManagerFrontOffices, attachments
        );
    }

    @Override
    public boolean equals(Object obj) {
        if (this == obj)
            return true;
        if (obj == null)
            return false;
        if (getClass() != obj.getClass())
            return false;
        AbstractApplication other = (AbstractApplication) obj;
        return Objects.equals(id, other.id)
                && applicationType == other.applicationType
                && status == other.status
                && pipelineStatus == other.pipelineStatus
                && Objects.equals(applicant, other.applicant)
                && Objects.equals(applicationNumber, other.applicationNumber);
    }
}
