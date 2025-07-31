package bg.bulsi.mvr.raeicei.model.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Table;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.util.UUID;

@Getter
@Setter
@Audited
@Entity
@Table(schema = "raeicei", name = "contact")
public class Contact extends Person {
    
    private static final long serialVersionUID = -6597421120261040247L;
	@Column
	private UUID eIdentity;
}
