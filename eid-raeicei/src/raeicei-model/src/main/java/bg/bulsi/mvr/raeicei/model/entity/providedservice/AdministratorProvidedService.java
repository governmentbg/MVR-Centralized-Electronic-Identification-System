//package bg.bulsi.mvr.raeicei.model.entity.providedservice;
//
//import org.hibernate.envers.Audited;
//
//import bg.bulsi.mvr.raeicei.model.entity.ServiceType;
//import bg.bulsi.mvr.raeicei.model.entity.ServiceType.Fields;
//import jakarta.persistence.Column;
//import jakarta.persistence.DiscriminatorValue;
//import jakarta.persistence.Entity;
//import jakarta.persistence.EnumType;
//import jakarta.persistence.Enumerated;
//import lombok.Getter;
//import lombok.Setter;
//
//@Getter
//@Setter
//@Audited
//@Entity
//@DiscriminatorValue(value = ManagerType.Fields.EID_ADMINISTRATOR)
//public class AdministratorProvidedService extends ProvidedService {
//
//    @Column(name = "application_type", insertable = false, updatable = false)
//    @Enumerated(EnumType.STRING)
//	private EidServiceType applicationType;
//}
