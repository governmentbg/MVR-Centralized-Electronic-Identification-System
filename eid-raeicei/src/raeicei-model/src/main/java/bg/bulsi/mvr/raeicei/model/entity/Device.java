package bg.bulsi.mvr.raeicei.model.entity;

import bg.bulsi.mvr.raeicei.contract.dto.DeviceType;
import bg.bulsi.mvr.raeicei.model.AbstractAudit;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.util.UUID;

@Getter
@Setter
@Entity
@Audited
@Table(schema = "raeicei", name = "devices")
public class Device extends AbstractAudit<String> {

    private static final long serialVersionUID = 9094896304115277371L;

	@Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id;

    @Column(nullable = false)
    @Enumerated(EnumType.STRING)
    private DeviceType type;

    @Column(nullable = false)
    private String name; // документ за самоличност

    @Column
    private String description; // документ за самоличност издадена от МВР

    @Column
    private String authorizationLink;

    @Column
    // TODO: URL formatting
    private String backchannelAuthorizationLink;
    
    @Column
    private Boolean isActive;
}
