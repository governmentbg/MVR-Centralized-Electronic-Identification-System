package bg.bulsi.mvr.raeicei.model.entity.tariif;

import bg.bulsi.mvr.raeicei.model.AbstractAudit;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import bg.bulsi.mvr.raeicei.model.enums.Currency;
import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.ToString;
import lombok.experimental.SuperBuilder;

import org.hibernate.envers.Audited;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.UUID;

@Getter
@Setter
@Entity
@Audited
@ToString
@NoArgsConstructor
@AllArgsConstructor
//TODO: should we use SINGLE table or joined
@Inheritance(strategy = InheritanceType.SINGLE_TABLE)
@DiscriminatorColumn(discriminatorType = DiscriminatorType.STRING, name = "tariff_type")
@Table(name = "tariffs", schema = "raeicei")
public abstract class Tariff extends AbstractAudit<String> {

    private static final long serialVersionUID = -3301604246347313114L;

	@Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id;

    @Column(nullable = false)
    private LocalDate startDate;

    @Column(nullable = false)
    private BigDecimal price;
    
    @Enumerated(EnumType.STRING)
    private Currency currency;

    //TODO: replace with code above with Enum for Service for CEI/EidAdministrator
    
    @ManyToOne(optional = false)
    private EidManager eidManager;

    @Column
    private Boolean isActive;

//  @Column(nullable = false)
//  private Double startApplicationPrice;
//  @Column(nullable = false)
//  private Double stopApplicationPrice;
//  @Column(nullable = false)
//  private Double resumeApplicationPrice;
//  @Column(nullable = false)
//  private Double revokeApplicationPrice;
}
