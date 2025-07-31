package bg.bulsi.mvr.raeicei.model.entity.tariif;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.UUID;

import org.hibernate.envers.Audited;

import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import bg.bulsi.mvr.raeicei.model.enums.Currency;
import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;
import jakarta.persistence.ManyToOne;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@Audited
@Entity
@NoArgsConstructor
@DiscriminatorValue(value = TariffType.Fields.SERVICE_TARIFF)
public class ServiceTariff extends Tariff {
	
    private static final long serialVersionUID = -3386262845481839085L;

	@ManyToOne(optional = true)
    private ProvidedService providedService;
	

	public ServiceTariff(ProvidedService providedService,UUID id, LocalDate startDate, BigDecimal price, Currency currency, EidManager eidManager, Boolean isActive) {
		super(id,startDate,price,currency,eidManager, isActive);
		this.providedService = providedService;
	}

}