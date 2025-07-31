package bg.bulsi.mvr.raeicei.model.entity;

import bg.bulsi.mvr.raeicei.model.enums.NomLanguage;
import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.util.Map;
import java.util.UUID;

@Getter
@Setter
@Entity
@Audited
@NoArgsConstructor
@AllArgsConstructor
@Table(schema = "raeicei", name = "document_types")
public class DocumentType {
//    ACTIVITIES,
//    SECURITY_MEASURES,
//    STAFF_REQUIREMENTS,
//    SECURITY_ASSESSMENT,
//    DAMAGES_LIABILITY_COVERAGE,
//    SECURITY_PRODCDURES,
//    TECHNICAL_DEVICES_LIST,
//    STAFF_LIST,
//    PAID_TAX,
//    OTHER

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id;

    @Column(nullable = false)
    private String name;

    @Column
    private Boolean active;

    @Column
    private Boolean requiredForAdministrator;

    @Column
    private Boolean requiredForCenter;

    @ElementCollection
    @CollectionTable(name = "document_descriptions",
            joinColumns = {@JoinColumn(name = "document_id", referencedColumnName = "id")})
    @MapKeyColumn(name = "language")
    @Column(name = "description")
    private Map<NomLanguage, String> descriptions;

    public DocumentType(UUID id) {
        this.id = id;
    }
}
