package bg.bulsi.mvr.raeicei.model.entity;

import jakarta.persistence.Column;
import jakarta.persistence.ElementCollection;
import jakarta.persistence.Entity;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.Audited;

import java.util.List;

@Getter
@Setter
@Audited
@Entity

public class Employee extends Person {

	@Column
	@ElementCollection(targetClass = String.class)
	private List<String> roles;
}
