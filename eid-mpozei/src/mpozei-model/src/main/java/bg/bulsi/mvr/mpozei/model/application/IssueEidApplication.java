package bg.bulsi.mvr.mpozei.model.application;

import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.io.Serial;

@Getter
@Setter
@Entity
@Audited
@DiscriminatorValue(value = AbstractApplication.Discriminator.ISSUE_EID)
public class IssueEidApplication extends AbstractApplication {
    @Serial
    private static final long serialVersionUID = -5679894674578179801L;
}


