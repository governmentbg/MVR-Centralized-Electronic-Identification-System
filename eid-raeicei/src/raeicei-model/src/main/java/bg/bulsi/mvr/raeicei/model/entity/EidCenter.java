package bg.bulsi.mvr.raeicei.model.entity;

import java.util.List;
import java.util.UUID;

import org.hibernate.annotations.ColumnTransformer;
import org.hibernate.envers.Audited;

import bg.bulsi.mvr.raeicei.model.enums.Region;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;
import jakarta.persistence.Column;
import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;
import jakarta.persistence.Lob;
import jakarta.persistence.Table;
import lombok.Getter;
import lombok.Setter;

/**
 * Center for electronic identification
 */
@Getter
@Setter
@Audited
@Entity
///@Table(name = "eid_center", schema = "raeicei")
@DiscriminatorValue(value = ManagerType.Fields.EID_CENTER)
public class EidCenter extends EidManager {

	private static final long serialVersionUID = -7816952100576967252L;

	private String clientId;
	
	//https://www.geeksforgeeks.org/spring-boot-enhancing-data-security-column-level-encryption/
	private String clientSecret;
	
}
