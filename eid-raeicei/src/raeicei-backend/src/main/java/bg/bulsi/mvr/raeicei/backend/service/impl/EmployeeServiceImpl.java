package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.raeicei.backend.mapper.EmployeeMapper;
import bg.bulsi.mvr.raeicei.backend.service.EmployeeService;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerStatus;
import bg.bulsi.mvr.raeicei.contract.dto.EmployeeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.IdentifierTypeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.ServiceType;
import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.entity.Employee;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerRepository;
import bg.bulsi.mvr.raeicei.model.repository.EmployeeRepository;
import bg.bulsi.mvr.raeicei.model.repository.view.EmployeeResultView;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.EID_ADMINISTRATOR_NOT_FOUND;
import static bg.bulsi.mvr.common.exception.ErrorCode.EMPLOYEE_NOT_FOUND;

@Slf4j
@Service
@RequiredArgsConstructor
public class EmployeeServiceImpl implements EmployeeService {

    @Autowired
    protected EmployeeRepository employeeRepository;
    @Autowired
    private final EidManagerRepository eidManagerRepository;
    @Autowired
    protected EmployeeMapper mapper;

    @Override
    public Page<EmployeeDTO> getAllEmployees(UUID systemId, Pageable pageable) {
        return mapper.mapviewDTOPage(employeeRepository.getAllActiveEmployeesFromAuditByQuery(systemId, pageable));
    }

    @Override
    public EmployeeResultView checkEmployee(UUID id, String uid, IdentifierTypeDTO uidType, ServiceType managerType) {
        EmployeeResultView employee = employeeRepository.checkEmployee(id, uid, uidType.name(), managerType.name())
                .orElseThrow(() -> new EntityNotFoundException(EMPLOYEE_NOT_FOUND, id));
        return employee;
    }

    @Override
    public Employee getEmployeeById(UUID id) {
        return employeeRepository.findById(id).orElseThrow(() -> new EntityNotFoundException(EMPLOYEE_NOT_FOUND, id.toString(), id.toString()));
    }

    @Override
    public Employee createEmployee(EmployeeDTO dto, UUID eidManagerId) {
        Employee entity = mapper.mapToEntity(dto);
        entity.setIsActive(null);

        EidManager eidManager = getEidManagerById(eidManagerId);

        checkEidManagerCurrentStatus(eidManager);

        eidManager.getEmployees().add(entity);
        eidManager.setManagerStatus(EidManagerStatus.PENDING_ATTACHMENTS);

        return employeeRepository.save(entity);
    }

    @Override
    public Employee updateEmployee(UUID id, EmployeeDTO employeeDTO, UUID eidManagerId) {
        Employee entity = getEmployeeById(id);
        entity = mapper.mapToEntity(entity, employeeDTO);
        entity.setIsActive(null);

        EidManager eidManager = getEidManagerById(eidManagerId);

        checkEidManagerCurrentStatus(eidManager);

        eidManager.setManagerStatus(EidManagerStatus.PENDING_ATTACHMENTS);

        return employeeRepository.save(entity);
    }

    @Override
    public void deleteEmployee(UUID id, UUID eidManagerId) {
        Employee employee = getEmployeeById(id);

        employee.setIsActive(false);

        EidManager eidManager = getEidManagerById(eidManagerId);

        checkEidManagerCurrentStatus(eidManager);

        eidManager.getEmployees().remove(employee);
    }

    private EidManager getEidManagerById(UUID eidManagerId) {
        return eidManagerRepository.findById(eidManagerId).orElseThrow(() ->
                new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, eidManagerId.toString(), eidManagerId.toString()));
    }

    private static void checkEidManagerCurrentStatus(EidManager eidManager) {
        if (EidManagerStatus.STOP.equals(eidManager.getManagerStatus()) || EidManagerStatus.SUSPENDED.equals(eidManager.getManagerStatus()) || EidManagerStatus.IN_REVIEW.equals(eidManager.getManagerStatus())) {
            throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
        }
    }
}
