package bg.bulsi.mvr.mpozei.model.repository.view;

import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;

import java.util.List;
import java.util.UUID;

public interface ApplicationReportTotal {
    List<String> HEADERS = List.of("Издадени УЕИ", "Активирани УЕИ", "Спрени УЕИ", "Прекратени УЕИ");

    ApplicationType getApplicationType();

    Integer getApplicationsCount();
}
