package bg.bulsi.backend.service.impl;

import bg.bulsi.backend.mapper.CitizenIdentifierMapper;
import bg.bulsi.backend.service.CitizenIdentifierService;
import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.exception.WrongStatusException;
import bg.bulsi.mvr.common.util.CitizenIdentifierNumberValidator;
import bg.bulsi.mvr.reicontract.dto.CitizenDataDTO;
import bg.bulsi.mvr.reicontract.dto.EidentityDTO;
import bg.bulsi.mvr.reicontract.dto.EidentityRequestDTO;
import bg.bulsi.mvr.reicontract.dto.UpdateCitizenIdentifierDTO;
import bg.bulsi.reimodel.model.CitizenIdentifier;
import bg.bulsi.reimodel.model.IdentifierType;
import bg.bulsi.reimodel.repository.CitizenIdentifierRepository;
import bg.bulsi.reimodel.repository.view.CitizenIdentifierView;
import lombok.AllArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.*;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;

@Slf4j
@Service
@AllArgsConstructor
public class CitizenIdentifierServiceImpl implements CitizenIdentifierService {
    private final CitizenIdentifierMapper citizenIdentifierMapper;
    private final CitizenIdentifierRepository citizenIdentifierRepository;
    private final ValidationService validationService;
    
    @Override
    @Transactional(readOnly = true)
    //TODO: CitizenIdentifier.id needed, not eIdentityId.
    public CitizenIdentifierView getByEidentityId(UUID eIdentityId) {
        return citizenIdentifierRepository.findViewByEidentityIdAndActiveIsTrue(eIdentityId)
                .orElseThrow(() -> new EntityNotFoundException(ErrorCode.EIDENTITY_NOT_FOUND_BY_ID, eIdentityId.toString()));
    }

    @Override
    public CitizenIdentifierView getByEidentityIdInternal(UUID eidentityId) {
        return citizenIdentifierRepository.findFirstByEidentity_IdOrderByCreateDateDesc(eidentityId)
                .orElseThrow(() -> new EntityNotFoundException(ErrorCode.EIDENTITY_NOT_FOUND_BY_ID, eidentityId.toString()));

    }

    @Override
    @Transactional
    public CitizenIdentifierView create(CitizenDataDTO dto) {
    	this.validationService.validateCitizenDataDto(dto);

    	IdentifierType identifierType = IdentifierType.valueOf(dto.getCitizenIdentifierType().name());

        List<CitizenIdentifierView> existing = citizenIdentifierRepository.findByNumberAndType(dto.getCitizenIdentifierNumber(), identifierType);
        if (!existing.isEmpty()) {
            List<CitizenIdentifierView> existingCIs = existing.stream().filter(CitizenIdentifierView::getActive).toList();
            if (existingCIs.isEmpty()) {
                throw new ValidationMVRException(EXISTING_DEACTIVATED_EID);
            }
            if (existingCIs.size() > 1) {
                throw new ValidationMVRException(INTERNAL_SERVER_ERROR);
            }
            return existingCIs.get(0);
        }
        CitizenIdentifier citizenIdentifier = citizenIdentifierMapper.map(dto);

        citizenIdentifierRepository.save(citizenIdentifier);
        return getByEidentityId(citizenIdentifier.getEidentity().getId());
    }

    @Override
    @Transactional
   //TODO: CitizenIdentifier.id needed, not eIdentityId
    public CitizenIdentifierView update(UUID eIdentityId, CitizenDataDTO dto) {
    	this.validationService.validateCitizenDataDto(dto);
    	
        CitizenIdentifier citizenIdentifier = citizenIdentifierRepository.findByEidentityIdAndActiveIsTrue(eIdentityId)
                .orElseThrow(() -> new EntityNotFoundException(ErrorCode.EIDENTITY_NOT_FOUND_BY_ID, eIdentityId.toString()));
        if (!citizenIdentifier.getActive()) {
            throw new WrongStatusException(ErrorCode.EIDENTITY_NOT_ACTIVE, eIdentityId.toString());
        }
        citizenIdentifierMapper.update(dto, citizenIdentifier);
        citizenIdentifierRepository.save(citizenIdentifier);
        return getByEidentityId(eIdentityId);
    }

    @Override
    @Transactional
    public EidentityDTO updateActiveByEidentityId(UUID eIdentityId, Boolean isActive) {
        CitizenIdentifierView view = citizenIdentifierRepository.findFirstByEidentity_IdOrderByCreateDateDesc(eIdentityId)
                .orElseThrow(() -> new EntityNotFoundException(ErrorCode.EIDENTITY_NOT_FOUND_BY_ID, eIdentityId.toString()));
        CitizenIdentifier citizenIdentifier = citizenIdentifierRepository.findById(view.getId())
                .orElseThrow(() -> new EntityNotFoundException(ErrorCode.EIDENTITY_NOT_FOUND_BY_ID, eIdentityId.toString()));
        citizenIdentifier.setActive(isActive);
        citizenIdentifierRepository.save(citizenIdentifier);
        return citizenIdentifierMapper.mapToEidentityDTO(citizenIdentifier);
    }

    @Override
    public EidentityDTO findByNumberAndType(String number, IdentifierType type) {
        CitizenIdentifierView citizen = citizenIdentifierRepository.findByNumberAndTypeAndActiveIsTrue(number, type)
                .orElseThrow(() -> new EntityNotFoundException(ErrorCode.EIDENTITY_NOT_FOUND_BY_NUMBER));
        return citizenIdentifierMapper.map(citizen);
    }

    @Override
    public EidentityDTO findByNumberAndTypeInternal(String number, IdentifierType type) {
        CitizenIdentifierView citizen = citizenIdentifierRepository.findFirstByNumberAndTypeOrderByCreateDateDesc(number, type)
                .orElseThrow(() -> new EntityNotFoundException(ErrorCode.EIDENTITY_NOT_FOUND_BY_NUMBER));
        return citizenIdentifierMapper.map(citizen);
    }

    @Override
    public List<EidentityDTO> findAllByNumberAndType(List<EidentityRequestDTO> identities) {
        List<EidentityDTO> result = new ArrayList<>();
        identities.forEach(e -> {
            try {
                EidentityDTO eidentity = findByNumberAndType(e.getCitizenIdentifierNumber(), IdentifierType.valueOf(e.getCitizenIdentifierType().name()));
                result.add(eidentity);
            } catch (EntityNotFoundException ex) {
                log.error(ex.getMessage());
            }
        });
        return result;
    }

    @Override
    @Transactional
    public void updateCitizenIdentifier(UpdateCitizenIdentifierDTO dto) {
        CitizenIdentifier oldCI = citizenIdentifierRepository.findCitizenIdentifierByNumberAndTypeAndActiveIsTrue(dto.getOldCitizenIdentifierNumber(), IdentifierType.valueOf(dto.getOldCitizenIdentifierType().name()))
                .orElseThrow(() -> new EntityNotFoundException(ErrorCode.CITIZEN_IDENTIFIER_NOT_FOUND_BY_NUMBER, dto.getOldCitizenIdentifierNumber()));
        if (Objects.equals(dto.getOldCitizenIdentifierNumber(), dto.getNewCitizenIdentifierNumber()) &&
                Objects.equals(dto.getOldCitizenIdentifierType(), dto.getNewCitizenIdentifierType())) {
            throw new ValidationMVRException(CITIZEN_IDENTIFIER_IS_THE_SAME);
        }
        oldCI.setActive(false);
        citizenIdentifierRepository.save(oldCI);
        CitizenIdentifier newCI = citizenIdentifierMapper.mapToNewCI(oldCI);
        newCI.setNumber(dto.getNewCitizenIdentifierNumber());
        newCI.setType(IdentifierType.valueOf(dto.getNewCitizenIdentifierType().name()));
        citizenIdentifierRepository.save(newCI);
    }

    private boolean validateCitizenIdentifierNumber(String number, IdentifierType type) {
        return switch (type) {
            case EGN -> CitizenIdentifierNumberValidator.validateEGN(number);
            case LNCh ->  CitizenIdentifierNumberValidator.validateLNCH(number);
            default -> false;
        };
    }
}
