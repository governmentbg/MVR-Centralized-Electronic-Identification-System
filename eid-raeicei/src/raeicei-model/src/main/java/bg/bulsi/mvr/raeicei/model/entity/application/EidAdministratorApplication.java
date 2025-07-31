package bg.bulsi.mvr.raeicei.model.entity.application;

import bg.bulsi.mvr.raeicei.model.entity.Device;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.envers.AuditJoinTable;
import org.hibernate.envers.Audited;

import java.util.List;

@Getter
@Setter
@Entity
@Audited
@DiscriminatorValue(value = ManagerType.Fields.EID_ADMINISTRATOR)
public class EidAdministratorApplication extends AbstractApplication {

    @OneToMany(cascade = CascadeType.PERSIST, fetch = FetchType.LAZY)
    @JoinTable(
            name = "application_devices",
            joinColumns = @JoinColumn(name = "application_id"),
            inverseJoinColumns = @JoinColumn(name = "device_id"))
    @AuditJoinTable(name = "jt_application_devices_aud")
    private List<Device> devices;
}
