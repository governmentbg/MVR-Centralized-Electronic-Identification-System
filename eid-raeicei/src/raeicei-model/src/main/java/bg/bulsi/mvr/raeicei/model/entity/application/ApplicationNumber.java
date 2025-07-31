package bg.bulsi.mvr.raeicei.model.entity.application;


import bg.bulsi.mvr.raeicei.model.entity.application.number.NumberGenerator;
import bg.bulsi.mvr.raeicei.model.enums.ApplicationType;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.annotations.GenericGenerator;

import java.io.Serializable;
import java.util.Objects;

@Entity
@Getter
@Setter
@NoArgsConstructor
@Table(schema = "raeicei", name = "application_number")
//@Audited(targetAuditMode = NOT_AUDITED)
public class ApplicationNumber implements Serializable {
    @Id
    @GeneratedValue(generator = "application-number-generator")
    @GenericGenerator(name = "application-number-generator", type = NumberGenerator.class)
    private String id;

    @Enumerated(EnumType.STRING)
    private ApplicationType applicationType;

    public ApplicationNumber(final ApplicationType applicationType){
        this.applicationType = applicationType;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof ApplicationNumber that)) return false;
        return Objects.equals(id, that.id);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }
}