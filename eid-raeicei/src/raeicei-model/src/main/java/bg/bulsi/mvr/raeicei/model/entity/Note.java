package bg.bulsi.mvr.raeicei.model.entity;

import bg.bulsi.mvr.raeicei.model.AbstractAudit;
import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Getter
@Setter
@Audited
@Entity
@AllArgsConstructor
@NoArgsConstructor
@Table(schema = "raeicei", name = "notes")
public class Note extends AbstractAudit<String> {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id;

    @Column(nullable = false)
    private String authorsUsername;

    @Column(nullable = false)
    private boolean isOutgoing = false;

    @Column(nullable = false)
    private String content;

    @ElementCollection
    @CollectionTable(name = "note_atachments_names",
            joinColumns = {@JoinColumn(name = "note_id", referencedColumnName = "id")})
    @Column(name = "attachment_name")
    private List<String> attachmentsNames = new ArrayList<>();

    @Column(nullable = false)
    private String newStatus;
}
