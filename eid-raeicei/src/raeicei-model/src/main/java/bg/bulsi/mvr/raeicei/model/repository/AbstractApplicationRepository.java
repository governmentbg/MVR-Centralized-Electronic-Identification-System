package bg.bulsi.mvr.raeicei.model.repository;

import bg.bulsi.mvr.raeicei.model.entity.application.AbstractApplication;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.UUID;

@Repository
public interface AbstractApplicationRepository extends JpaRepository<AbstractApplication, UUID> {
}
