package bg.bulsi.mvr.raeicei.backend.facade;

import bg.bulsi.mvr.raeicei.contract.dto.EmployeeByUidResult;
import bg.bulsi.mvr.raeicei.contract.dto.EmployeeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.ServiceType;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import java.util.List;
import java.util.Map;
import java.util.UUID;


public interface EmployeeFacade {

    Page<EmployeeDTO> getAllEmployeeBySystemId(UUID systemId, Pageable pageable);

    EmployeeByUidResult checkEmployee(Map<String, Object> payload, ServiceType managerType);

    EmployeeDTO getEmployeeById(UUID id);

    EmployeeDTO createEmployee(EmployeeDTO dto, UUID eidManagerId);

    EmployeeDTO updateEmployee(UUID id, EmployeeDTO dto, UUID eidManagerId);

    void deleteEmployee(UUID id, UUID eidManagerId);
}