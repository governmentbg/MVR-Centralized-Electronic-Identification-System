package bg.bulsi.mvr.mpozei.model.id_generator.id;

import bg.bulsi.mvr.mpozei.model.id_generator.NumberCounter;
import bg.bulsi.mvr.mpozei.model.id_generator.NumberGenerator;
import jakarta.annotation.Nonnull;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotEmpty;
import lombok.Data;
import lombok.Getter;
import lombok.NonNull;
import lombok.Setter;
import org.hibernate.annotations.GenericGenerator;
import org.hibernate.envers.Audited;
import org.hibernate.envers.RelationTargetAuditMode;

import java.io.Serializable;
import java.util.Objects;
import java.util.UUID;

//import static bg.bulsi.mvr.mpozei.model.id_generator.NumberCounter.NUMBER_COUNTER_ID;
import static org.hibernate.envers.RelationTargetAuditMode.NOT_AUDITED;

@Entity
@Data
@Table(schema = "mpozei", name = "application_number")
@Audited(targetAuditMode = RelationTargetAuditMode.NOT_AUDITED)
public class ApplicationNumber implements Serializable {
    public final static String ID = "eae8db59-b348-43e5-b21f-c9c9d0d65d9b";

    @Id
    @GeneratedValue(generator = "application-number-generator")
    //, parameters = { @org.hibernate.annotations.Parameter(name = NumberCounter.NUMBER_COUNTER_ID, value = ID) }
    @GenericGenerator(name = "application-number-generator", type = NumberGenerator.class)
    private String id;
    
    @NonNull
    @NotEmpty
	private String administratorCode;
    
    @NonNull
    @NotEmpty
    private String officeCode;

    

	public ApplicationNumber(@NonNull @NotEmpty String administratorCode, @NonNull @NotEmpty String officeCode) {
		super();
		this.administratorCode = administratorCode;
		this.officeCode = officeCode;
	}
    
	public ApplicationNumber() {
		super();
	}
}