package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.raeicei.backend.service.EidAdministratorService;
import bg.bulsi.mvr.raeicei.backend.service.EidCenterService;
import bg.bulsi.mvr.raeicei.backend.service.ReportService;
import bg.bulsi.mvr.raeicei.model.repository.view.ReportOfEidManagers;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;


@Slf4j
@Service
@RequiredArgsConstructor
public class ReportServiceImpl implements ReportService {

    private final EidAdministratorService eidAdministratorService;
    private final EidCenterService eidCenterService;

    @Override
    public List<List<String>> getEidAdministratorsReport() {
        List<ReportOfEidManagers> administrators = eidAdministratorService.getEidAdministratorsReport();
        List<List<String>> result = new ArrayList<>();
        result.add(ReportOfEidManagers.HEADERS);
        administrators.forEach(e -> result.add(List.of(
                checkForNullValue(e.getName()),
                checkForNullValue(e.getEikNumber()),
                checkForNullValue(e.getHomePage()),
                checkForNullValue(e.getAddress()),
                checkForNullValue(e.getEmail()),
                checkForNullValue(e.getFrontOfficeInfo()),
                checkForNullValue(e.getFrontOfficeRegion()),
                checkForNullValue(e.getFrontOfficeLatitude()),
                checkForNullValue(e.getFrontOfficeLongitude()),
                checkForNullValue(e.getFrontOfficeLocation()),
                checkForNullValue(e.getFrontOfficePhoneNumber()),
                checkForNullValue(e.getFrontOfficeWorkingHours())
        )));
        return result;
    }

    @Override
    public List<List<String>> getEidCentersReport() {
        List<ReportOfEidManagers> centers = eidCenterService.getEidCentersReport();
        List<List<String>> result = new ArrayList<>();
        result.add(ReportOfEidManagers.HEADERS);
        centers.forEach(e -> result.add(List.of(
                checkForNullValue(e.getName()),
                checkForNullValue(e.getEikNumber()),
                checkForNullValue(e.getHomePage()),
                checkForNullValue(e.getAddress()),
                checkForNullValue(e.getEmail()),
                checkForNullValue(e.getFrontOfficeInfo()),
                checkForNullValue(e.getFrontOfficeRegion()),
                checkForNullValue(e.getFrontOfficeLatitude()),
                checkForNullValue(e.getFrontOfficeLongitude()),
                checkForNullValue(e.getFrontOfficeLocation()),
                checkForNullValue(e.getFrontOfficePhoneNumber()),
                checkForNullValue(e.getFrontOfficeWorkingHours())
        )));
        return result;
    }

    private String checkForNullValue(String value) {
        return value != null ? value : "";
    }
}
