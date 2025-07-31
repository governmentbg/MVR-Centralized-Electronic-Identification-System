package bg.bulsi.mvr.raeicei.model.repository;

import bg.bulsi.mvr.raeicei.model.entity.DocumentType;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.UUID;

public interface DocumentTypeRepository extends JpaRepository<DocumentType, UUID> {
    List<DocumentType> findAllByActiveTrue();
}
