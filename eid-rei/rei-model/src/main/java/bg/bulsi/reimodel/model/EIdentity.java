package bg.bulsi.reimodel.model;

import com.fasterxml.jackson.annotation.JsonIgnore;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import java.util.UUID;

@Getter
@Setter
@Entity
@Audited
@Table(name = "eidentity", schema = "rei")
public class EIdentity extends AbstractAudit<String> {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id;

    @JsonIgnore
    @OneToMany(mappedBy = "eidentity", cascade = CascadeType.ALL, orphanRemoval = true)
    private List <CitizenIdentifier> citizenIdentifiers = new ArrayList<>();

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof EIdentity eIdentity)) return false;
        return Objects.equals(id, eIdentity.id) && Objects.equals(citizenIdentifiers, eIdentity.citizenIdentifiers);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }
}
