package bg.bulsi.mvr.raeicei.model.repository;

import bg.bulsi.mvr.raeicei.model.entity.Note;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.UUID;

@Repository
public interface NoteRepository extends JpaRepository<Note, UUID> {
}
