package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.backend.service.ApplicationService;
import bg.bulsi.mvr.mpozei.backend.service.ReportService;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportByOfficesRequest;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportByOperatorsRequest;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportByRegionRequest;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportTotalRequest;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.repository.ApplicationRepository;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByOffice;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByOperators;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByRegion;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportTotal;
import java.time.format.DateTimeFormatter;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationType.*;

@Slf4j
@Service
@RequiredArgsConstructor
public class ReportServiceImpl implements ReportService {
    private final ApplicationService applicationService;
    private final ApplicationRepository<AbstractApplication> applicationRepository;
    private final ApplicationMapper applicationMapper;
    
    @Override
    public List<List<String>> getApplicationsReportByOffices(ReportByOfficesRequest dto) {
        List<ApplicationReportByOffice> applications = applicationService.getApplicationReportByOffices(dto.getEidAdministratorId(), dto.getFrom(), dto.getTo());
        List<List<String>> result = new ArrayList<>();
        result.add(ApplicationReportByOffice.HEADERS);
        applications.forEach(e -> result.add(List.of(e.getAdministratorFrontOfficeId().toString(), e.getCertificatesAmount().toString())));
        return result;
    }

    @Override
    public List<List<String>> getApplicationsReportByRegion(ReportByRegionRequest dto) {
        List<ApplicationReportByRegion> applications = applicationService.getApplicationReportByRegion(dto.getFrom(), dto.getTo());
        List<List<String>> result = new ArrayList<>();
        result.add(ApplicationReportByOffice.HEADERS);
        applications.forEach(e -> result.add(List.of(e.getRegion(), e.getCertificatesAmount().toString())));
        return result;
    }

    @Override
    public List<List<String>> getApplicationsReportTotal(ReportTotalRequest dto) {
        List<ApplicationReportTotal> applications = applicationService.getApplicationReportTotal(dto.getFrom(), dto.getTo());

        List<List<String>> result = new ArrayList<>();
        result.add(ApplicationReportTotal.HEADERS);
        List<String> applicationsCount = new ArrayList<>();
        applications.stream().filter(e -> e.getApplicationType().equals(ISSUE_EID)).findAny().ifPresentOrElse(e -> applicationsCount.add(e.getApplicationsCount().toString()), () -> applicationsCount.add("0"));
        applications.stream().filter(e -> e.getApplicationType().equals(RESUME_EID)).findAny().ifPresentOrElse(e -> applicationsCount.add(e.getApplicationsCount().toString()), () -> applicationsCount.add("0"));
        applications.stream().filter(e -> e.getApplicationType().equals(STOP_EID)).findAny().ifPresentOrElse(e -> applicationsCount.add(e.getApplicationsCount().toString()), () -> applicationsCount.add("0"));
        applications.stream().filter(e -> e.getApplicationType().equals(REVOKE_EID)).findAny().ifPresentOrElse(e -> applicationsCount.add(e.getApplicationsCount().toString()), () -> applicationsCount.add("0"));

        result.add(applicationsCount);
        return result;
    }

    @Override
    public List<ApplicationReportByOperators> getJsonApplicationsReportByOperators(ReportByOperatorsRequest dto) {
        return applicationService.getJsonApplicationReportByOperators(dto.getOperators(), dto.getFrom(), dto.getTo());
    }

    @Override
    public List<String[]> getCsvApplicationsReportByOperators(ReportByOperatorsRequest dto) {
        List<ApplicationReportByOperators> applications = applicationService.getCsvApplicationReportByOperators(dto.getOperators(), dto.getFrom(), dto.getTo());
        List<String[]> result = new ArrayList<>();
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("dd.MM.yyyy");
        result.add(new String[]{UserContextHolder.getFromServletContext().getUsername(), dto.getFrom().format(formatter), dto.getTo().format(formatter)});
        result.add(ApplicationReportByOperators.HEADERS);
        applications.forEach(e -> result.add(new String[]{e.getOperatorUsername(), String.valueOf(e.getSubmitted()), String.valueOf(e.getSigned()), String.valueOf(e.getPaid()), String.valueOf(e.getApproved()), String.valueOf(e.getGeneratedCertificate()), String.valueOf(e.getCompleted()), String.valueOf(e.getDenied())}));
        return result;
    }

	@Override
	public List<String[]> getCsvFindApplications(ApplicationFilter filter) {
		List<AbstractApplication> applications = this.applicationRepository.findByFilter(this.applicationMapper.mapToDbApplicationFilter(filter));
        List<String[]> result = new ArrayList<>();
        result.add(FIND_APPLICATIONS_HEADERS);
        applications.forEach(e -> {
        	ApplicationDTO dto = this.applicationMapper.mapToApplicationDTO(e);
        	result.add(this.applicationMapper.mapToArray(dto));
        });
		
        return result;
	}
}
