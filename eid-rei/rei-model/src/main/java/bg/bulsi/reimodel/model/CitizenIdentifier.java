package bg.bulsi.reimodel.model;


import bg.bulsi.reimodel.config.EGNEncryption;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.annotations.ColumnTransformer;
import org.hibernate.envers.Audited;

import java.util.Objects;
import java.util.UUID;

@Getter
@Setter
@Entity
@Audited
@Table(name = "citizen_identifier", schema = "rei")
public class CitizenIdentifier extends AbstractAudit<String> {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id;

    @Column(name = "first_name")
    private String firstName;

    @Column(name = "second_name")
    private String secondName;

    @Column(name = "last_name")
    private String lastName;

    @Column
    @Enumerated(EnumType.STRING)
    private IdentifierType type;

    @ColumnTransformer(
            read = "pgp_sym_decrypt( number::bytea, '" + EGNEncryption.PASSWORD + "')",
            write = "pgp_sym_encrypt( ?, '" + EGNEncryption.PASSWORD + "')"
    )
    @Column(name = "number", columnDefinition = "bytea")
    private String number;

    @Column
    private Boolean active = true;

    @ManyToOne(cascade = CascadeType.PERSIST)
    @JoinColumn(name = "eidentity_id")
    private EIdentity eidentity = new EIdentity();

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof CitizenIdentifier that)) return false;
        return Objects.equals(id, that.id) && Objects.equals(firstName, that.firstName) && Objects.equals(secondName, that.secondName) && Objects.equals(lastName, that.lastName) && type == that.type && Objects.equals(number, that.number) && Objects.equals(active, that.active) && Objects.equals(eidentity, that.eidentity);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id, firstName, secondName, lastName, type, number, active, eidentity);
    }
}
