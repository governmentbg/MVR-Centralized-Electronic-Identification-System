package bg.bulsi.mvr.mpozei.model.nomenclature;

import bg.bulsi.mvr.mpozei.model.AbstractAudit;
import jakarta.persistence.*;
import lombok.Data;
import lombok.EqualsAndHashCode;
import org.hibernate.envers.Audited;

import java.io.Serial;
import java.util.UUID;

@Data
@Entity
@Audited
@EqualsAndHashCode(callSuper = false)
@Inheritance(strategy = InheritanceType.TABLE_PER_CLASS)
public abstract class AbstractNomenclature extends AbstractAudit<String> {
	@Serial
	private static final long serialVersionUID = 7751315263724183349L;

	@Id
	@GeneratedValue(strategy = GenerationType.UUID)
	@Column(name = "id", nullable = false, unique = true, updatable = false)
	private UUID id;

	@JoinColumn(name = "nomenclature_type")
	@ManyToOne(optional = false, fetch = FetchType.EAGER)
	private NomenclatureType nomCode;

	@Column
	private String name;

	@Column
	@Enumerated(EnumType.STRING)
	private NomLanguage language;
	
	@Column
	private String description;

	@Column(name = "active", columnDefinition = "boolean default true")
	private boolean active = true;
}
