package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.raeicei.backend.service.*;
import bg.bulsi.mvr.raeicei.model.entity.Contact;
import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.entity.EidManagerFrontOffice;
import bg.bulsi.mvr.raeicei.model.entity.Employee;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import lombok.AllArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@AllArgsConstructor
@Service
public class EidManagerServiceImpl implements EidManagerService {

    private final ProvidedServiceService providedServiceService;

    private final EidManagerFrontOfficeService eidManagerFrontOfficeService;

    private final AuthorizedPersonService authorizedPersonService;

    private final EmployeeService employeeService;

    @Override
    public void addProvidedServicesToEidManager(EidManager eidManager) {
        List<ProvidedService> providedServices = this.providedServiceService.getAllProvidedServicesByType(eidManager.getServiceType());

        if (providedServices != null && !providedServices.isEmpty()) {
            eidManager.setProvidedServices(providedServices);
        }
    }

    @Override
    public void addFrontOfficesToEidManager(EidManager eidManager, List<UUID> frontOfficesIds) {
        if (frontOfficesIds != null) {
            eidManager.getEidManagerFrontOffices().clear();
            for (UUID id : frontOfficesIds) {
                EidManagerFrontOffice administratorFrontOffice = eidManagerFrontOfficeService.getEidManagerFrontOfficeById(id);
                administratorFrontOffice.setEidManager(eidManager);
            }
        }
    }

    @Override
    public void addAuthorizedPersonsToEidManager(EidManager eidManager, List<UUID> authorizedPersonsIds) {
        if (authorizedPersonsIds != null) {
            eidManager.getAuthorizedPersons().clear();
            for (UUID id : authorizedPersonsIds) {
                Contact authorizedPerson = authorizedPersonService.getAuthorizedPersonById(id);
                eidManager.getAuthorizedPersons().add(authorizedPerson);
            }
        }
    }

    @Override
    public void addEmployeesToEidManager(EidManager eidManager, List<UUID> employeesIds) {
        if (employeesIds != null) {
            eidManager.getEmployees().clear();
            for (UUID id : employeesIds) {
                Employee employee = employeeService.getEmployeeById(id);
                eidManager.getEmployees().add(employee);
            }
        }
    }
}
