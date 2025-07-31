package bg.bulsi.mvr.mpozei.model.repository;

import bg.bulsi.mvr.mpozei.model.nomenclature.AbstractNomenclature;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.UUID;
@Repository
public interface AbstractNomenclatureRepository<T extends AbstractNomenclature> extends JpaRepository<T, UUID> {
}
