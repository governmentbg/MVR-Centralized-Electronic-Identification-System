package bg.bulsi.mvr.mpozei.backend.service;

import bg.bulsi.mvr.mpozei.contract.dto.ApplicationFilter;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportByOfficesRequest;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportByOperatorsRequest;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportByRegionRequest;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportTotalRequest;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByOperators;

import java.util.List;

public interface ReportService {
    String[] FIND_APPLICATIONS_HEADERS = {"Заявление №", "Тип заявление", "Офис", "Подадено чрез", "Подадено на", "Заявен носител", "Администратор", "Статус"};
	
    List<List<String>> getApplicationsReportByOffices(ReportByOfficesRequest dto);

    List<List<String>> getApplicationsReportByRegion(ReportByRegionRequest dto);

    List<List<String>> getApplicationsReportTotal(ReportTotalRequest dto);

    List<ApplicationReportByOperators> getJsonApplicationsReportByOperators(ReportByOperatorsRequest dto);

    List<String[]> getCsvApplicationsReportByOperators(ReportByOperatorsRequest dto);
    
    List<String[]> getCsvFindApplications(ApplicationFilter filter);
}
