package bg.bulsi.mvr.raeicei.model.entity;

import bg.bulsi.mvr.raeicei.model.AbstractAudit;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.time.LocalDate;
import java.util.UUID;

@Getter
@Setter
@Entity
@Audited
@Table(schema = "raeicei", name = "discounts")
public class Discount extends AbstractAudit<String> {

    private static final long serialVersionUID = 3225024881106052535L;

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id;

    @Column(nullable = false)
    private LocalDate startDate;
    @Column(nullable = false)
    private Integer ageFrom;
    @Column(nullable = false)
    private Integer ageUntil;
    @Column(nullable = false)
    private Double value;
    @Column
    private boolean disability;
    @Column
    private boolean onlineService;

    @ManyToOne(optional = false)
    private EidManager eidManager;

    @ManyToOne
    private ProvidedService providedService;

    @Column
    private Boolean isActive;
}
