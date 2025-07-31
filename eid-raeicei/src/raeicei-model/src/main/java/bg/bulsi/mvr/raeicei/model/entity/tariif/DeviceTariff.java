package bg.bulsi.mvr.raeicei.model.entity.tariif;

import org.hibernate.envers.Audited;

import bg.bulsi.mvr.raeicei.model.entity.Device;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;
import jakarta.persistence.ManyToOne;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.experimental.SuperBuilder;

@Getter
@Setter
@Audited
@Entity
@NoArgsConstructor
@AllArgsConstructor
@DiscriminatorValue(value = TariffType.Fields.DEVICE_TARIFF)
public class DeviceTariff extends Tariff {

    private static final long serialVersionUID = 5520523098386189314L;
	@ManyToOne(optional = true)
    private Device device;
}
