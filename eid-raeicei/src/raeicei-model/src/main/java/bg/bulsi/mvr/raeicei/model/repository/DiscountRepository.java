package bg.bulsi.mvr.raeicei.model.repository;

import bg.bulsi.mvr.raeicei.model.entity.Discount;
import bg.bulsi.mvr.raeicei.model.entity.tariif.Tariff;
import feign.Param;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface DiscountRepository extends JpaRepository<Discount, UUID>{

    Optional<Discount> findFirstByEidManagerIdAndStartDateLessThanEqualAndIsActiveOrderByStartDateDesc(UUID eidManagerId, LocalDate date, Boolean isActive);

    Boolean existsByEidManagerIdAndProvidedServiceIdAndStartDateGreaterThanEqual(UUID eidManagerId, UUID providedServiceId, LocalDate startDate);

    List<Discount> findAllByEidManagerIdAndIsActive(UUID eidManagerId, Boolean isActive);

    @Query("""
    		SELECT d FROM Discount d WHERE d.eidManager.id = :eidManagerId 
            AND (:applicantAge IS NULL OR (d.ageFrom <= :applicantAge AND d.ageUntil >= :applicantAge)) 
            AND (:isDisabilityRequired IS NULL OR d.disability = :isDisabilityRequired) 
            AND (:isOnlineService IS NULL OR d.onlineService  = :isOnlineService) 
            AND (:isOnlineService IS NULL OR d.onlineService  = :isOnlineService) 
            AND (:providedServiceId IS NULL OR d.providedService.id  = :providedServiceId) 
    		AND (d.startDate <= :date) 
            ORDER BY  d.value DESC, d.startDate DESC
            FETCH FIRST 1 rows only   		
    		""")
//cast(ivd.trnDatetime as LocalDate):date IS NULL OR 

    Optional<Discount> findDiscount(
            @Param("eidManagerId") UUID eidManagerId,
            @Param("date") LocalDate date,
            @Param("applicantAge") Integer applicantAge,
            @Param("isDisabilityRequired") Boolean isDisabilityRequired,
            @Param("isOnlineService") Boolean isOnlineService,
            @Param("providedServiceId") UUID providedServiceId);

}
