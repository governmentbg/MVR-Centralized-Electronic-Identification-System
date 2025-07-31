package bg.bulsi.mvr.raeicei.model.repository;

import bg.bulsi.mvr.raeicei.contract.dto.EmployeeDTO;
import bg.bulsi.mvr.raeicei.model.entity.Employee;
import bg.bulsi.mvr.raeicei.model.repository.view.EmployeeResultView;
import bg.bulsi.mvr.raeicei.model.repository.view.EmployeeView;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface EmployeeRepository extends JpaRepository<Employee, UUID> {

	@Query(value = "select e from EidManager m LEFT JOIN m.employees e LEFT JOIN FETCH e.roles where m.id = :systemId AND e.isActive = true")
	public Page<Employee> findEmployeeBySystemId(@Param("systemId") UUID systemId, Pageable pageable);

	@Query(nativeQuery = true,
			value = "   WITH "
					+ " not_active_revisions AS ( "
					+ " 	SELECT ea.id, MAX(ea.rev) AS rev "
					+ " 	FROM raeicei.employee_aud ea "
					+ " 	WHERE ea.is_active IS NULL OR ea.is_active = FALSE "
					+ " 	GROUP BY ea.id "
					+ " ), "
					+ " 	target_revisions AS ( "
					+ " 	SELECT ea.id, MAX(ea.rev) AS rev "
					+ " 	FROM raeicei.employee_aud ea "
					+ " 	LEFT JOIN (SELECT * FROM not_active_revisions) na ON na.id = ea.id "
					+ " 	WHERE ea.is_active = TRUE AND (na.rev IS NULL OR ea.rev > na.rev) "
					+ " 	GROUP BY ea.id "
					+ " ) "
					+ " 	SELECT e.id, ARRAY_AGG(er.roles) AS roles , e.name, e.name_latin, e.citizen_identifier_type, e.citizen_identifier_number, e.email, e.phone_number, e.is_active "
					+ " 	FROM raeicei.employee_aud e "
					+ " 	RIGHT JOIN (SELECT * FROM target_revisions) AS rev ON rev.id = e.id "
					+ " 	LEFT JOIN raeicei.eid_manager_employees me ON e.id = me.employee_id "
					+ " 	LEFT JOIN raeicei.employee_roles er ON e.id = er.employee_id "
					+ " 	WHERE me.manager_id = :systemId AND e.is_active = TRUE "
					+ " 	GROUP BY e.id, e.name, e.name_latin, e.citizen_identifier_type, e.citizen_identifier_number, e.email, e.phone_number, e.is_active ")
	Page<EmployeeView> getAllActiveEmployeesFromAuditByQuery(@Param("systemId") UUID systemId, Pageable pageable);

	@Query(nativeQuery = true,
			value = " select e.citizen_identifier_number as uid, e.citizen_identifier_type as uidType, m.id as providerId, m.name as providerName, TRUE as isAdministrator"
				+ " from  raeicei.employee e "
				+ "        INNER join raeicei.eid_manager_employees em on e.id = em.employee_id AND em.manager_id = :managerId"
				+ "        INNER join raeicei.eid_manager m  on m.id = em.manager_id AND m.service_type = :managerType "
				+ " where e.citizen_identifier_type = :uidType AND e.citizen_identifier_number=  CAST (:citizenUid AS varchar ) ")
	public Optional<EmployeeResultView> checkEmployee(@Param("managerId") UUID id, @Param("citizenUid") String uid,
			@Param("uidType") String uidType, @Param("managerType") String managerType);
}
