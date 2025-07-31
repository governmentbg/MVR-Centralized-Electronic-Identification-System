package bg.bulsi.mvr.mpozei.model.application;

import bg.bulsi.mvr.common.pipeline.PipelineEntity;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.mpozei.config.EGNEncryption;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.contract.dto.IdentifierType;
import bg.bulsi.mvr.mpozei.model.AbstractAudit;
import bg.bulsi.mvr.mpozei.model.id_generator.id.ApplicationNumber;
import bg.bulsi.mvr.mpozei.model.nomenclature.ReasonNomenclature;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.annotations.ColumnTransformer;
import org.hibernate.annotations.JdbcTypeCode;
import org.hibernate.envers.Audited;
import org.hibernate.envers.NotAudited;
import org.hibernate.type.SqlTypes;

import java.io.Serial;
import java.util.Objects;
import java.util.UUID;

@Getter
@Setter
@Entity
@Audited
@Table(name = "application", schema = "mpozei")
@Inheritance(strategy = InheritanceType.SINGLE_TABLE)
@DiscriminatorColumn(discriminatorType = DiscriminatorType.STRING, name = "application_type")
public class AbstractApplication extends AbstractAudit<String> implements PipelineEntity {

	public static final UUID MVR_ADMINISTRATOR_ID = UUID.fromString("9030e0ed-af59-444e-89e2-1ccda572c372");
	
	@Serial
	private static final long serialVersionUID = 8729312743054253661L;

	@Id
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id = UUID.randomUUID();

    @Column(name = "application_type", insertable = false, updatable = false)
    @Enumerated(EnumType.STRING)
    private ApplicationType applicationType;

    @Column
    @Enumerated(EnumType.STRING)
    private ApplicationStatus status = ApplicationStatus.SUBMITTED;

    @Column
    @Enumerated(EnumType.STRING)
    private PipelineStatus pipelineStatus =  PipelineStatus.INITIATED;

    @OneToOne(cascade = CascadeType.PERSIST, fetch = FetchType.EAGER)
    @NotAudited
    private ApplicationNumber applicationNumber ;

    @Column
    private String firstName;

    @Column
    private String secondName;

    @Column
    private String lastName;
    
    @Column
    private String firstNameLatin;

    @Column
    private String secondNameLatin;

    @Column
    private String lastNameLatin;

    @Column
    private UUID eidentityId;

    @Column
    private UUID citizenProfileId;

    @Lob
    @Column(columnDefinition = "text")
    private String applicationXml;

    // signature of xml - Base64.encoded CMSSignedData
    @Lob
    @Column(columnDefinition = "text")
    private String detachedSignature;

    @Column
    private UUID eidAdministratorId;

    @Column
    private UUID administratorFrontOfficeId;
    
    @Column
    @Enumerated(EnumType.STRING)
    private IdentifierType citizenIdentifierType;

    @Column
    private String citizenship;

    @ColumnTransformer(
            read = "pgp_sym_decrypt( citizen_identifier_number::bytea, '" + EGNEncryption.PASSWORD + "')",
            write = "pgp_sym_encrypt( ?, '" + EGNEncryption.PASSWORD + "')"
    )
    @Column(name = "citizen_identifier_number",columnDefinition = "bytea")
    private String citizenIdentifierNumber;

    @Column
    @Enumerated(EnumType.STRING)
    private ApplicationSubmissionType submissionType;
    
    @Column
    @JdbcTypeCode(SqlTypes.JSON)
    private ApplicationParams params = new ApplicationParams();
    
    /**
     * This is used to store values that won't be stored in the Database
     */
    @Transient
    private TemporaryData temporaryData = new TemporaryData();

    @Column
    private UUID deviceId;

    @ManyToOne(fetch = FetchType.EAGER)
    @JoinColumn
    private ReasonNomenclature reason;

    @Column
    private String reasonText;

    public String getFullName() {
        return firstName + " " + secondName + " " + lastName;
    }

    public String getFullLatinName() {
        return firstNameLatin + " " + secondNameLatin + " " + lastNameLatin;
    }

    @Override
	public int hashCode() {
		return Objects.hash(applicationNumber, applicationType, citizenIdentifierNumber, citizenIdentifierType,
				citizenship, deviceId, eidAdministratorId, eidentityId, firstName, firstNameLatin, id,
				lastName, lastNameLatin, params, pipelineStatus, reason, reasonText, secondName, secondNameLatin,
                applicationXml, status);
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
		return Objects.equals(applicationNumber, other.applicationNumber) && applicationType == other.applicationType
				&& Objects.equals(citizenIdentifierNumber, other.citizenIdentifierNumber)
				&& citizenIdentifierType == other.citizenIdentifierType
				&& Objects.equals(citizenship, other.citizenship) && deviceId == other.deviceId
				&& Objects.equals(eidAdministratorId, other.eidAdministratorId)
				&& Objects.equals(eidentityId, other.eidentityId)
				&& Objects.equals(firstName, other.firstName) && Objects.equals(firstNameLatin, other.firstNameLatin)
				&& Objects.equals(id, other.id) && Objects.equals(lastName, other.lastName)
				&& Objects.equals(lastNameLatin, other.lastNameLatin) && Objects.equals(params, other.params)
				&& pipelineStatus == other.pipelineStatus && Objects.equals(reason, other.reason)
				&& Objects.equals(reasonText, other.reasonText) && Objects.equals(secondName, other.secondName)
				&& Objects.equals(secondNameLatin, other.secondNameLatin)
				&& Objects.equals(applicationXml, other.applicationXml) && status == other.status;
	}

	protected static class Discriminator {

    	private Discriminator(){}

        public static final String ISSUE_EID = "ISSUE_EID";
        public static final String RESUME_EID = "RESUME_EID";
        public static final String REVOKE_EID = "REVOKE_EID";
        public static final String STOP_EID = "STOP_EID";
    }
}
