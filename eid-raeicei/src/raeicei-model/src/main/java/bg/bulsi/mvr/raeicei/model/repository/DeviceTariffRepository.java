package bg.bulsi.mvr.raeicei.model.repository;

import bg.bulsi.mvr.raeicei.model.entity.tariif.DeviceTariff;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.UUID;

@Repository
public interface DeviceTariffRepository extends JpaRepository<DeviceTariff, UUID>{

	DeviceTariff findByDeviceIdAndEidManagerId(UUID deviceId, UUID eidManagerId);
	
//    Boolean existsByEidAdministratorIdAndStartDateGreaterThanEqual(UUID eidAdministratorId, LocalDate startDate);
//
//    Optional<DeviceTariff> findFirstByEidAdministratorIdAndStartDateLessThanEqualOrderByStartDateDesc(UUID eidAdministratorId, LocalDate date);
//
//    List<DeviceTariff> findAllByEidAdministratorId(UUID id);
//
//    DeviceTariff findByDeviceId (UUID deviceId);
}
