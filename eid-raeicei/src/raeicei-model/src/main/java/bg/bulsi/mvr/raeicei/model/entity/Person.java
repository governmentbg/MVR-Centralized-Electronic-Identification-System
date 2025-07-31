package bg.bulsi.mvr.raeicei.model.entity;

import bg.bulsi.mvr.raeicei.model.AbstractAudit;
import bg.bulsi.mvr.raeicei.model.enums.IdentifierType;
import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.util.UUID;

@Getter
@Setter
@Audited
@MappedSuperclass
@AllArgsConstructor
@NoArgsConstructor
public abstract class Person extends AbstractAudit<String> {
    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id;

    @Column(nullable = false)
    private String name;
    
    @Column(nullable = false)
    private String nameLatin;

    @Column
    private Boolean isActive;
    
    @Column
    private String phoneNumber;
    
    @Column
    private String email;

    @Column
    @Enumerated(EnumType.STRING)
    private IdentifierType citizenIdentifierType;

    @Column(name = "citizen_identifier_number", length = 10)
    private String citizenIdentifierNumber;

}
