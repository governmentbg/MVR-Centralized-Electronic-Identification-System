package bg.bulsi.mvr.mpozei.model.repository;

import bg.bulsi.mvr.mpozei.model.nomenclature.NomLanguage;
import bg.bulsi.mvr.mpozei.model.nomenclature.ReasonNomenclature;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface ReasonsNomRepository extends AbstractNomenclatureRepository<ReasonNomenclature> {
    Optional<ReasonNomenclature> findByNameAndLanguage(String name, NomLanguage language);

    Optional<ReasonNomenclature> findById(UUID id);
}
