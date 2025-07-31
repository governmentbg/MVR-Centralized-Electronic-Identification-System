package bg.bulsi.mvr.mpozei.model.certificate;

import bg.bulsi.mvr.mpozei.contract.dto.CertificateStatus;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.time.OffsetDateTime;
import java.util.Objects;
import java.util.UUID;

@Entity
@Getter
@Setter
@Audited
public class CertificateHistory {
    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id;

    @Column
    private UUID certificateId;

    @Column
    private UUID applicationId;

    @Column
    private String applicationNumber;
    
    @Column
    private UUID deviceId;
    
    @Column
    private OffsetDateTime createDate;

    @Column
    private OffsetDateTime modifiedDate;

    @Column
    private OffsetDateTime validityFrom;

    @Column
    private OffsetDateTime validityUntil;

    @Column
    @Enumerated(EnumType.STRING)
    private CertificateStatus status;

    @Column
    private UUID reasonId;

    @Column
    private String reasonText;
    
    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof CertificateHistory that)) return false;
        return Objects.equals(id, that.id) && Objects.equals(applicationId, that.applicationId) && Objects.equals(createDate, that.createDate) && Objects.equals(modifiedDate, that.modifiedDate) && Objects.equals(validityUntil, that.validityUntil) && status == that.status && Objects.equals(certificateId, that.certificateId) && Objects.equals(reasonId, that.reasonId) && Objects.equals(reasonText, that.reasonText);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id, applicationId, createDate, modifiedDate, validityUntil, status, certificateId, reasonId, reasonText);
    }
}
