package bg.bulsi.mvr.mpozei.model.repository.view;

import java.util.List;

public interface ApplicationReportByRegion {
    default List<String> getHeaders() {
        return List.of("Регион", "Брой УЕИ");
    }

	String getRegion();
    Integer getCertificatesAmount();

}
