package bg.bulsi.mvr.raeicei.model.entity.application.number;

import bg.bulsi.mvr.raeicei.model.enums.ApplicationType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.io.Serializable;
import java.util.Objects;

import static org.hibernate.envers.RelationTargetAuditMode.NOT_AUDITED;

@Entity
@Getter
@Setter
@Table(schema = "raeicei", name = "number_generator")
@Audited(targetAuditMode = NOT_AUDITED)
@NoArgsConstructor
public class NumberCounter implements Serializable {
    private static final long serialVersionUID = 2780639974217916377L;

	public final static String NUMBER_COUNTER_ID = "type";

    @Id
    private ApplicationType type;

    @Column(name = "counter", columnDefinition = "integer DEFAULT 0")
    private Integer counter = 0;

    public NumberCounter(ApplicationType type) {
        this.type = type;
    }

    public int getNextCount() {
        Integer counter = getCounter();
        ++counter;
        setCounter(counter);
        return counter;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof NumberCounter that)) return false;
        return Objects.equals(type, that.type) && Objects.equals(counter, that.counter);
    }

    @Override
    public int hashCode() {
        return Objects.hash(type, counter);
    }
}