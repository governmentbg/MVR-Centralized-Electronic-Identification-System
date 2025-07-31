package bg.bulsi.mvr.raeicei.model.repository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import bg.bulsi.mvr.raeicei.model.enums.EidServiceType;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;

public interface ProvidedServiceRepository extends JpaRepository<ProvidedService, UUID>{

    List<ProvidedService> findAllByManagerType(ManagerType serviceType);
    
    Optional<ProvidedService> findByApplicationTypeAndApplicationTypeIsNotNull(EidServiceType applicationType);
}
