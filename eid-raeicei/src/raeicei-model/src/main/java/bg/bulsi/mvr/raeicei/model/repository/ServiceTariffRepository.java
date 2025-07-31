package bg.bulsi.mvr.raeicei.model.repository;

import java.time.LocalDate;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import bg.bulsi.mvr.raeicei.model.entity.tariif.ServiceTariff;
import bg.bulsi.mvr.raeicei.model.entity.tariif.Tariff;
import bg.bulsi.mvr.raeicei.model.repository.view.TariffView;

public interface ServiceTariffRepository extends JpaRepository<ServiceTariff, UUID> {

	    List<ServiceTariff> findAllByEidManagerId(UUID eidManagerId);
	    
//	    @Query("SELECT d FROM DeviceTariff d WHERE d.eidManager.id = :eidManagerId " +
//	            "AND d.startDate <= :date ")
//	    TariffView findByDeviceIdAndManagerId(UUID deviceId, UUID eidManagerId);
	    Optional<ServiceTariff> findFirstByEidManagerIdAndProvidedServiceIdAndStartDateLessThanEqualOrderByStartDateDesc(UUID eidManagerId, UUID providedServiceId, LocalDate date);

}
