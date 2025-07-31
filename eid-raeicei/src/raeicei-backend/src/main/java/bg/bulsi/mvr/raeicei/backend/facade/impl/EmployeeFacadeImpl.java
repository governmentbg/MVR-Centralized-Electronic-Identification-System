package bg.bulsi.mvr.raeicei.backend.facade.impl;

import bg.bulsi.mvr.raeicei.backend.facade.EmployeeFacade;
import bg.bulsi.mvr.raeicei.backend.mapper.EmployeeMapper;
import bg.bulsi.mvr.raeicei.backend.service.EmployeeService;
import bg.bulsi.mvr.raeicei.contract.dto.EmployeeByUidResult;
import bg.bulsi.mvr.raeicei.contract.dto.EmployeeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.IdentifierTypeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.ServiceType;
import bg.bulsi.mvr.raeicei.model.entity.Employee;
import bg.bulsi.mvr.raeicei.model.repository.view.EmployeeResultView;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Map;
import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class EmployeeFacadeImpl implements EmployeeFacade {

    protected final EmployeeMapper employeeMapper;

    protected final EmployeeService employeeService;

    @Override
    public Page<EmployeeDTO> getAllEmployeeBySystemId(UUID systemId, Pageable pageable) {
        return employeeService.getAllEmployees(systemId, pageable);
    }

    @Override
    public EmployeeByUidResult checkEmployee(Map<String, Object> payload, ServiceType managerType) {
        UUID id = ((UUID) payload.get("id"));
        String uid = payload.get("uid").toString();
        IdentifierTypeDTO uidType = ((IdentifierTypeDTO) payload.get("uidType"));

        log.info("Check employee managerId: {}, managerType: {}, employeeUidType: {}, employeeUid: {}", id, managerType, uidType, uid);
        EmployeeResultView emplResult = employeeService.checkEmployee(id, uid, uidType, managerType);

        log.info("EmployeeResultView: {}", emplResult.toString());
        return employeeMapper.mapToEmployeeResult(emplResult);
    }

    @Override
    public EmployeeDTO getEmployeeById(UUID id) {
        Employee entity = employeeService.getEmployeeById(id);
        return employeeMapper.mapToEmployeeDto(entity);
    }

    @Override
    public EmployeeDTO createEmployee(EmployeeDTO dto, UUID eidManagerId) {
        Employee entity = employeeService.createEmployee(dto, eidManagerId);
        return employeeMapper.mapToEmployeeDto(entity);
    }

    @Override
    public EmployeeDTO updateEmployee(UUID id, EmployeeDTO dto, UUID eidManagerId) {
        Employee entity = employeeService.updateEmployee(id, dto, eidManagerId);
        return employeeMapper.mapToEmployeeDto(entity);
    }

    @Override
    public void deleteEmployee(UUID id, UUID eidManagerId) {
        employeeService.deleteEmployee(id, eidManagerId);
    }
}
