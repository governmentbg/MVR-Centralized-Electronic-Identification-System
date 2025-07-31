package bg.bulsi.mvr.raeicei.model.entity;

import bg.bulsi.mvr.raeicei.model.AbstractAudit;
import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import org.hibernate.envers.Audited;

import java.util.UUID;

@Data
@Audited
@AllArgsConstructor
@NoArgsConstructor
@Entity
@Table(schema = "raeicei", name = "documents")
public class Document extends AbstractAudit<String> {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id;

    @Column(nullable = false)
    private String fileName;

    @Lob
    @Column(nullable = false)
    private byte[] content;

    @ManyToOne(cascade = CascadeType.MERGE, fetch = FetchType.EAGER)
    @JoinColumn(name = "document_type")
    private DocumentType documentType;

    @Column(nullable = false)
    private String filePath;

    @Column(nullable = false)
    private boolean isOutgoing = false;

    public Document(UUID id) {
        this.id = id;
    }
}
