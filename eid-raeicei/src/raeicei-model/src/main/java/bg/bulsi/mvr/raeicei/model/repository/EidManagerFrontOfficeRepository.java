package bg.bulsi.mvr.raeicei.model.repository;

import bg.bulsi.mvr.raeicei.model.entity.EidManagerFrontOffice;
import bg.bulsi.mvr.raeicei.model.enums.Region;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface EidManagerFrontOfficeRepository extends JpaRepository<EidManagerFrontOffice, UUID> {

    List<EidManagerFrontOffice> findAllByEidManagerIdAndIsActive(UUID eidManagerId, Boolean isActive);

    Optional<EidManagerFrontOffice> findByName(String name);

    List<EidManagerFrontOffice> findAllByRegionAndCreateDateBetween(Region region, LocalDateTime from, LocalDateTime to);

    Boolean existsByCode(String code);
}
