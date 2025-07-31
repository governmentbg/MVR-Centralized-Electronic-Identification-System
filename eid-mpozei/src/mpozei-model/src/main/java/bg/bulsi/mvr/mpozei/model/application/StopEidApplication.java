package bg.bulsi.mvr.mpozei.model.application;

import java.io.Serial;

import org.hibernate.envers.Audited;

import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;
import lombok.Getter;
import lombok.Setter;

@Entity
@Getter
@Setter
@Audited
@DiscriminatorValue(value = AbstractApplication.Discriminator.STOP_EID)
public class StopEidApplication extends AbstractApplication {
    @Serial
    private static final long serialVersionUID = 1438475747699500316L;
}
