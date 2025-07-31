package bg.bulsi.mvr.raeicei.model.entity.application;

import bg.bulsi.mvr.raeicei.model.enums.ManagerType;
import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.Audited;

@Getter
@Setter
@Entity
@Audited
@DiscriminatorValue(value = ManagerType.Fields.EID_CENTER)
public class EidCenterApplication extends AbstractApplication{

}
