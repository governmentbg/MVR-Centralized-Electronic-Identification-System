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
@DiscriminatorValue(value = AbstractApplication.Discriminator.RESUME_EID)
public class ResumeEidApplication extends AbstractApplication {
    @Serial
    private static final long serialVersionUID = -4125154226962296152L;
}
