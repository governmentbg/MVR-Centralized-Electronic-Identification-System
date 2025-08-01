package bg.bulsi.mvr.mpozei.model.repository.view;

import java.util.List;
import java.util.UUID;

public interface ApplicationReportByOffice {
    List<String> HEADERS = List.of("Офис", "Брой УЕИ");

    UUID getAdministratorFrontOfficeId();
    Integer getCertificatesAmount();
}
