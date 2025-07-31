package bg.bulsi.mvr.mpozei.model.nomenclature;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.Table;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.io.Serial;

@Getter
@Setter
@Audited
@Entity
@Table(schema = "mpozei", name = "reason_nomenclature")
public class ReasonNomenclature extends AbstractNomenclature {
    public static final String REASON_REPLACED = "REPLACED";
    public static final String CERTIFICATE_EXPIRED_REASON = "CERTIFICATE_EXPIRED";
    public static final String CITIZEN_PASSED_AWAY_REASON = "REVOKED_PASSED_AWAY_CITIZEN";
    public static final String STOPPED_REVOKED_BY_SYSTEM = "STOPPED_REVOKED_BY_SYSTEM";
    public static final String DENIED_TIMED_OUT = "DENIED_TIMED_OUT";
    
    @Serial
    private static final long serialVersionUID = -7767927079576831232L;

    @Column(name = "text_required")
    private Boolean textRequired;

    @Column(name = "permitted_user")
    @Enumerated(EnumType.STRING)
    private PermittedUser permittedUser =  PermittedUser.PRIVATE;

}
