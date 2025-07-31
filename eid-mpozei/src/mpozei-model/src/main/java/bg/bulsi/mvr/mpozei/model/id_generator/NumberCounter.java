package bg.bulsi.mvr.mpozei.model.id_generator;

import bg.bulsi.mvr.mpozei.model.id_generator.id.ApplicationNumber;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.NonNull;
import lombok.RequiredArgsConstructor;
import lombok.Setter;
import lombok.ToString;

import org.hibernate.annotations.GenericGenerator;
import org.hibernate.envers.Audited;
import org.hibernate.envers.RelationTargetAuditMode;

import java.io.Serializable;
import java.util.Objects;
import java.util.UUID;


@Entity
@Getter
@Setter
@Table(schema = "mpozei", name = "number_generator")
@Audited(targetAuditMode = RelationTargetAuditMode.NOT_AUDITED)
@IdClass(NumberCounterId.class)
@RequiredArgsConstructor
@ToString
public class NumberCounter implements Serializable {
	
    private static final long serialVersionUID = -746530815046014602L;

//	public final static String NUMBER_COUNTER_ID = "id";

    @Id
    @NonNull
    @Column(length = 4)
    private String administratorCode;
    
    @Id
    @NonNull
    @Column(length = 5)
    private String officeCode;
    
    @Column(name = "counter", columnDefinition = "integer DEFAULT 0")
    private Integer counter = 0;

    public int getNextCount() {
        Integer counter = getCounter();
        ++counter;
        setCounter(counter);
        return counter;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof NumberCounter that)) return false;
        //&&Objects.equals(counter, that.counter)
        return Objects.equals(administratorCode, that.administratorCode) && Objects.equals(officeCode, that.officeCode) ;
    }

    @Override
    public int hashCode() {
        return Objects.hash(administratorCode, officeCode);
    }
    
    public void setNumberCounterId(NumberCounterId cId) {
    	this.setAdministratorCode(cId.getAdministratorCode());
    	this.setOfficeCode(cId.getOfficeCode());
   }

	public NumberCounter(@NonNull String administratorCode, @NonNull String officeCode, Integer counter) {
		super();
		this.administratorCode = administratorCode;
		this.officeCode = officeCode;
		this.counter = counter;
	}

	public NumberCounter() {
		super();
		// TODO Auto-generated constructor stub
	}
    
    
}