package bg.bulsi.mvr.raeicei.model.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import bg.bulsi.mvr.raeicei.model.entity.tariif.DeviceTariff;
import bg.bulsi.mvr.raeicei.model.entity.tariif.Tariff;
import bg.bulsi.mvr.raeicei.model.repository.view.TariffView;

import java.time.LocalDate;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface TariffRepository extends JpaRepository<Tariff, UUID> {


    Optional<Tariff> findFirstByEidManagerIdAndStartDateLessThanEqualAndIsActiveOrderByStartDateDesc(UUID eidManagerId, LocalDate date, Boolean isActive);

    Boolean existsByEidManagerIdAndStartDateGreaterThanEqual(UUID eidManagerId, LocalDate startDate);

    List<Tariff> findAllByEidManagerIdAndIsActive(UUID eidManagerId, Boolean isActive);
    
    Optional<DeviceTariff>  findByDeviceIdAndEidManagerId(UUID deviceId, UUID eidManagerId);

//    @Query("SELECT d FROM DeviceTariff d WHERE d.eidManager.id = :eidManagerId " +
//            "AND d.startDate <= :date ")
//    TariffView findByDeviceIdAndManagerId(UUID deviceId, UUID eidManagerId);
}
