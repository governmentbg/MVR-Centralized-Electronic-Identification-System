package bg.bulsi.mvr.raeicei.model.entity;

import bg.bulsi.mvr.raeicei.config.StringListConverter;
import bg.bulsi.mvr.raeicei.model.AbstractAudit;
import bg.bulsi.mvr.raeicei.model.enums.Region;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotEmpty;
import jakarta.validation.constraints.NotNull;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.util.List;
import java.util.UUID;

@Getter
@Setter
@Entity
@Audited
@Table(name = "front_office", schema = "raeicei")
public class EidManagerFrontOffice extends AbstractAudit<String> {
    private static final long serialVersionUID = 8617769050541237383L;

	public static final String ONLINE = "ONLINE";

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id;

    @Column
    @NotNull
    private String name;

    @ManyToOne(cascade = CascadeType.ALL)
    @JoinTable(name="eid_manager_offices", 
        joinColumns={@JoinColumn(name="office_id")},
        inverseJoinColumns={@JoinColumn(name="eid_manager_id")})
    private EidManager eidManager;

    @Column(nullable = false)
    private String location;

    @Column
    @Enumerated(EnumType.STRING)
    private Region region;

    @Column(nullable = false)
    private String contact;

    @Column
    private Boolean isActive;

    @Column
    private String longitude;

    @Column
    private String latitude;

    @Column
    @Convert(converter = StringListConverter.class)
    private List<String> workingHours;

    @Column
    private String email;
    
    @Column(name="code",length = 4)
    private String code;

    @Column
    private String description;
}
