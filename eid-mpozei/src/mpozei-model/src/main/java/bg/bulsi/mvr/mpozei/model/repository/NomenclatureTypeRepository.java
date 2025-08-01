package bg.bulsi.mvr.mpozei.model.repository;

import bg.bulsi.mvr.mpozei.model.nomenclature.NomenclatureType;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.UUID;

@Repository
public interface NomenclatureTypeRepository extends JpaRepository<NomenclatureType, UUID> {
}
