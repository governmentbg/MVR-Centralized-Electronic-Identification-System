package bg.bulsi.mvr.raeicei.model.entity.providedservice;

import java.time.LocalDate;
import java.util.UUID;

import org.hibernate.envers.Audited;

import bg.bulsi.mvr.raeicei.model.AbstractAudit;
import bg.bulsi.mvr.raeicei.model.enums.EidServiceType;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;
import jakarta.persistence.Column;
import jakarta.persistence.DiscriminatorColumn;
import jakarta.persistence.DiscriminatorType;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.Inheritance;
import jakarta.persistence.InheritanceType;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Entity
@Audited
@Table(schema = "raeicei", name = "provided_service")
//@Inheritance(strategy = InheritanceType.SINGLE_TABLE)
//@DiscriminatorColumn(discriminatorType = DiscriminatorType.STRING, name = "service_type")
public class ProvidedService extends AbstractAudit<String> {
	
    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id;

    @Column(nullable = false, unique = true)
    private String name;
    
    @Column(nullable = false, unique = true)
    private String nameLatin;
    
    @Column(name = "application_type", insertable = false, updatable = false)
    @Enumerated(EnumType.STRING)
	private EidServiceType applicationType;
    
    @Column(name = "service_type", insertable = false, updatable = false)
    @Enumerated(EnumType.STRING)
    private ManagerType managerType;

    @Column
    private Boolean isActive;
}
