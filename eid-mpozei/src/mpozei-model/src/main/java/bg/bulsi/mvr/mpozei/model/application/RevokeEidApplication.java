package bg.bulsi.mvr.mpozei.model.application;

import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.io.Serial;

@Entity
@Getter
@Setter
@Audited
@DiscriminatorValue(value = AbstractApplication.Discriminator.REVOKE_EID)
public class RevokeEidApplication extends AbstractApplication {
    @Serial
    private static final long serialVersionUID = 8581344358138730438L;
}
