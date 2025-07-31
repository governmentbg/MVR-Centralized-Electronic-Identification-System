package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.raeicei.backend.mapper.DiscountMapper;
import bg.bulsi.mvr.raeicei.backend.service.DiscountService;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.Discount;
import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import bg.bulsi.mvr.raeicei.model.repository.DiscountRepository;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerRepository;
import bg.bulsi.mvr.raeicei.model.repository.ProvidedServiceRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.EID_ADMINISTRATOR_NOT_FOUND;
import static bg.bulsi.mvr.common.exception.ErrorCode.ENTITY_NOT_FOUND;

@Slf4j
@Service
@RequiredArgsConstructor
public class DiscountServiceImpl implements DiscountService {

    private final DiscountRepository discountRepository;
    private final DiscountMapper mapper;
    private final EidManagerRepository eidManagerRepository;
    private final ProvidedServiceRepository providedServiceRepository;

    @Value("${exchange-rates.bgn-to-eur}")
    private Double bgnToEuroExchangeRate;

    @Value("${exchange-rates.eur-to-bgn}")
    private Double euroToBgnExchangeRate;

    @Override
    public Discount getDiscountByDateAndEidManagerId(DiscountDateDTO dto) {
        return discountRepository.findFirstByEidManagerIdAndStartDateLessThanEqualAndIsActiveOrderByStartDateDesc(dto.getEidManagerId(), dto.getDate(), true).orElseThrow(()
                -> new EntityNotFoundException(ENTITY_NOT_FOUND, dto.getEidManagerId().toString(), dto.getEidManagerId().toString()));
    }

    @Override
    public Discount getDiscountById(UUID id) {
        return discountRepository.findById(id).orElseThrow(() -> new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, "Discount", id.toString()));
    }

    @Override
    public DiscountDoubleCurrencyResponseDTO getDoubleCurrencyDiscountById(UUID id) {
        Discount entity = discountRepository.findById(id).orElseThrow(() -> new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, "Discount", id.toString()));
        DiscountDoubleCurrencyResponseDTO responseDTO = mapper.mapToDiscountDoubleCurrencyResponseDto(entity);

//        if (Currency.BGN.equals(entity.getCurrency())) {
//            responseDTO.setPrimaryDiscountAmount(entity.getValue());
//            responseDTO.setPrimaryDiscountAmountCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.BGN);
//            responseDTO.setSecondaryDiscountAmount(BigDecimal.valueOf(responseDTO.getPrimaryDiscountAmount() * bgnToEuroExchangeRate)
//                    .setScale(2, RoundingMode.HALF_UP)
//                    .doubleValue());
//            responseDTO.setSecondaryDiscountAmountCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.EUR);
//        } else {
//            responseDTO.setPrimaryDiscountAmount(entity.getValue());
//            responseDTO.setPrimaryDiscountAmountCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.EUR);
//            responseDTO.setSecondaryDiscountAmount(BigDecimal.valueOf(responseDTO.getPrimaryDiscountAmount() * euroToBgnExchangeRate)
//                    .setScale(2, RoundingMode.HALF_UP)
//                    .doubleValue());
//            responseDTO.setSecondaryDiscountAmountCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.BGN);
//        }

        return responseDTO;
    }

    @Override
    public Discount createDiscount(DiscountResponseDTO dto) {
        if (discountRepository.existsByEidManagerIdAndProvidedServiceIdAndStartDateGreaterThanEqual(dto.getEidManagerId(), dto.getProvidedServiceId(), dto.getStartDate())) {
            throw new ValidationMVRException(ErrorCode.DISCOUNT_ALREADY_EXISTS_FOR_THE_DATE);
        }

        Discount entity = mapper.mapToEntity(dto);
        entity.setIsActive(true);

        if (dto.getDisability()) {
            entity.setDisability(true);
            entity.setValue(dto.getDiscount());
            entity.setAgeFrom(0);
            entity.setAgeUntil(200);
            entity.setStartDate(dto.getStartDate());
        }

        if (dto.getProvidedServiceId() != null) {
            ProvidedService providedService = providedServiceRepository.findById(dto.getProvidedServiceId()).orElseThrow();
            entity.setProvidedService(providedService);
        }

        EidManager eidManager = getEidManagerById(dto.getEidManagerId());

        checkEidManagerCurrentStatus(eidManager);

        entity.setEidManager(eidManager);

        return discountRepository.save(entity);
    }

    @Override
    public List<Discount> getAllDiscountsByEidManagerId(UUID id) {
        return discountRepository.findAllByEidManagerIdAndIsActive(id, true);
    }

    @Override
    public DiscountResponseDTO updateDiscount(UUID id, DiscountDTO discountDTO, UUID eidManagerId) {
        Discount entity = getDiscountById(id);

        EidManager eidManager = getEidManagerById(eidManagerId);

        checkEidManagerCurrentStatus(eidManager);

        entity = mapper.mapToEntity(entity, discountDTO);
        entity.setIsActive(true);
        discountRepository.save(entity);
        return mapper.mapToResponseDto(entity);
    }

    @Override
    public void deleteDiscount(UUID id) {
        Discount discount = getDiscountById(id);

        EidManager eidManager = getEidManagerById(discount.getEidManager().getId());

        checkEidManagerCurrentStatus(eidManager);

        discount.setIsActive(false);
    }

    private EidManager getEidManagerById(UUID eidManagerId) {
        return eidManagerRepository.findById(eidManagerId).orElseThrow(() ->
                new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, eidManagerId.toString(), eidManagerId.toString()));
    }

    private static void checkEidManagerCurrentStatus(EidManager eidManager) {
        if (EidManagerStatus.STOP.equals(eidManager.getManagerStatus()) || EidManagerStatus.SUSPENDED.equals(eidManager.getManagerStatus()) || EidManagerStatus.IN_REVIEW.equals(eidManager.getManagerStatus())) {
            throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
        }
    }
}
