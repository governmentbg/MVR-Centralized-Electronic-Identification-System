package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.mpozei.backend.service.NomenclatureService;
import bg.bulsi.mvr.mpozei.model.nomenclature.AbstractNomenclature;
import bg.bulsi.mvr.mpozei.model.nomenclature.NomLanguage;
import bg.bulsi.mvr.mpozei.model.nomenclature.NomenclatureType;
import bg.bulsi.mvr.mpozei.model.nomenclature.ReasonNomenclature;
import bg.bulsi.mvr.mpozei.model.repository.AbstractNomenclatureRepository;
import bg.bulsi.mvr.mpozei.model.repository.NomenclatureTypeRepository;
import bg.bulsi.mvr.mpozei.model.repository.ReasonsNomRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.REASON_NOT_FOUND;
import static bg.bulsi.mvr.common.exception.ErrorCode.REASON_NOT_FOUND_BY_NAME;

@Service
@RequiredArgsConstructor
public class NomenclatureServiceImpl implements NomenclatureService {
    private static final NomLanguage DEFAULT_LANGUAGE = NomLanguage.BG;

    private final AbstractNomenclatureRepository abstractNomenclatureRepository;
    private final ReasonsNomRepository reasonNomRepository;
    private final NomenclatureTypeRepository nomenclatureTypeRepository;

    @Override
    public ReasonNomenclature getReasonById(UUID id) {
        return reasonNomRepository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException(REASON_NOT_FOUND, id));
    }

    @Override
    public ReasonNomenclature getReasonByName(String name) {
        return reasonNomRepository.findByNameAndLanguage(name, DEFAULT_LANGUAGE)
                .orElseThrow(() -> new EntityNotFoundException(REASON_NOT_FOUND_BY_NAME, name));
    }

    @Override
    public List<ReasonNomenclature> getAllReasons() {
        return reasonNomRepository.findAll();
    }

    @Override
    public void createNomenclature(AbstractNomenclature nomenclature) {
        abstractNomenclatureRepository.save(nomenclature);
    }

    @Override
    public void createNomenclatureType(NomenclatureType nomenclatureType) {
        nomenclatureTypeRepository.save(nomenclatureType);
    }

	@Override
	public List<NomenclatureType> getAllNomenclatureTypesByIds(Iterable<UUID> ids) {
        return nomenclatureTypeRepository.findAllById(ids);
	}
}
