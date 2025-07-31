package bg.bulsi.mvr.raeicei.model.repository.view;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.UUID;

import bg.bulsi.mvr.raeicei.model.entity.Device;
import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import bg.bulsi.mvr.raeicei.model.enums.Currency;

import lombok.Data;

@Data
public class TariffView {

    private UUID id;

    private LocalDate startDate;

    private BigDecimal price;
    
    private Currency currency;
    
    private EidManager eidManager;

    private ProvidedService providedService;

    private Device device;

}
