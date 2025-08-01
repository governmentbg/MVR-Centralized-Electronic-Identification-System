package bg.bulsi.mvr.mpozei.model.nomenclature;

import bg.bulsi.mvr.mpozei.model.AbstractAudit;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.io.Serial;
import java.util.Objects;
import java.util.UUID;

@Getter
@Setter
@Audited
@Entity
public class NomenclatureType extends AbstractAudit<String> {
    @Serial
    private static final long serialVersionUID = -7366569383501357532L;

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id;

    @Column
    private String name;

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof NomenclatureType that)) return false;
        return Objects.equals(id, that.id) && Objects.equals(name, that.name);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id, name);
    }
}
