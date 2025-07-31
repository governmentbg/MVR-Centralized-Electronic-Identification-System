package bg.bulsi.mvr.raeicei.model.repository;

import bg.bulsi.mvr.raeicei.model.entity.Document;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.UUID;

@Repository
public interface DocumentRepository extends JpaRepository<Document, UUID> {

//    List<Document> findDocumentByApplicationId(UUID id);
}
