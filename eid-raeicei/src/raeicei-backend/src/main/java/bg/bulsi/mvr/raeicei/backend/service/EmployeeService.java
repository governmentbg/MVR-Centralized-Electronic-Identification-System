package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.EmployeeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.IdentifierTypeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.ServiceType;
import bg.bulsi.mvr.raeicei.model.entity.Employee;
import bg.bulsi.mvr.raeicei.model.repository.view.EmployeeResultView;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import java.util.List;
import java.util.UUID;

public interface EmployeeService {

    Page<EmployeeDTO> getAllEmployees(UUID systemId, Pageable pageable);

    EmployeeResultView checkEmployee(UUID id, String uid, IdentifierTypeDTO uidType, ServiceType managerType);

    Employee getEmployeeById(UUID id);

    Employee createEmployee(EmployeeDTO employeeDTO, UUID eidManagerId);

    Employee updateEmployee(UUID id, EmployeeDTO employeeDTO, UUID eidManagerId);

    void deleteEmployee(UUID id, UUID eidManagerId);
}
