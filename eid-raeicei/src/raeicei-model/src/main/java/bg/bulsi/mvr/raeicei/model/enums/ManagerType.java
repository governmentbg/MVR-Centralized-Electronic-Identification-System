package bg.bulsi.mvr.raeicei.model.enums;

import java.util.List;
import java.util.UUID;

import org.hibernate.envers.Audited;

import bg.bulsi.mvr.raeicei.model.AbstractAudit;
import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.OneToMany;
import jakarta.persistence.Table;
import lombok.Getter;
import lombok.Setter;
import lombok.experimental.FieldNameConstants;

//public enum ManagerType {
//	EIDENTITY_AUTHENTICATION,
//	EXTENTED_EIDENTITY_AUTHENTICATION
//}

//@Getter
//@Setter
//@Entity
//@Audited
//@Table(schema = "raeicei", name = "service_type")
//public class ManagerType extends AbstractAudit<String> {
//	
//    @Id
//    @GeneratedValue(strategy = GenerationType.UUID)
//    @Column(name = "id", nullable = false, unique = true, updatable = false)
//    private UUID id;
//
//    @Column(nullable = false, unique = true)
//    private String name;
//
//    @OneToMany(mappedBy = "serviceType", cascade = CascadeType.ALL)
//    private List<ProvidedService> providedServices;
//    
//    public static class SupportedTypes {
//    	
//    	private static final String EID_CENTER = "EID_CENTER";
//    	
//    	private static final String EID_ADMINISTRATOR = "EID_ADMINISTRATOR";
//    }
//}

@FieldNameConstants(onlyExplicitlyIncluded = true)
public enum ManagerType {
	@FieldNameConstants.Include  EID_CENTER,
	@FieldNameConstants.Include  EID_ADMINISTRATOR
}
