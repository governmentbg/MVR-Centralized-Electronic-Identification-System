package bg.bulsi.mvr.mpozei.backend.service;

import bg.bulsi.mvr.mpozei.model.nomenclature.AbstractNomenclature;
import bg.bulsi.mvr.mpozei.model.nomenclature.NomenclatureType;
import bg.bulsi.mvr.mpozei.model.nomenclature.ReasonNomenclature;

import java.util.List;
import java.util.UUID;

public interface NomenclatureService {
    ReasonNomenclature getReasonById(UUID id);

    ReasonNomenclature getReasonByName(String name);

    List<ReasonNomenclature> getAllReasons();
    
    List<NomenclatureType> getAllNomenclatureTypesByIds(Iterable<UUID> ids);

    void createNomenclature(AbstractNomenclature nomenclature);

    void createNomenclatureType(NomenclatureType nomenclatureType);
}
