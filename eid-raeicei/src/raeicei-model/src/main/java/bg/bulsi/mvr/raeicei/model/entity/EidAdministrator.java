package bg.bulsi.mvr.raeicei.model.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

import org.hibernate.envers.AuditJoinTable;
import org.hibernate.envers.Audited;

import bg.bulsi.mvr.raeicei.model.enums.ManagerType;

import java.io.Serial;
import java.util.*;

@Getter
@Setter
@Entity
@Audited
//@Table(name = "eid_administrator", schema = "raeicei")
@DiscriminatorValue(value = ManagerType.Fields.EID_ADMINISTRATOR)
public class EidAdministrator extends EidManager {
    @Serial
    private static final long serialVersionUID = 7367622588110356254L;

    @ManyToMany(fetch = FetchType.EAGER)
    @JoinTable(
    	      name = "eid_administrator_device", 
    	      joinColumns = @JoinColumn(name = "eid_administrator_id"), 
    	      inverseJoinColumns = @JoinColumn(name = "device_id"))
    @AuditJoinTable(name = "jt_eid_administrator_device_aud")
    @OrderBy("lastUpdate DESC")
    private List<Device> devices = new ArrayList<>();

    @Column
    private String downloadUrl;
}
