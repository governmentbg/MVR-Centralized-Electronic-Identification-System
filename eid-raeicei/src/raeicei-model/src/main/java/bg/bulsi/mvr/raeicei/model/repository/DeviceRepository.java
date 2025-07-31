package bg.bulsi.mvr.raeicei.model.repository;

import bg.bulsi.mvr.raeicei.contract.dto.DeviceType;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface DeviceRepository extends JpaRepository<Device, UUID> {

    List<Device> findAllByType(DeviceType type);
    
    @Query(value = "SELECT em.devices FROM EidManager em WHERE TYPE(em) = EidAdministrator AND em.id = :aeiId")
    Optional<Device> findDevicesByAdministratorId(@Param("aeiId") UUID aeiId);
    
    List<Device> findAllByIsActiveTrue();
}
