package bg.bulsi.mvr.raeicei.backend.mapper;

import bg.bulsi.mvr.raeicei.contract.dto.EmployeeByUidResult;
import bg.bulsi.mvr.raeicei.contract.dto.EmployeeDTO;
import bg.bulsi.mvr.raeicei.model.entity.Employee;
import bg.bulsi.mvr.raeicei.model.repository.view.EmployeeResultView;
import bg.bulsi.mvr.raeicei.model.repository.view.EmployeeView;
import org.mapstruct.*;
import org.springframework.data.domain.Page;

import java.util.List;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public interface EmployeeMapper {

    @Mappings({
            @Mapping(target = "citizenIdentifierType", expression = "java(bg.bulsi.mvr.raeicei.contract.dto.IdentifierTypeDTO.fromValue(entity.getCitizenIdentifierType().name()))")
    })
    public EmployeeDTO mapToEmployeeDto(Employee entity);

    public Employee mapToEntity(EmployeeDTO dto);

    default public Page<EmployeeDTO> mapToEmployeeDTOPage(Page<Employee> employees) {
        return employees.map(this::mapToEmployeeDto);
    }

    public List<EmployeeDTO> mapToEmployeeDTOList(List<Employee> employees);

    //	@Mappings({
//		@Mapping(target = "uidType", expression = "java(bg.bulsi.mvr.raeicei.contract.dto.IdentifierTypeDTO.fromValue(entity.getCitizenIdentifierType().name()))"),
//		@Mapping(target = "uid", expression = "citizenIdentifierNumber"),
//		@Mapping(target = "providerId", expression = ""),
//		@Mapping(target = "providerName", expression = ""),
//		@Mapping(target = "isAdministrator", expression = ""),
//		})
    public EmployeeByUidResult mapToEmployeeResult(EmployeeResultView dto);

    public EmployeeDTO viewToDto(EmployeeView view);

    public List<EmployeeDTO> viewsToDtos(List<EmployeeView> employees);

    default public Page<EmployeeDTO> mapviewDTOPage(Page<EmployeeView> employees) {
        return employees.map(this::viewToDto);
    }

    @Mapping(target = "id", ignore = true)
    public Employee mapToEntity(@MappingTarget Employee entity, EmployeeDTO dto);
}
