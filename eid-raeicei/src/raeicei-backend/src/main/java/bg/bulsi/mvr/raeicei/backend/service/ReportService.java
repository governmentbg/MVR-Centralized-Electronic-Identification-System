package bg.bulsi.mvr.raeicei.backend.service;

import java.util.List;

public interface ReportService {

    List<List<String>> getEidAdministratorsReport();

    List<List<String>> getEidCentersReport();
}
